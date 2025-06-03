using MarcusRent.Classes;

namespace MarcusRent.Models
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }

        public ApplicationUser Customer { get; set; }
        public ICollection<CarOrder> CarOrders { get; set; } = new List<CarOrder>();

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
        public float Price { get; set; }

        public bool ActiveOrder { get; set; }

    }
}


