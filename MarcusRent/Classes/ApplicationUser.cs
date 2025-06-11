using Microsoft.AspNetCore.Identity;

namespace MarcusRent.Classes
{
    public class ApplicationUser : IdentityUser
    {
        public bool ApprovedByAdmin { get; set; }
    }
}


