namespace MarcusRent.Models
{
    public class AdminDashboardViewModel
    {
        public List<CarViewModel> Cars { get; set; } = new();
        public List<OrderViewModel> Orders { get; set; } = new();
        public List<CustomerViewModel> Customers { get; set; } = new();
    }
}