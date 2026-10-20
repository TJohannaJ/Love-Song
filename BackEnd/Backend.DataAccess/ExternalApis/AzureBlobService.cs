using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Backend.Domain.DTOs;
using Backend.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Backend.Infrastructure.Services
{
    using Azure.Storage.Blobs;
    using Azure.Storage.Sas;
    using Azure.Storage;

    public class AzureBlobService : IBlobService
    {
        private readonly string _connectionString;
        private readonly string _containerName = "images";

        public AzureBlobService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AzureBlobStorage")!;
        }

        public SasUploadResponseDTO GenerateUploadSasUrl(string fileName, string contentType)
        {
            // 1. Biztonsági ellenőrzés: Csak képfájlok engedélyezése!
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
            if (!allowedTypes.Contains(contentType.ToLower()))
                throw new ArgumentException("Csak JPG, PNG és WEBP képek tölthetők fel!");

            // 2. Egyedi fájlnév generálása
            string extension = Path.GetExtension(fileName);
            string uniqueBlobName = $"{Guid.NewGuid()}{extension}";

            var blobClient = new BlobClient(_connectionString, _containerName, uniqueBlobName);

            // 3. SAS Token paraméterek (Csak ÍRÁSI jog, max 3 percre!)
            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = _containerName,
                BlobName = uniqueBlobName,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5), // Óraeltérés kiküszöbölése
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(10)
            };

// Megadjuk a Read (Olvasás), Write (Írás) és Create (Létrehozás) jogokat is:
            sasBuilder.SetPermissions(BlobSasPermissions.Read | BlobSasPermissions.Write | BlobSasPermissions.Create);
            
            // 4. SAS URL előállítása
            Uri sasUri = blobClient.GenerateSasUri(sasBuilder);

            return new SasUploadResponseDTO
            {
                UploadUrl = sasUri.ToString(),
                BlobUrl = blobClient.Uri.ToString() // Ezt mentjük el az adatbázisba
            };
        }
    }
}