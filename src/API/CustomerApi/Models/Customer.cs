using System;
using System.ComponentModel.DataAnnotations;

namespace CustomerApi.Models
{
    public class Customer
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [Range(18, int.MaxValue, ErrorMessage = "Age must be at least 18")]
        public int Age { get; set; }
    }
} 