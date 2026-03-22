using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Domain.Interfaces
{
    public interface IRecommendationRepository
    {
        Task<IEnumerable<IRecommendationRepository>> GetRecommendations();
    }
}
