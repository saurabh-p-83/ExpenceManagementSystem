using Application.Interface;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Infrastructure.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly BlobContainerClient _containerClient;

        public FileStorageService(IOptions<AzureBlobSettings> settings)
        {
            var blobSettings = settings.Value ?? throw new ArgumentNullException(nameof(settings.Value));

            var blobServiceClient = new BlobServiceClient(blobSettings.ConnectionString);
            _containerClient = blobServiceClient.GetBlobContainerClient(blobSettings.ContainerName);

            _containerClient.CreateIfNotExists(PublicAccessType.None);
        }

        public async Task<string> UploadFileOnBlob(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File cannot be null or empty.", nameof(file));

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var blobClient = _containerClient.GetBlobClient(fileName);

            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, overwrite: true);

            return blobClient.Uri.ToString();
        }
    }
}