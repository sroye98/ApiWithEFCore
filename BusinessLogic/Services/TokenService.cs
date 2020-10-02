using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Entities;
using BusinessLogic.Interfaces;
using BusinessLogic.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace BusinessLogic.Services
{
    public class TokenService : ITokenService
    {
        //TODO: Change _key value
        private readonly string _key = "e5n0EbWLpBjaoMEeAs7puqU6oH6fxHiwmYQHudS1blRox1x5gzuH54Z2KT7ryBZCJ4Mt2exueJtb2836cofgSt4vF8fRvwe254dX";
        private readonly int _expirationTime = 5;
        private readonly string _issuer = "https://localhost";
        private readonly UserManager<AppUser> _userManager;

        public TokenService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<TokenResponse> GenerateTokens(
            AppUser user,
            string ipAddress)
        {
            try
            {
                List<Claim> claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Email, user.Email)
                };

                IList<string> roles = await _userManager.GetRolesAsync(user);
                if (roles == null)
                {
                    roles = new List<string>();
                }

                foreach (string role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
                SigningCredentials credentials = new SigningCredentials(
                    securityKey,
                    SecurityAlgorithms.HmacSha256Signature);
                JwtHeader header = new JwtHeader(credentials);
                JwtSecurityToken jwtToken = new JwtSecurityToken(
                    _issuer,
                    _issuer,
                    claims,
                    expires: DateTime.Now.AddMinutes(_expirationTime),
                    signingCredentials: credentials);
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

                string token = handler.WriteToken(jwtToken);
                RefreshToken refreshToken = generateRefreshToken(ipAddress);

                return new TokenResponse
                {
                    JwtToken = token,
                    RefreshToken = refreshToken.Token
                };
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private RefreshToken generateRefreshToken(string ipAddress)
        {
            using (var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomBytes),
                    Expires = DateTime.UtcNow.AddDays(7),
                    Created = DateTime.UtcNow,
                    CreatedByIp = ipAddress
                };
            }
        }
    }
}
