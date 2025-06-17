using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MarcusRent.Models
{
    public class CarViewModel
    {
        public int CarId { get; set; }     
        public string Brand { get; set; }
        
        public string Model { get; set; }

        public int Year { get; set; }
        public bool Available { get; set; } = true;

        [Range(0, double.MaxValue)]
        public decimal PricePerDay { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();

        public decimal TotalEarnings { get; set; }
        public bool IsCurrentlyRented => !Available;
        public DateTime? CurrentRentalEndDate { get; set; }
        public string? CurrentCustomerName { get; set; }
    }
}
