using Abp.Zero.EntityFrameworkCore;
using EC.Authorization.Roles;
using EC.Authorization.Users;
using EC.Entities;
using EC.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace EC.EntityFrameworkCore
{
    public class ECDbContext : AbpZeroDbContext<Tenant, Role, User, ECDbContext>
    {
        /* Define a DbSet for each entity of the application */
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ContractHistory> ContractHistories { get; set; }
        public DbSet<ContractSetting> ContractSettings { get; set; }
        public DbSet<SignerSignatureSetting> SignerSignatureSettings { get; set; }
        public DbSet<ContractSigning> ContractSigning { get; set; }
        public DbSet<SignatureUser> SignatureUsers { get; set; }
        public DbSet<ContractBase64Image> ContractBase64Images { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<ContractTemplate> ContractTemplates { get; set; }
        public DbSet<ContractTemplateSetting> ContractTemplateSettings { get; set; }
        public DbSet<ContractTemplateSigner> ContractTemplateSigners { get; set; }
        public DbSet<MassContractTemplateSigner> MassContractTemplateSigners { get; set; }
        public ECDbContext(DbContextOptions<ECDbContext> options)
            : base(options)
        {
        }
    }
}