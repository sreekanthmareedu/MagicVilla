using System.ComponentModel.DataAnnotations;

namespace MagicVilla_VillaAPI.Models.DTO
{
    public class VillaUpdateDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string Name{ get; set; }
        [Required]
        public int Occupancy { get; set; }  

        public int Sqft { get; set; }
        [Required]
        public string ImageURL { get; set; }
        public string Amenity { get; set; }

        public string Details { get; set; }
        [Required]
        public string Rate { get; set; }
    }
}
