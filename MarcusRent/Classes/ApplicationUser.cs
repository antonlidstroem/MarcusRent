using Microsoft.AspNetCore.Identity;

namespace MarcusRent.Classes
{
    public class ApplicationUser : IdentityUser
    {
        public bool ApprovedByAdmin { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";
    }
}
