using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BackendService.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        [Required]
        public string UserType { get; set; } = string.Empty;

        public Donor? Donor { get; set; }
        public Recipient? Recipient { get; set; }
        public Admin? Admin { get; set; }
    }
}