namespace MarcusRent.Classes
{
    public class Car
    {
        public int CarId { get; set; }

        public List<CarImage> Images { get; set; } = new();

        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public float PricePerDay { get; set; }
        public bool Available { get; set; }

        public List<Order> Orders { get; set; } = new();
    }
}
