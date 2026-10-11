using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BackendService.DTOs
{
    public class LoginDto
    {
        [Required]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;        
    }
}