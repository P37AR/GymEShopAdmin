using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymEShopAdminApplication.Models
{
    public class EShopApplicationUser
    {
        //
        // Summary:
        //     Gets or sets the normalized email address for this user.
        public string NormalizedEmail { get; set; }
        //
        // Summary:
        //     Gets or sets the email address for this user.
        [ProtectedPersonalData]
        public string Email { get; set; }
        //
        // Summary:
        //     Gets or sets the normalized user name for this user.
        public string NormalizedUserName { get; set; }
        //
        // Summary:
        //     Gets or sets the user name for this user.
        [ProtectedPersonalData]
        public string UserName { get; set; }
    }
}
