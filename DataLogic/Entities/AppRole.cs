using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace DataAccess.Entities
{
    public class AppRole : IdentityRole<Guid>
    {
        public AppRole()
        {
        }

        public virtual ICollection<AppUserRole> UserRoles { get; set; }

        public virtual ICollection<AppRoleClaim> RoleClaims { get; set; }
    }
}
