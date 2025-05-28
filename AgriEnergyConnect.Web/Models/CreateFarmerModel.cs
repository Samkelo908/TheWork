﻿using System.ComponentModel.DataAnnotations;

namespace AgriEnergyConnect.Web.Models
{
    public class CreateFarmerModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public string Region { get; set; } = string.Empty;
    }
}
