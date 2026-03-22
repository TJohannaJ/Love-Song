using Backend.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using static Backend.Application.Recommendations.GetRecommendationsQuery;

namespace Backend.Application.Recommendations
{
    public record RecommendationDto(int Id, string Name, string Email);
    public record GetRecommendationsQuery() : IRequest<IEnumerable<RecommendationDto>>;
    public class GetRecommendationsHandler : IRequestHandler<GetRecommendationsQuery, IEnumerable<RecommendationDto>>
    {
        private readonly IRecommendationRepository _repo;
        public GetRecommendationsHandler(IRecommendationRepository repo) => _repo = repo;
        public Task<IEnumerable<RecommendationDto>> Handle(GetRecommendationsQuery request, CancellationToken ct)
        {
            var users = await _repo.GetRecommendations();
            return users.Select(u => new UserDto(u.Id, u.FullName, u.Email));
        }



        // DTO a frontendnek (hogy ne a nyers entitást küldjük ki)


        // Handler: Itt van az "ész"
        public class GetUsersHandler : IRequestHandler<GetUsersQuery, IEnumerable<UserDto>>
    {
        
        

        public async Task<IEnumerable<UserDto>> Handle(GetUsersQuery request, CancellationToken ct)
        {
            var users = await _repo.GetAllUsersAsync();
            return users.Select(u => new UserDto(u.Id, u.FullName, u.Email));
        }
    }
}
}
