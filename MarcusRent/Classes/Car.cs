using System.ComponentModel.DataAnnotations;
using MarcusRent.Classes;

namespace MarcusRent.Classes
{
    public class Car
    {
        public int CarId { get; set; }

        public List<CarImage> CarImages { get; set; } = new();
       
        public string Brand { get; set; }
        
        public string Model { get; set; }
        [Range(1900, 2100)]
        public int Year { get; set; }
        [Range(0, double.MaxValue)]
        public decimal PricePerDay { get; set; }
        public bool Available { get; set; }
    }
}

