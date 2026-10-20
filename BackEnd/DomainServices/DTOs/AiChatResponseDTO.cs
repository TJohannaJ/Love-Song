namespace Backend.Domain.DTOs;

public class AiChatResponseDTO
{
    public string Reply { get; set; } = string.Empty;
    public List<ProfileDTO> Profiles { get; set; } = new();
}