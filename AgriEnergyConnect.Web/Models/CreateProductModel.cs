using System.ComponentModel.DataAnnotations;

namespace AgriEnergyConnect.Web.Models
{
    public class CreateProductModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime HarvestDate { get; set; }
    }
}
