using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace RaceDay.MVC.Services
{
    public class BlobStorageService
    {
        private readonly string _connectionString;
        private readonly string _containerName;

        public BlobStorageService(IConfiguration configuration)
        {
            _connectionString = configuration["AzureBlobStorage:ConnectionString"]!;
            _containerName = configuration["AzureBlobStorage:ContainerName"]!;
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0) return string.Empty;

            // Connect to the Azure Blob container
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            // Create container if it doesn't exist, and set access to public so we can view the images
            await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            // Create a unique name for the file
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var blobClient = blobContainerClient.GetBlobClient(fileName);

            // Upload the file stream
            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });
            }

            // Return the URL of the uploaded image
            return blobClient.Uri.ToString();
        }
    }
}