// Domain/Interfaces/IAiBioGenerator.cs
using Backend.Domain.DTOs;
using Backend.Domain.Entities;

namespace Backend.Domain.Interfaces
{
    public interface IAiBioGenerator
    {
        Task<GeneratedBioResultDTO> GenerateBothBiosAsync(User user, GenerateBioDTO dto, CancellationToken ct = default);
    }
}