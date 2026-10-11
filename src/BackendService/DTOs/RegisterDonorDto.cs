using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BackendService.DTOs
{
    public class RegisterDonorDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Phone]
        [MaxLength(50)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;        
    }
}