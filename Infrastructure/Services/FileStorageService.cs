using Application.Interface;
using Azure.Storage.Blobs;
using Infrastructure.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly AzureBlobSettings _settings;

        public FileStorageService(IOptions<AzureBlobSettings> settings)
        {
            _settings = settings.Value;
        }

        public async Task<string> UploadFileOnBlob(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");

            var blobServiceClient = new BlobServiceClient(_settings.ConnectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(_settings.ContainerName);

            await containerClient.CreateIfNotExistsAsync();

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var blobClient = containerClient.GetBlobClient(fileName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            return blobClient.Uri.ToString();
        }
    }
}