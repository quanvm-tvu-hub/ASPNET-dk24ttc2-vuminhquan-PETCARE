using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebThuCungNew.Models
{
    public class Pet
    {
        [Key]
        public int PetId { get; set; }

        public int OwnerId { get; set; }
        [ForeignKey("OwnerId")]
        public User? Owner { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Type { get; set; } // Dog, Cat

        public string? Breed { get; set; }
        public string? Notes { get; set; }
    }
}
