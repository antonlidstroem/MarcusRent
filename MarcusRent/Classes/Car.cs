using MarcusRent.Classes;

namespace MarcusRent.Classes
{
    public class Car
    {
        public int CarId { get; set; }

        public List<CarImage> CarImages { get; set; } = new();

        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public decimal PricePerDay { get; set; }
        public bool Available { get; set; }

        //public ICollection<CarOrder> CarOrders { get; set; } = new List<CarOrder>();

        public ICollection<CarOrder> CarOrders { get; set; }

    }
}

