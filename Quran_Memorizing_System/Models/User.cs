using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Quran_Memorizing_System.Models
{
    public class User
    {
        [BindProperty]
        [Required]
        [RegularExpression(@"^(Participant|Sheikh)$")]
        public string role { get; set; }
        
        [BindProperty]
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [BindProperty]
        [Required]
        [MinLength(2)]
        public string UserName { get; set; }
        
        [BindProperty]
        [Required]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character")]
        public string Password { get; set; }
        
        [BindProperty]
        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string confirmPassword { get; set; }
        
        [BindProperty]
        [Required]
        public int PhoneNumber { get; set; }
        
        [BindProperty]
        [Required]
        public bool PhoneVisability { get; set; }
        
        [BindProperty]
        [Required]
        public string DateOfBirth { get; set; }

        [BindProperty]
        [Required]
        [RegularExpression(@"^(Male|Female)$", ErrorMessage = "Gender must be Male or Female")]
        public string gender { get; set; }

        public bool isverified { get; set; }

        public User()
        {
            role = "";
            gender = "";
            UserName = "";
            Email = "";
            isverified = false;
        }
    }
}
