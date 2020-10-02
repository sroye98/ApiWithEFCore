using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace BusinessLogic.Services
{
    public class TokenService : ITokenService
    {
        //TODO: Change _key value
        private readonly string _key = "e5n0EbWLpBjaoMEeAs7puqU6oH6fxHiwmYQHudS1blRox1x5gzuH54Z2KT7ryBZCJ4Mt2exueJtb2836cofgSt4vF8fRvwe254dX";
        private readonly int _expirationDays = 30;
        private readonly string _issuer = "https://localhost";

        public TokenService()
        {
        }

        public Task<string> GenerateToken(
            Guid userId,
            string userName,
            string email,
            List<string> roles)
        {
            try
            {
                List<Claim> claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Jti, userId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, email),
                    new Claim(JwtRegisteredClaimNames.NameId, userId.ToString()),
                    new Claim(JwtRegisteredClaimNames.UniqueName, userName),
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim(ClaimTypes.Name, userName),
                    new Claim(ClaimTypes.Email, email)
                };

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
                    expires: DateTime.Now.AddDays(_expirationDays),
                    signingCredentials: credentials);
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                string token = handler.WriteToken(jwtToken);

                return Task.FromResult(token);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
