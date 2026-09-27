using System;
using System.Collections.Generic;
using System.Text;

namespace Backend.Domain.Interfaces
{
    public interface ITokenRepository
    {
        Task StoreTokensAsync(string userId, string accessToken, string refreshToken, int expiresIn);
        Task<string> GetAccessTokenAsync(string userId);
        Task<string> GetRefreshTokenAsync(string userId);
    }
}
