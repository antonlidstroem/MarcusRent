using MarcusRent.Classes;
namespace MarcusRent.Classes { 
public class CarImage
{
    public int CarImageId { get; set; }
    public string Url { get; set; }

    public int CarId { get; set; }
    public Car Car { get; set; }
}
}


