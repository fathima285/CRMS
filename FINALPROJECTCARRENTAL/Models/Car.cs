using System.ComponentModel.DataAnnotations;

namespace CarRentalSystem.Models
{
    public class Car
    {
        public int CarID { get; set; }

        [Required(ErrorMessage = "Car name is required")]
        [StringLength(100, ErrorMessage = "Car name cannot exceed 100 characters")]
        public string CarName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Car model is required")]
        [StringLength(50, ErrorMessage = "Car model cannot exceed 50 characters")]
        public string CarModel { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Image URL cannot exceed 200 characters")]
        public string ImageUrl { get; set; } = string.Empty;

        public bool IsAvailable { get; set; } = true;

        // Navigation properties
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}