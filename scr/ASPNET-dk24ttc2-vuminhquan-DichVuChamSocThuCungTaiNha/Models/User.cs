using System.ComponentModel.DataAnnotations;

namespace WebThuCungNew.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }

        [Required]
        public string Role { get; set; } = "Client"; // Client, Sitter, Admin

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Pet>? Pets { get; set; }
        public ICollection<Booking>? ClientBookings { get; set; }
        public ICollection<Booking>? SitterBookings { get; set; }
    }
}
