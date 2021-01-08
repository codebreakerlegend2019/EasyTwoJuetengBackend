using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EasyTwoJuetengBackend.Persistence.BlobStorageRepositories
{
    public class BlobStorageRepo : IBlobStorageRepo
    {
        #region
        private readonly IConfiguration _configuration;
        private readonly CloudStorageAccount _cloudStorage;

        #endregion
        public BlobStorageRepo(IConfiguration configuration)
        {
            this._configuration = configuration;
            var azureBlobConnectionString = _configuration.GetSection("AzureBlobStorage:EasyTwoBlobStorage").Value;
            _cloudStorage = CloudStorageAccount.Parse(azureBlobConnectionString);
        }

        public async Task UploadFile(IFormFile file,string desiredFileName)
        {
            var cloudBlobClient = _cloudStorage.CreateCloudBlobClient();
            var cloudBlobContainer = cloudBlobClient.GetContainerReference("easytwojuetengstorage");

            if (await cloudBlobContainer.CreateIfNotExistsAsync())
                await cloudBlobContainer.SetPermissionsAsync(new
                    BlobContainerPermissions
                { PublicAccess = BlobContainerPublicAccessType.Off });

            var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(desiredFileName);
            cloudBlockBlob.Properties.ContentType = file.ContentType;

            await cloudBlockBlob.UploadFromStreamAsync(file.OpenReadStream());
        }
        public async Task<FileContentResult> DownloadFile(string fileName)
        {
            var ms = new MemoryStream();
            var cloudBlobClient = _cloudStorage.CreateCloudBlobClient();
            var container = cloudBlobClient.GetContainerReference("easytwojuetengstorage");

            if (await container.ExistsAsync())
            {
                var file = container.GetBlobReference(fileName);

                if (await file.ExistsAsync())
                {
                    await file.DownloadToStreamAsync(ms);
                    
                    var blobStream = file.OpenReadAsync().Result;

                    var byteRead = new byte[blobStream.Length];
                    blobStream.Read(byteRead, 0, byteRead.Length);
                    var contentType = file.Properties.ContentType;
                    return new FileContentResult(byteRead, contentType);
                }
            }
            return null;
        }     
    }
}
