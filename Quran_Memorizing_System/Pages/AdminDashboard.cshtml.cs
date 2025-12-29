using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Quran_Memorizing_System.Models;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Xml.Linq;

namespace Quran_Memorizing_System.Pages
{
    public class AdminDashboardModel : PageModel
    {
        public int Userscount { get; set; }
        
        public int Circulescount { get; set; }
        public int Examcount { get; set; }
        public int Sessioncount { get; set; }
        DB db;

        [BindProperty]
        [Required]
        [EmailAddress]
        public string aemail { get; set; }
        
        [BindProperty]
        [Required]
        public string aname { get; set; }
        
        [BindProperty]
        [Required]
        public string apassword { get; set; }


        public DataTable admins { get; set; }

        public List<User> users { get; set; }
        public DataTable circules { get; set; }

        public DataTable exams { get; set; }


        public AdminDashboardModel(DB dB)
        {
            db = dB;
        }


        void refereshdata()
        {
            admins = db.getalladmins();
            users = db.getallusers();
            circules = db.getallciruels();
            exams = db.getallexams();

            List<int> nums = db.getstatisitcs();
            Userscount = nums[0];
            Circulescount = nums[1];
            Examcount = nums[2];
            Sessioncount = nums[3];
        }

        public void OnGet()
        {
            string name = HttpContext.Session.GetString("name");
            string role = HttpContext.Session.GetString("role");
            string email = HttpContext.Session.GetString("email");

            refereshdata();
            
            if (role == "Admin")
            {
                List<int> nums = db.getstatisitcs();
                Userscount = nums[0];
                Circulescount = nums[1];
                Examcount = nums[2];
                Sessioncount = nums[3];
            }
            else
            {
                TempData["ErrorMessage"] = "You must be admin";
            }
        }

        public IActionResult OnPostAddadmin()
        {
            if (db.EmailExists(aemail))
            {
                ModelState.AddModelError("aemail", "this email is already in our system");
            }
            
            if (!ModelState.IsValid)
            {
                admins = db.getalladmins();
                return Page();
            }
            
            bool s = db.addadmin(aemail, aname, apassword);
            if (s)
            {
                TempData["SuccessMessage"] = "You added an admin";
            }
            else
            {
                TempData["ErrorMessage"] = "Something went wrong";
            }

            refereshdata();
            return Page();
        }

        public IActionResult OnPostRemove(string email)
        {
            if (db.delteadmin(email))
            {
                TempData["SuccessMessage"] = "You deleted the admin";
            }
            else
            {
                TempData["ErrorMessage"] = "Something went wrong";
            }


            refereshdata();
            ModelState.Clear();
            return Page();
        }

        public IActionResult OnPostRemoveuser(string email, string type)
        {
            if (db.DeleteUser(email, type))
            {
                TempData["SuccessMessage"] = "You deleted a user";
            }
            else
            {
                TempData["ErrorMessage"] = "Something went wrong";
            }
            ModelState.Clear();
            refereshdata();
            return Page();
        }


        public IActionResult OnPostRemovecircule(string name)
        {
            if (db.DeleteCircle(name))
            {
                TempData["SuccessMessage"] = "You deleted a circule";
            }
            else
            {
                TempData["ErrorMessage"] = "Something went wrong";
            }

            ModelState.Clear();
            refereshdata();
            return Page();
        }

        public IActionResult OnPostDeleteexam(int id)
        {
            if (db.deleteExam(id))
            {
                TempData["SuccessMessage"] = "You deleted an exam";
            }
            else
            {
                TempData["ErrorMessage"] = "Something went wrong";
            }

            ModelState.Clear();
            refereshdata();
            return Page();
        }

    }
}
