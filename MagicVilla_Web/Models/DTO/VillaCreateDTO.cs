﻿using System.ComponentModel.DataAnnotations;

namespace MagicVilla_Web.Models.DTO
{
    public class VillaCreateDTO
    {
        
        [Required]
        [MaxLength(30)]
        public string Name{ get; set; }
        public int Occupancy { get; set; }  

        public int Sqft { get; set; }

        public string ImageURL { get; set; }
        public string Amenity { get; set; }

        public string Details { get; set; }
        [Required]
        public string Rate { get; set; }
    }
}
