using System;
using Microsoft.AspNetCore.Identity;

namespace DataAccess.Entities
{
    public class AppUserRole : IdentityUserRole<Guid>
    {
        public AppUserRole()
        {
        }

        public virtual AppUser User { get; set; }

        public virtual AppRole Role { get; set; }
    }
}
