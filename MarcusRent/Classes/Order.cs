using MarcusRent.Classes;

namespace MarcusRent.Classes
{

    public class Order
    {
        public int OrderId { get; set; }
        public int CarId { get; set; }
        public Car Car { get; set; }
        public string UserId { get; set; }
        public ApplicationUser Customer { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public decimal Price { get; set; }
        //public bool ActiveOrder { get; set; }
    }
}