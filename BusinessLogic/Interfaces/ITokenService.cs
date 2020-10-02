using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLogic.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateToken(
            Guid userId,
            string userName,
            string email,
            List<string> roles);
    }
}
