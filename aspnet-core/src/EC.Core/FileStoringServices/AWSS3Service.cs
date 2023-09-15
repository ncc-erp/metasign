using Abp.Application.Services;
using Abp.Dependency;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using EC.Configuration;
using EC.Constants.FileStoring;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using static EC.Constants.Enum;

namespace EC.FileStorageServices
{
    public class AWSS3Service : ApplicationService, IFileStoringService, ITransientDependency
    {
        private IAmazonS3 s3client;

        public AWSS3Service()
        { }

        public AWSS3Dto ResetCredential()
        {
            var tenantId = AbpSession.TenantId;
            string accessKeyId = "";
            string secretKey = "";
            string region = "";
            string bucketName = "";
            string prefix = "";
            if (tenantId == null)
            {
                accessKeyId = SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.AWSAccessKeyId).Result;
                secretKey = SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.AWSSecretKey).Result;
                region = SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.AWSRegion).Result;
                bucketName = SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.AWSBucketName).Result;
                prefix = SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.AWSPrefix).Result;
            }
            else
            {
                accessKeyId = SettingManager.GetSettingValueForTenantAsync(AppSettingNames.AWSAccessKeyId, tenantId.Value).Result;
                secretKey = SettingManager.GetSettingValueForTenantAsync(AppSettingNames.AWSSecretKey, tenantId.Value).Result;
                region = SettingManager.GetSettingValueForTenantAsync(AppSettingNames.AWSRegion, tenantId.Value).Result;
                bucketName = SettingManager.GetSettingValueForTenantAsync(AppSettingNames.AWSBucketName, tenantId.Value).Result;
                prefix = SettingManager.GetSettingValueForTenantAsync(AppSettingNames.AWSPrefix, tenantId.Value).Result;
            }
            BasicAWSCredentials credentials = new BasicAWSCredentials(accessKeyId, secretKey);
            s3client = new AmazonS3Client(credentials, RegionEndpoint.GetBySystemName(region));
            return new AWSS3Dto
            {
                AccessKeyId = accessKeyId,
                SecretKey = secretKey,
                Region = region,
                BucketName = bucketName,
                Prefix = prefix
            };
        }

        private async Task UploadFileToS3(IFormFile file, string key)
        {
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var s3config = ResetCredential();
                var request = new PutObjectRequest
                {
                    BucketName = s3config.BucketName,
                    Key = key,
                    InputStream = ms
                };
                // request.Metadata.Add("Content-Type", file.ContentType);
                await s3client.PutObjectAsync(request);
            }
        }

        private async Task<byte[]> DownloadFileFromS3(string key)
        {
            var s3config = ResetCredential();
            var request = new GetObjectRequest
            {
                BucketName = s3config.BucketName,
                Key = key
            };
            var getObjectResponse = await s3client.GetObjectAsync(request);
            var memoryStream = new MemoryStream();
            await getObjectResponse.ResponseStream.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }

        private async Task<List<byte[]>> DownloadMultipleFilesFromS3(string prefix)
        {
            List<byte[]> fileList = new List<byte[]>();
            var s3config = ResetCredential();
            var request = new ListObjectsV2Request
            {
                BucketName = s3config.BucketName,
                Prefix = prefix
            };
            var response = s3client.ListObjectsV2Async(request);
            foreach (var s3Object in response.Result.S3Objects)
            {
                byte[] file = await DownloadFileFromS3(s3Object.Key);
                fileList.Add(file);
            }
            return fileList;
        }

        private async Task DeleteFileFromS3(string key)
        {
            var s3config= ResetCredential();
            var request = new DeleteObjectRequest
            {
                BucketName = s3config.BucketName,
                Key = key
            };
            await s3client.DeleteObjectAsync(request);
        }

        private async Task DeleteMultipleFilesFromS3(string prefix)
        {
            List<string> fileList = await SearchForFilesByPrefix(prefix);
            foreach (string key in fileList)
            {
                await DeleteFileFromS3(key);
            }
        }

        private async Task<string> GetPreSignedUrl(string key)
        {
            string desiredFileName = await GetFileNameFromKey(key);
            string desiredFileNameEncoded = Uri.EscapeDataString(desiredFileName);
            var s3config= ResetCredential();
            var request = new GetPreSignedUrlRequest
            {
                BucketName = s3config.BucketName,
                Key = key,
                Expires = DateTime.UtcNow.AddMinutes(10),
                ResponseHeaderOverrides = new ResponseHeaderOverrides
                {
                    ContentDisposition = $"attachment; filename=\"{desiredFileNameEncoded}\""
                }
            };
            return s3client.GetPreSignedURL(request);
        }

        private async Task<List<string>> SearchForFilesByPrefix(string prefix)
        {
            List<byte[]> fileList = new List<byte[]>();
            var s3config= ResetCredential();
            var request = new ListObjectsV2Request
            {
                BucketName = s3config.BucketName,
                Prefix = prefix
            };
            var response = s3client.ListObjectsV2Async(request);
            List<string> result = new List<string>();
            foreach (var s3Object in response.Result.S3Objects)
            {
                result.Add(s3Object.Key);
            }
            return result;
        }

        public async Task UploadFile(IFormFile file, string tenantName, FileCategory fileCategory, string guid, int? index)
        {
            string key = await MakeKey(tenantName, fileCategory, guid, index, file.FileName);
            await UploadFileToS3(file, key);
        }

        public async Task<byte[]> DownloadFile(string tenantName, FileCategory fileCategory, string guid, int? index, string fileName)
        {
            string key = await MakeKey(tenantName, fileCategory, guid, index, fileName);
            return await DownloadFileFromS3(key);
        }

        public async Task<List<byte[]>> DownloadMultipleFiles(string tenantName, FileCategory fileCategory, string guid)
        {
            string prefix = await MakeKey(tenantName, fileCategory, guid, null, null);
            return await DownloadMultipleFilesFromS3(prefix);
        }

        public async Task DeleteFile(string tenantName, FileCategory fileCategory, string guid, int? index, string fileName)
        {
            string key = await MakeKey(tenantName, fileCategory, guid, index, fileName);
            await DeleteFileFromS3(key);
        }

        public async Task DeleteMultipleFiles(string tenantName, FileCategory fileCategory, string guid)
        {
            string prefix = await MakeKey(tenantName, fileCategory, guid, null, null);
            await DeleteMultipleFilesFromS3(prefix);
        }

        public async Task<string> GetDirectDownloadUrl(string tenantName, FileCategory fileCategory, string guid, int? index, string fileName)
        {
            string key = await MakeKey(tenantName, fileCategory, guid, index, fileName);
            return await GetPreSignedUrl(key);
        }

        public async Task<List<string>> SearchForFiles(string tenantName, FileCategory fileCategory, string guid, int? index, string fileName)
        {
            string prefix = await MakeKey(tenantName, fileCategory, guid, index, fileName);
            return await SearchForFilesByPrefix(prefix);
        }

        private async Task<string> MakeKey(string tenantName, FileCategory fileCategory, string guid, int? index, string? fileName)
        {
            var s3config= ResetCredential();
            string key = s3config.Prefix.TrimEnd('/') + '/' + tenantName;

            switch (fileCategory)
            {
                case FileCategory.Attachment:
                    key += '/' + FileStoringConstants.AttachmentFolder.TrimEnd('/');
                    break;

                case FileCategory.UnsignedContract:
                    key += '/' + FileStoringConstants.ContractFolder.TrimEnd('/');
                    key += '/' + FileStoringConstants.UnsignedFolder.TrimEnd('/');
                    break;

                case FileCategory.SignedContract:
                    key += '/' + FileStoringConstants.ContractFolder.TrimEnd('/');
                    key += '/' + FileStoringConstants.SignedFolder.TrimEnd('/');
                    break;

                case FileCategory.Signature:
                    key += '/' + FileStoringConstants.SignatureFolder.TrimEnd('/');
                    break;

                case FileCategory.Download:
                    key += '/' + FileStoringConstants.DownloadFolder.TrimEnd('/');
                    break;
            }

            key += '/' + guid;
            if (index != null)
            {
                key += '_' + index.ToString();
            }
            if (fileName != null)
            {
                key += "_" + fileName;
            }
            return key;
        }

        private async Task<string> GetFileNameFromKey(string key)
        {
            string s3FileName = key.Substring(key.LastIndexOf('/') + 1);
            string fileName = s3FileName.Substring(s3FileName.IndexOf('_', s3FileName.IndexOf('_') + 1) + 1);
            return fileName;
        }
    }
}