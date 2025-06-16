namespace MarcusRent.Models
{
    public class AdminDashboardViewModel
    {
        public List<AdminCarViewModel> Cars { get; set; } = new();
        public List<AdminOrderViewModel> Orders { get; set; } = new();
        public List<AdminCustomerViewModel> Customers { get; set; } = new();
    }
}