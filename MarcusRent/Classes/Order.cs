using MarcusRent.Classes;

namespace MarcusRent.Classes
{

    public class Order
    {
        public int OrderId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Price { get; set; }
        public bool ActiveOrder { get; set; }

        public string UserId { get; set; }               // foreign key
        public ApplicationUser Customer { get; set; }    // navigation property
        public ICollection<CarOrder> CarOrders { get; set; }

    }
}






