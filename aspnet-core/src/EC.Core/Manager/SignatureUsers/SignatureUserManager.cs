using EC.Entities;
using EC.Manager.SignatureUsers.Dto;
using HRMv2.NccCore;
using Microsoft.AspNetCore.Server.IISIntegration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EC.Manager.SignatureUsers
{
    public class SignatureUserManager : BaseManager, ISignatureUserManager
    {
        public SignatureUserManager(IWorkScope workScope) : base(workScope)
        {
        }

        public async Task<CreateSignatureUserDto> Create(CreateSignatureUserDto input)
        {
            var entity = ObjectMapper.Map<SignatureUser>(input);

            var id = await WorkScope.InsertAndGetIdAsync(entity);

            if (entity.IsDefault)
            {
                await UnDefaultSignatures(id);
            }

            return input;
        }

        public async Task UnDefaultSignatures(long defaultId)
        {
            var otherDefault = await WorkScope.GetAll<SignatureUser>()
                 .Where(x => x.UserId == AbpSession.UserId)
                 .Where(x => x.Id != defaultId)
                 .Where(x => x.IsDefault)
                 .ToListAsync();

            foreach (var signature in otherDefault)
            {
                signature.IsDefault = false;
            }

            CurrentUnitOfWork.SaveChanges();
        }

        public GetSignatureUserDto Get(long id)
        {
            return QueryAllSignatureUser()
                .Where(x => x.Id == id)
                .FirstOrDefault();
        }

        public async Task<List<GetSignatureUserDto>> GetAll()
        {
            var loginUser = AbpSession.UserId;

            return await QueryAllSignatureUser()
                .Where(x => x.UserId == loginUser)
                .Select(x => new GetSignatureUserDto
                {
                    Id = x.Id,
                    SignatureType = x.SignatureType,
                    UserId = x.UserId,
                    File = x.File,
                    FileBase64 = x.FileBase64,
                    IsDefault = x.IsDefault,
                    CreationTime = x.CreationTime,
                    LastModificationTime = x.LastModificationTime,
                    LastModifierUser = x.LastModifierUser,
                }).ToListAsync();
        }

        public async Task<List<GetSignatureUserDto>> GetAllByEmail(long settingId)
        {
            var email = await WorkScope.GetAll<ContractSetting>()
                .Where(x => x.Id == settingId)
                .Select(x => x.SignerEmail)
                .FirstOrDefaultAsync();

            return await WorkScope.GetAll<SignatureUser>()
                            .Where(x => x.User.EmailAddress.ToLower() == email.ToLower())
                            .OrderByDescending(x => x.CreationTime)
                            .Include(x => x.User)
                            .Select(x => new GetSignatureUserDto
                            {
                                Id = x.Id,
                                SignatureType = x.SignatureType,
                                UserId = x.UserId,
                                File = x.File,
                                FileBase64 = x.FileBase64,
                                IsDefault = x.IsDefault,
                            }).ToListAsync();
        }

        public async Task<UpdateSignatureUserDto> Update(UpdateSignatureUserDto input)
        {
            var entity = await WorkScope.GetAsync<SignatureUser>(input.Id);
            entity.SignatureTypeId = input.SignatureTypeId;
            entity.UserId = input.UserId;
            entity.File = input.File;
            entity.FileBase64 = input.FileBase64;
            entity.IsDefault = input.IsDefault;

            await WorkScope.UpdateAsync(entity);

            if (entity.IsDefault)
            {
                await UnDefaultSignatures(entity.Id);
            }

            return input;
        }

        public IQueryable<GetSignatureUserDto> QueryAllSignatureUser()
        {
            return WorkScope.GetAll<SignatureUser>()
                .OrderByDescending(x => x.CreationTime)
                .Select(x => new GetSignatureUserDto
                {
                    Id = x.Id,
                    SignatureType = x.SignatureType,
                    UserId = x.UserId,
                    File = x.File,
                    FileBase64 = x.FileBase64,
                    IsDefault = x.IsDefault,
                    CreationTime = x.CreationTime,
                    LastModificationTime = (DateTime)x.LastModificationTime,
                    LastModifierUser = x.LastModifierUser.FullName
                });
        }

        public async Task<long> Delete(long id)
        {
            await WorkScope.DeleteAsync<SignatureUser>(id);
            return id;
        }

        public async Task SetDefaultSignature(SetDefaultSignatureDto input)
        {
            var loginUserId = AbpSession.UserId;

            var signatureUsers = await WorkScope.GetAll<SignatureUser>()
                .Where(x => x.UserId == loginUserId)
                .ToListAsync();

            var activeSignature = signatureUsers.Where(x => x.Id == input.Id).FirstOrDefault();

            activeSignature.IsDefault = input.IsDefault;

            foreach (var signature in signatureUsers)
            {
                if (signature.Id != input.Id)
                {
                    signature.IsDefault = false;
                }
            }
            await CurrentUnitOfWork.SaveChangesAsync();
        }
    }
}