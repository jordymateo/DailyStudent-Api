using DailyStudent.Api.Services.Security.UserContext;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace DailyStudent.Api.Services.Cloud
{
    public class GoogleCloudService: IGoogleCloudService
    {
        private readonly IUserContext _userContext;
        private readonly GoogleCloudOptions _cloudOptions;
        private readonly GoogleCredential _cloudCredentials;

        private const string ProfileImageBucket = "profile-ds";

        public GoogleCloudService(
            IUserContext userContext,
            IOptions<GoogleCloudOptions> options
            )
        {

            _userContext = userContext;
            _cloudOptions = options.Value;
            _cloudCredentials = GoogleCredential.FromJson(JsonSerializer.Serialize(_cloudOptions));
        }
        public async Task<string> SaveProfileImage(IFormFile image) 
            => await SaveProfileImage(_userContext.User.Id, image);

        public async Task<string> SaveProfileImage(int userId, IFormFile image)
        {

            var options = new UploadObjectOptions
            {
                PredefinedAcl = PredefinedObjectAcl.PublicRead
            };
            using (StorageClient storageClient = StorageClient.Create(_cloudCredentials))
            {
                var imageSaved = await storageClient.UploadObjectAsync(ProfileImageBucket, $"{userId}-profile-avatar{Path.GetExtension(image.FileName)}", image.ContentType, image.OpenReadStream(), options);
                return imageSaved.MediaLink;
            }
        }

        public async Task<string> SaveAttachment(IFormFile file)
        {

            var options = new UploadObjectOptions
            {
                PredefinedAcl = PredefinedObjectAcl.PublicRead,
            };
            using (StorageClient storageClient = StorageClient.Create(_cloudCredentials))
            {
                var fileSaved = await storageClient.UploadObjectAsync(ProfileImageBucket, file.FileName, file.ContentType, file.OpenReadStream(), options);
                return GetURL(fileSaved.Bucket, fileSaved.Name);
            }
        }

        public async Task<string> SaveAttachment(IFormFile file, string fileName)
        {

            var options = new UploadObjectOptions
            {
                PredefinedAcl = PredefinedObjectAcl.PublicRead
            };
            using (StorageClient storageClient = StorageClient.Create(_cloudCredentials))
            {
                var comp = file.FileName.Split('.');
                fileName += "." + comp[1];
                var fileSaved = await storageClient.UploadObjectAsync(ProfileImageBucket, file.FileName, file.ContentType, file.OpenReadStream(), options);
                return GetURL(fileSaved.Bucket, fileSaved.Name);
            }
        }

        private string GetURL(string bucket, string obj)
        {
            return "https://storage.googleapis.com/" + bucket + "/" + obj;
        }
    }
}
