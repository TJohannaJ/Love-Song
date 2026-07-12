using Backend.Domain.Interfaces;
using Backend.Infrastructure.DBEntities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Infrastructure.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly UserManager<DBUser> _userManager;

        public TokenRepository(UserManager<DBUser> userManager) => _userManager = userManager;

        public async Task StoreTokensAsync(string userId, string accessToken, string refreshToken, int expiresIn)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return;

            await _userManager.SetAuthenticationTokenAsync(user, "Spotify", "access_token", accessToken);
            // ... többi mentés
        }
    }
}
