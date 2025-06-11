namespace MarcusRent.Models
{
    public class CarViewModel
    {
        public int CarId { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public decimal PricePerDay { get; set; }
        public List<string> ImageUrls { get; set; }
    }
}
