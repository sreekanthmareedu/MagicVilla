using MagicVilla_VillaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {

        }
        public DbSet<Villa> Villas { get; set; }

        public DbSet<User> Users { get; set;}

        public DbSet<VillaNumber> VillaNumbers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Villa>().HasData(
                new Villa()
                {
                    Id = 1,
                    Name = "Royal Villa",
                    Details = "explore living",
                    ImageURL = "",
                    Occupancy = 5,
                    Amenity = "",
                    Rate = "300$",
                    CreatedDate = DateTime.Now,
                    Sqft = 500
                    


                }

                );
            
        }
    }
}
