using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AgriEnergyConnect.API.Models
{
    public class FarmerModel
    {
        [Key]
        [Column("id")] // Matches your database column name
        public string Id { get; set; }

        public string Name { get; set; }
        public string FarmName { get; set; }
        public string Location { get; set; }
        public string ContactEmail { get; set; }
        public string PhoneNumber { get; set; }
        public string Specialization { get; set; }
        public ICollection<ProductModel> Products { get; set; }
    }
}
