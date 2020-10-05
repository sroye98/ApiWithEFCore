using System;
using Microsoft.AspNetCore.Identity;

namespace DataAccess.Entities
{
    public class AppUserToken : IdentityUserToken<Guid>
    {
        public AppUserToken()
        {
        }

        public virtual AppUser User { get; set; }
    }
}
