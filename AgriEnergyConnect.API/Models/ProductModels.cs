using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgriEnergyConnect.API.Models
{
    public class ProductModel
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        public decimal Price { get; set; }
        public int Quantity { get; set; }

        [Required]
        public string Category { get; set; } = string.Empty;

        public DateTime HarvestDate { get; set; }

        [ForeignKey("Farmer")]
        public string FarmerId { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }


        public FarmerModel? Farmer { get; set; } // Made nullable as it's a navigation property
    }

    public class CreateProductModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public string Category { get; set; } = string.Empty;

        [Required]
        public DateTime HarvestDate { get; set; }
    }

    public class UpdateProductModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        public string Category { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime HarvestDate { get; set; }
    }
}