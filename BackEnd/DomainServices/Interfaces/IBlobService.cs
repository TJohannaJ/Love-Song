using Backend.Domain.DTOs;

namespace Backend.Domain.Interfaces
{
    public interface IBlobService
    {
        SasUploadResponseDTO GenerateUploadSasUrl(string fileName, string contentType);
    }
}