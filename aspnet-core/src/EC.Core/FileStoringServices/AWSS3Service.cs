using Abp.Application.Services;
using Abp.Dependency;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using EC.Configuration;
using EC.Constants.FileStoring;
using EC.Manager;
using HRMv2.NccCore;
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

        public AWSS3Service() { }

        public void ResetCredential ()
        {
            string accessKeyId = SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.AWSAccessKeyId).Result;
            string secretKey = SettingManager.GetSettingValueForApplicationAsync(AppSettingNames.AWSSecretKey).Result;
            BasicAWSCredentials credentials = new BasicAWSCredentials(accessKeyId, secretKey);
            s3client = new AmazonS3Client(credentials, RegionEndpoint.GetBySystemName(AWSS3Constants.Region));
        }

        private async Task UploadFileToS3(IFormFile file, string key)
        {

            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var request = new PutObjectRequest
                {
                    BucketName = AWSS3Constants.BucketName,
                    Key = key,
                    InputStream = ms
                };
                // request.Metadata.Add("Content-Type", file.ContentType);
                ResetCredential();
                await s3client.PutObjectAsync(request);
            }
        }

        private async Task<byte[]> DownloadFileFromS3(string key)
        {
            var request = new GetObjectRequest
            {
                BucketName = AWSS3Constants.BucketName,
                Key = key
            };
            ResetCredential();
            var getObjectResponse = await s3client.GetObjectAsync(request);
            var memoryStream = new MemoryStream();
            await getObjectResponse.ResponseStream.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }

        private async Task<List<byte[]>> DownloadMultipleFilesFromS3(string prefix)
        {
            List<byte[]> fileList = new List<byte[]>();
            var request = new ListObjectsV2Request
            {
                BucketName = AWSS3Constants.BucketName,
                Prefix = prefix
            };
            ResetCredential();
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
            var request = new DeleteObjectRequest
            {
                BucketName = AWSS3Constants.BucketName,
                Key = key
            };
            ResetCredential();
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
            var request = new GetPreSignedUrlRequest
            {
                BucketName = AWSS3Constants.BucketName,
                Key = key,
                Expires = DateTime.UtcNow.AddMinutes(10),
                ResponseHeaderOverrides = new ResponseHeaderOverrides
                {
                    ContentDisposition = $"attachment; filename=\"{desiredFileName}\""
                }
            };
            ResetCredential();
            return s3client.GetPreSignedURL(request);
        }

        private async Task<List<string>> SearchForFilesByPrefix(string prefix)
        {
            List<byte[]> fileList = new List<byte[]>();
            var request = new ListObjectsV2Request
            {
                BucketName = AWSS3Constants.BucketName,
                Prefix = prefix
            };
            ResetCredential();
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

        public async Task DeleteFile(string tenantName, FileCategory fileCategory, string guid , int? index, string fileName)
        {
            string key = await MakeKey(tenantName , fileCategory, guid, index, fileName);
            await DeleteFileFromS3(key);
        }

        public async Task DeleteMultipleFiles(string tenantName, FileCategory fileCategory, string guid)
        {
            string prefix = await MakeKey(tenantName, fileCategory, guid , null, null);
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
            string key = AWSS3Constants.Prefix.TrimEnd('/') + '/' + tenantName;

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
