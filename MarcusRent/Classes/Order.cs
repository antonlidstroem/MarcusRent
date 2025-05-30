using MarcusRent.Classes;

namespace MarcusRent.Classes
{
    public class Order
    {
        public int OrderId { get; set; }
        public ApplicationUser Customer { get; set; }
        public List<Car> Cars { get; set; } = new();

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
        public float Price { get; set; }

        public bool ActiveOrder { get; set; }
    }
}

