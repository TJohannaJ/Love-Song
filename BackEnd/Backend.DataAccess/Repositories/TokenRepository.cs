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

            await _userManager.SetAuthenticationTokenAsync(user, "Spotify", "AccessToken", accessToken);

            if (!string.IsNullOrEmpty(refreshToken))
            {
                await _userManager.SetAuthenticationTokenAsync(user, "Spotify", "RefreshToken", refreshToken);
            }
        }

        public async Task<string> GetAccessTokenAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return await _userManager.GetAuthenticationTokenAsync(user, "Spotify", "AccessToken");
        }

        public async Task<string> GetRefreshTokenAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return await _userManager.GetAuthenticationTokenAsync(user, "Spotify", "RefreshToken");
        }
    }
}
