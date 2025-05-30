using MarcusRent.Classes;

namespace MarcusRent.Classes
{
    public class CarOrder
    {
        public int CarId { get; set; }
        public Car Car { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int RentalDays { get; set; }
        public float Price { get; set; }
    }
}