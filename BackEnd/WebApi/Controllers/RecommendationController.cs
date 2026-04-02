using Backend.Domain.Entities;
using Backend.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RecommendationController : Controller
    {
        private readonly RecommendationService recommendationService;


        public RecommendationController(RecommendationService recommendationService)
        {
            this.recommendationService = recommendationService;
        }

        [HttpGet("{id}", Name = "GetRecommendations")]
            public List<User> GetRecomenndationsForUser(string id) => recommendationService.GetRecommendationsForUser(id);              
    }
}
