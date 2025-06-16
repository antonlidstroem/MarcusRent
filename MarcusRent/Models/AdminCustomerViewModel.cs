namespace MarcusRent.Models
{
    public class AdminCustomerViewModel
    {
        public string UserId { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Email { get; set; } = "";
        public bool ApprovedByAdmin { get; set; }
    }
}