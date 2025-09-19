using CarRentalSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace CarRentalSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserID);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Password).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Role).IsRequired().HasMaxLength(20);
                
                // Ensure username is unique
                entity.HasIndex(e => e.Username).IsUnique();
            });

            // Configure Car entity
            modelBuilder.Entity<Car>(entity =>
            {
                entity.HasKey(e => e.CarID);
                entity.Property(e => e.CarName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CarModel).IsRequired().HasMaxLength(50);
                entity.Property(e => e.ImageUrl).HasMaxLength(200);
                entity.Property(e => e.IsAvailable).HasDefaultValue(true);
            });

            // Configure Booking entity
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(e => e.BookingID);
                entity.Property(e => e.TotalCost).HasColumnType("decimal(18,2)");
                
                // Configure foreign key relationships
                entity.HasOne(e => e.Customer)
                      .WithMany(u => u.Bookings)
                      .HasForeignKey(e => e.CustomerID)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Car)
                      .WithMany(c => c.Bookings)
                      .HasForeignKey(e => e.CarID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Seed data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed admin user
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserID = 1,
                    Username = "admin",
                    Password = "admin123", // In production, this should be hashed
                    Role = "Admin"
                }
            );

            // Seed cars
            modelBuilder.Entity<Car>().HasData(
                new Car { CarID = 1, CarName = "Honda Civic", CarModel = "2024", ImageUrl = "/images/honda-civic.jpg", IsAvailable = true },
                new Car { CarID = 2, CarName = "Toyota Camry", CarModel = "2024", ImageUrl = "/images/toyota-camry.jpg", IsAvailable = true },
                new Car { CarID = 3, CarName = "Tesla Model 3", CarModel = "2024", ImageUrl = "/images/tesla-model3.jpg", IsAvailable = true },
                new Car { CarID = 4, CarName = "BMW 3 Series", CarModel = "2024", ImageUrl = "/images/bmw-3series.jpg", IsAvailable = true },
                new Car { CarID = 5, CarName = "Mercedes C-Class", CarModel = "2024", ImageUrl = "/images/mercedes-cclass.jpg", IsAvailable = true },
                new Car { CarID = 6, CarName = "Audi A4", CarModel = "2024", ImageUrl = "/images/audi-a4.jpg", IsAvailable = true },
                new Car { CarID = 7, CarName = "Ford Mustang", CarModel = "2024", ImageUrl = "/images/ford-mustang.jpg", IsAvailable = true },
                new Car { CarID = 8, CarName = "Chevrolet Camaro", CarModel = "2024", ImageUrl = "/images/chevrolet-camaro.jpg", IsAvailable = true },
                new Car { CarID = 9, CarName = "Nissan Altima", CarModel = "2024", ImageUrl = "/images/nissan-altima.jpg", IsAvailable = true },
                new Car { CarID = 10, CarName = "Hyundai Sonata", CarModel = "2024", ImageUrl = "/images/hyundai-sonata.jpg", IsAvailable = true },
                new Car { CarID = 11, CarName = "Kia Optima", CarModel = "2024", ImageUrl = "/images/kia-optima.jpg", IsAvailable = true },
                new Car { CarID = 12, CarName = "Mazda 6", CarModel = "2024", ImageUrl = "/images/mazda-6.jpg", IsAvailable = true },
                new Car { CarID = 13, CarName = "Subaru Legacy", CarModel = "2024", ImageUrl = "/images/subaru-legacy.jpg", IsAvailable = true },
                new Car { CarID = 14, CarName = "Volkswagen Jetta", CarModel = "2024", ImageUrl = "/images/volkswagen-jetta.jpg", IsAvailable = true },
                new Car { CarID = 15, CarName = "Lexus ES", CarModel = "2024", ImageUrl = "/images/lexus-es.jpg", IsAvailable = true }
            );
        }
    }
}