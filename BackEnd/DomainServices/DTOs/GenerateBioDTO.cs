// Backend.Domain.DTOs/GenerateBioDTO.cs
namespace Backend.Domain.DTOs
{
    public class GenerateBioDTO
    {
        public List<string> Keywords { get; set; } = new();
        public string Tone { get; set; } = "playful"; // pl. playful, flirty, witty, mysterious
        public string TargetField { get; set; } = "short"; // "short" -> ShortIntro, "long" -> LongIntro
        public string? ExtraContext { get; set; }
    }
}