using Backend.Domain.DTOs;
using System.Threading.Tasks;

namespace Backend.Domain.Interfaces
{
	public interface IEssentiaService
	{
		Task<EssentiaFeaturesDTO> AnalyzeTrackAsync(string previewUrl);
	}
}