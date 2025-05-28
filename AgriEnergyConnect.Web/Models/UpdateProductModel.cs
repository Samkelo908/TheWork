using System.ComponentModel.DataAnnotations;

namespace AgriEnergyConnect.Web.Models
{
    public class UpdateProductModel
    {
        [Required]
        public int Id { get; set; }

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
}
