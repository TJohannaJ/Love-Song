namespace Backend.Domain.DTOs;

public class SasUploadResponseDTO
{
    public string UploadUrl { get; set; } = string.Empty; // Eredeti SAS URL a feltöltéshez
    public string BlobUrl { get; set; } = string.Empty;   // A végleges nyilvános kép URL
}