using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Quran_Memorizing_System.Models;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Quran_Memorizing_System.Pages
{
    public class CreateExamModel : PageModel
    {
        DB db;
        public User user { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Exam title is required")]
        public string ExamTitle { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Start time is required")]
        public DateTime StartTime { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "End time is required")]
        public DateTime EndTime { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Exam duration is required")]
        [Range(1, 300, ErrorMessage = "Duration must be between 1 and 300 minutes")]
        public int ExamDuration { get; set; }

        [BindProperty]
        public bool PublicAvailability { get; set; }

        [BindProperty]
        public int? CircleID { get; set; }

        [BindProperty]
        public List<Question> Questions { get; set; } = new List<Question>();

        // List of circles the user belongs to (for dropdown)
        public List<CircleOption> UserCircles { get; set; } = new List<CircleOption>();

        public class CircleOption
        {
            public int ID { get; set; }
            public string Name { get; set; }
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
        public CreateExamModel(DB dB)
        {
            user = new User();
            db = dB;
        }
        public IActionResult OnGet()
        {
            setuser();
            ExamTitle = "";
            StartTime = DateTime.Now;
            EndTime = DateTime.Now.AddHours(1);
            ExamDuration = 60;
            PublicAvailability = false;
            CircleID = null;
            Questions = new List<Question> { };

            // Fetch user's circles
            if (!String.IsNullOrEmpty(user.Email) && !String.IsNullOrEmpty(user.role))
            {
                DataTable circlesTable = db.getusercirules(user.Email, user.role);
                UserCircles = new List<CircleOption>();
                
                foreach (DataRow row in circlesTable.Rows)
                {
                    UserCircles.Add(new CircleOption
                    {
                        ID = Convert.ToInt32(row["ID"]),
                        Name = Convert.ToString(row["Name"])
                    });
                }
            }

            if (user.isverified)
            {
                return Page();
            }

            // Ensure end time is after start time
            if (EndTime <= StartTime)
            {
                ModelState.AddModelError("EndTime", "End time must be after start time.");
                setuser();
                if (!String.IsNullOrEmpty(user.Email) && !String.IsNullOrEmpty(user.role))
                {
                    DataTable circlesTable = db.getusercirules(user.Email, user.role);
                    UserCircles = new List<CircleOption>();

                    foreach (DataRow row in circlesTable.Rows)
                    {
                        UserCircles.Add(new CircleOption
                        {
                            ID = Convert.ToInt32(row["ID"]),
                            Name = Convert.ToString(row["Name"])
                        });
                    }
                }
                return Page();
            }
            else
            {
                return RedirectToPage("/index");
            }
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                // Repopulate circles list for dropdown
                setuser();
                if (!String.IsNullOrEmpty(user.Email) && !String.IsNullOrEmpty(user.role))
                {
                    DataTable circlesTable = db.getusercirules(user.Email, user.role);
                    UserCircles = new List<CircleOption>();
                    
                    foreach (DataRow row in circlesTable.Rows)
                    {
                        UserCircles.Add(new CircleOption
                        {
                            ID = Convert.ToInt32(row["ID"]),
                            Name = Convert.ToString(row["Name"])
                        });
                    }
                }
                return Page();
            }

            Exam exam = new Exam();
            exam.Circle_ID = CircleID;
            exam.starttime = StartTime;
            exam.endtime = EndTime;
            exam.Questions = Questions;
            exam.examduration = ExamDuration;
            exam.Sheikh_email = HttpContext.Session.GetString("email");
            exam.PublicAvailabilty = PublicAvailability;
            exam.Title = ExamTitle;

            if (db.Examnameexists(exam.Title))
            {
                ModelState.AddModelError("ExamTitle", "This name is already taken");

                if (!String.IsNullOrEmpty(user.Email) && !String.IsNullOrEmpty(user.role))
                {
                    DataTable circlesTable = db.getusercirules(user.Email, user.role);
                    UserCircles = new List<CircleOption>();

                    foreach (DataRow row in circlesTable.Rows)
                    {
                        UserCircles.Add(new CircleOption
                        {
                            ID = Convert.ToInt32(row["ID"]),
                            Name = Convert.ToString(row["Name"])
                        });
                    }
                }

                return Page();
            }


            if (db.createExam(exam))
            {
                TempData["SuccessMessage"] = "Exam created successfully!";
                return RedirectToPage("/Home");
            }
            else
            {
                TempData["ErrorMessage"] = "Something went wrong";
                return RedirectToPage("/CreateExam");
            }
        }
    }
}
