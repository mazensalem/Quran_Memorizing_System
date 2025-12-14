using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Quran_Memorizing_System.Models;
using Quran_Memorizing_System.Services;
using System.ComponentModel.DataAnnotations;

namespace Quran_Memorizing_System.Pages
{
    public class Forgot_PasswordModel : PageModel
    {
        DB _db;
        EmailService _emailService;

        [BindProperty]
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [BindProperty]
        public string role { get; set; }

        public Forgot_PasswordModel(DB db, EmailService emailService)
        {
            _db = db;
            _emailService = emailService;
        }

        public void OnGet()
        {
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (!_db.EmailExists(Email, role))
            {
                TempData["Info"] = "If an account exists with this email, you will receive a password reset link.";
                return Page();
            }

            string token = Guid.NewGuid().ToString();
            DateTime expiry = DateTime.Now.AddHours(1);

            _db.SaveResetToken(Email, role, token, expiry);

            var resetLink = Url.Page("/ResetPassword", null, new { token = token, role = role }, Request.Scheme);

            try
            {
                await _emailService.SendPasswordResetEmailAsync(Email, resetLink);
                TempData["Info"] = "If an account exists with this email, you will receive a password reset link.";
            }
            catch (Exception ex)
            {

                TempData["Error"] = "An error occurred while sending the email. Please try again later.";
                Console.WriteLine($"Email error: {ex.Message}");
            }

            return Page();
        }

    }
}
