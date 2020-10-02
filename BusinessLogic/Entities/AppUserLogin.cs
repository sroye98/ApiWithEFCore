using System;
using Microsoft.AspNetCore.Identity;

namespace BusinessLogic.Entities
{
    public class AppUserLogin : IdentityUserLogin<Guid>
    {
        public AppUserLogin()
        {
        }

        public virtual AppUser User { get; set; }
    }
}
