using System;
using Microsoft.AspNetCore.Identity;

namespace DataAccess.Entities
{
    public class AppRoleClaim : IdentityRoleClaim<Guid>
    {
        public AppRoleClaim()
        {
        }

        public virtual AppRole Role { get; set; }
    }
}
