using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebThuCungNew.Models
{
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }

        public int? ClientId { get; set; } // Nullable for guest bookings
        [ForeignKey("ClientId")]
        public User? Client { get; set; }

        // Guest booking information (nullable)
        public string? GuestName { get; set; }
        public string? GuestEmail { get; set; }
        public string? GuestPhone { get; set; }

        public int? SitterId { get; set; }
        [ForeignKey("SitterId")]
        public User? Sitter { get; set; }

        public int ServiceId { get; set; }
        [ForeignKey("ServiceId")]
        public Service Service { get; set; }

        // Pet information (can be entered directly for guests)
        public int? PetId { get; set; }
        [ForeignKey("PetId")]
        public Pet? Pet { get; set; }

        public string? PetName { get; set; }
        public string? PetType { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }

        public string Location { get; set; }

        public string Status { get; set; } = "Pending"; // Pending, Confirmed, Completed, Cancelled

        public string PaymentMethod { get; set; } // COD, BankTransfer

        public decimal TotalPrice { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
