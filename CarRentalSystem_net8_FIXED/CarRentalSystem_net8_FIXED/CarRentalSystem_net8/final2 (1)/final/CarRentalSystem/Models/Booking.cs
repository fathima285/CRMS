using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarRentalSystem.Models
{
    public class Booking
    {
        public int BookingID { get; set; }

        [Required]
        public int CustomerID { get; set; }

        [Required]
        public int CarID { get; set; }

        [Required(ErrorMessage = "Pickup date is required")]
        [DataType(DataType.Date)]
        public DateTime PickupDate { get; set; }

        [Required(ErrorMessage = "Return date is required")]
        [DataType(DataType.Date)]
        public DateTime ReturnDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalCost { get; set; }

        // Navigation properties
        [ForeignKey("CustomerID")]
        public virtual User Customer { get; set; } = null!;

        [ForeignKey("CarID")]
        public virtual Car Car { get; set; } = null!;
    }
}