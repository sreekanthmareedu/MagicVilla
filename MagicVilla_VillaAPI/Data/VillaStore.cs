using MagicVilla_VillaAPI.Models.DTO;

namespace MagicVilla_VillaAPI.Data
{
    public class VillaStore

    {
         public static List<VillaDTO> villaList =  new List<VillaDTO>()
            {
                new VillaDTO { Id = 1,Name = "Beach view" ,Sqft = 100, Occupancy = 2},
                new VillaDTO { Id = 2,Name = "Port view",Sqft = 400, Occupancy = 8 }
            };
    }
}
