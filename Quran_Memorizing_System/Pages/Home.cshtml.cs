using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Quran_Memorizing_System.Models;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Quran_Memorizing_System.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public record Notification(string Message, string TimeAgo);

        public DataTable Circles { get; set; }
        public List<Notification> Notifications { get; set; } = new();
        public bool isAdmin { get; set; }
        [BindProperty]
        public int SelectedCircleId { get; set; }

        public SelectList CircleOptions { get; set; }


        DB db;
        public User user { get; set; }

        [BindProperty]
        [Required]
        public string newcirculename { get; set; }
        [BindProperty]
        public bool ispublic { get; set; }

        public IndexModel(ILogger<IndexModel> logger, DB dB)
        {
            _logger = logger;
            db = dB;
            user = new User();

            Notifications = new List<Notification>
            {
                new Notification("New lesson available", "2h ago"),
                new Notification("Exam scheduled tomorrow", "1d ago")
            };
        }

        void setuser()
        {
            if (!String.IsNullOrEmpty(HttpContext.Session.GetString("email")))
            {
                var role = HttpContext.Session.GetString("role");
                var email = HttpContext.Session.GetString("email");
                DataTable userdt = db.GetUser(email, role);

                user.UserName = Convert.ToString(userdt.Rows[0]["UserName"]);
                user.PhoneNumber = Convert.ToInt32(userdt.Rows[0]["Phone"]);
                user.Email = email;
                user.gender = Convert.ToString(userdt.Rows[0]["Gender"]);
                user.role = role;
                user.PhoneVisability = Convert.ToBoolean(userdt.Rows[0]["Phonevisability"]);
                user.DateOfBirth = Convert.ToDateTime(userdt.Rows[0]["DateofBirth"]).ToShortDateString();

                if (user.gender == "F")
                {
                    user.gender = "Female";
                }
                else
                {
                    user.gender = "Male";
                }

                if (user.role == "Participant")
                {
                    user.isverified = false;
                }
                else
                {
                    user.isverified = Convert.ToBoolean(userdt.Rows[0]["isverifed"]);
                }
            }
        }

        public void OnGet()
        {
            setuser();


            if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.role))
            {
                Circles = new DataTable();
                CircleOptions = new SelectList(Enumerable.Empty<SelectListItem>());
                isAdmin = false;
                return;
            }

            Circles = db.getusercirules(user.Email, user.role);


            CircleOptions = new SelectList(
                Circles.AsEnumerable(),
                "ID",      
                "Name"     
            );

            /*We store role as sheikhs and participants*/
            isAdmin = (user.role == "Sheikh");
        }

        public IActionResult OnPostSelectCircle()
        {
            setuser();
            Circles = db.getusercirules(user.Email, user.role);

            CircleOptions = new SelectList(
                Circles.AsEnumerable(),
                "ID",
                "Name"
            );

          
          
            return RedirectToPage("/Memorization_Circle", new { Name = GetCircleNameById(SelectedCircleId) });
        }

        private string GetCircleNameById(int id)
        {
            foreach (DataRow row in Circles.Rows)
            {
                if (Convert.ToInt32(row["ID"]) == id)
                    return Convert.ToString(row["Name"]);
            }
            return "";
        }


    }
}
