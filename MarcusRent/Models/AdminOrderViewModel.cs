namespace MarcusRent.Models
{
    public class AdminOrderViewModel
    {
        public int OrderId { get; set; }
        public string CarName { get; set; } = "";
        public string CustomerName { get; set; } = "";
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Price { get; set; }
        public bool ActiveOrder { get; set; }
    }
}