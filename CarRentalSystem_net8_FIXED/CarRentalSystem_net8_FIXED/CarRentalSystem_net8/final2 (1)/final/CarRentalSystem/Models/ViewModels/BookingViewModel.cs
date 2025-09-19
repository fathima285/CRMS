using System.ComponentModel.DataAnnotations;

namespace CarRentalSystem.Models.ViewModels
{
    public class BookingViewModel
    {
        public int CarID { get; set; }
        public string CarName { get; set; } = string.Empty;
        public string CarModel { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "Pickup date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Pickup Date")]
        public DateTime PickupDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Return date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Return Date")]
        public DateTime ReturnDate { get; set; } = DateTime.Today.AddDays(1);

        [Display(Name = "Total Cost")]
        public decimal TotalCost { get; set; }

        [Display(Name = "Number of Days")]
        public int NumberOfDays { get; set; }

        public const decimal DailyRate = 50.00m;
    }
}