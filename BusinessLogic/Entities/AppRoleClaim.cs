using System;
using Microsoft.AspNetCore.Identity;

namespace BusinessLogic.Entities
{
    public class AppRoleClaim : IdentityRoleClaim<Guid>
    {
        public AppRoleClaim()
        {
        }

        public virtual AppRole Role { get; set; }
    }
}
