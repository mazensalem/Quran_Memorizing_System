using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Quran_Memorizing_System.Models;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Quran_Memorizing_System.Pages
{
    public class ResetPasswordModel : PageModel
    {
        private readonly DB _db;

        [BindProperty(SupportsGet = true)]
        public string Token { get; set; }

        [BindProperty(SupportsGet = true)]
        public string role { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
            ErrorMessage = "Password must contain uppercase, lowercase, number, and special character")]
        public string NewPassword { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Please confirm your password")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        public bool IsValidToken { get; set; }
        public string Message { get; set; }

        public ResetPasswordModel(DB db)
        {
            _db = db;
        }

        public void OnGet()
        {
            // Validate token
            DataTable dt = _db.ValidateResetToken(role, Token);
            IsValidToken = dt.Rows.Count > 0;

            if (!IsValidToken)
            {
                Message = "This password reset link is invalid or has expired.";
            }
        }

        public IActionResult OnPost()
        {
            // Validate token again
            DataTable dt = _db.ValidateResetToken(role, Token);
            IsValidToken = dt.Rows.Count > 0;

            if (!IsValidToken)
            {
                Message = "This password reset link is invalid or has expired.";
                return Page();
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Get email from token
            string email = dt.Rows[0]["Email"].ToString();

            // Reset password
            _db.ResetPasswordWithToken(email, role, NewPassword);

            TempData["SuccessMessage"] = "Your password has been reset successfully!";
            return RedirectToPage("/Login_Page");
        }
    }
}