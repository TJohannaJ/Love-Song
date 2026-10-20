using Backend.Domain.DTOs;
using System.Net.Http.Json;
using Backend.Domain.Interfaces;

namespace Backend.Infrastructure.ExternalApis
{
    public class EssentiaService : IEssentiaService
    {
        private readonly HttpClient _httpClient;

        public EssentiaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<EssentiaFeaturesDTO> AnalyzeTrackAsync(string previewUrl)
        {
            if (string.IsNullOrEmpty(previewUrl)) return new EssentiaFeaturesDTO();

            try
            {
                var response = await _httpClient.PostAsJsonAsync("http://localhost:5000/analyze", new { url = previewUrl });
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<EssentiaFeaturesDTO>() ?? new EssentiaFeaturesDTO();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Essentia hiba: {ex.Message}");
            }

            return new EssentiaFeaturesDTO();
        }
    }
}
