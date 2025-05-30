using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public bool ApprovedByAdmin { get; set; }
}
