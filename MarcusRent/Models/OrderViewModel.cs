using Microsoft.AspNetCore.Mvc.Rendering;

namespace MarcusRent.Models
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Price { get; set; }
        public bool ActiveOrder { get; set; }

        public int CarId { get; set; }      // Vald bil
        public string UserId { get; set; }  // Vald användare

        public IEnumerable<SelectListItem> CarOptions { get; set; }
        public IEnumerable<SelectListItem> UserOptions { get; set; }
    }
}