using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Quran_Memorizing_System.Models;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;

namespace Quran_Memorizing_System.Pages
{
    public class MemorizationCircleModel : PageModel
    {
        public DataTable Members { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Name { get; set; }

        public DataTable Posts { get; set; }
        [BindProperty]
        public string NewCircleName { get; set; }

        [BindProperty]
        public bool NewPublicAvailable { get; set; }
        [BindProperty]
        public string NewAnnouncement { get; set; }
        [BindProperty]
        public int EditAnnouncementId { get; set; }

        [BindProperty]
        public int DeleteAnnouncementId { get; set; }

        [BindProperty]
        public string EditAnnouncementText { get; set; }
        [BindProperty]
        public int NewCommentAnnouncementId { get; set; }

        [BindProperty]
        public string NewCommentText { get; set; }

        [BindProperty]
        public int EditCommentId { get; set; }

        [BindProperty]
        public string EditCommentText { get; set; }
        

        DB db;
        private readonly ILogger<MemorizationCircleModel> _logger;
        [BindProperty]
        [Required]
        [EmailAddress]
        public string addmemberemail { get; set; }

        public string role { get; set; }

        public Dictionary<int, DataTable> CommentsByAnnouncement { get; set; } = new();

        public MemorizationCircleModel(ILogger<MemorizationCircleModel> logger, DB dB)
        {
            _logger = logger;
            db = dB;
        }

        public IActionResult OnGet()
        {
   
            if (string.IsNullOrEmpty(Name))
            {
                return RedirectToPage("/Home");
            }


            Posts = db.GetAnnouncments(Name);
            Members = db.getusersincircule(Name);
            role = HttpContext.Session.GetString("role");

            CommentsByAnnouncement = new Dictionary<int, DataTable>();
            foreach (DataRow p in Posts.Rows)
            {
                // There is no ID in the DB
                int annId = Convert.ToInt32(p["Announcment_ID"]);
                CommentsByAnnouncement[annId] = db.GetCommentsForAnnouncement(annId);
            }

            return Page();
        }



        public IActionResult OnPostAddmember()
        {
            _logger.LogInformation("OnPostAddmember invoked. addmemberemail={Email}, Name={Name}", addmemberemail, Name);
            _logger.LogInformation("ModelState.IsValid={Valid}", ModelState.IsValid);

            bool exists = false;
            bool alreadyInCircle = false;
            try
            {
                exists = db.EmailExists(addmemberemail, "Participant");
                alreadyInCircle = db.isincircle(addmemberemail, Name);
                _logger.LogInformation("EmailExists={Exists}, IsInCircle={InCircle}", exists, alreadyInCircle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking email/circle status for {Email}", addmemberemail);
            }

            if (string.IsNullOrEmpty(addmemberemail) || !exists || alreadyInCircle)
            {
                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Please enter a valid email.";
                }
                else if (alreadyInCircle)
                {
                    TempData["ErrorMessage"] = "This email is already in the circle.";
                }
                else
                {
                    TempData["ErrorMessage"] = "This email is not registered.";
                }

                // keep the inline form visible so user can correct
                TempData["ShowAddInline"] = "1";
                return RedirectToPage("/Memorization_Circle", new { Name = Name });
            }

            bool addResult = false;
            try
            {
                addResult = db.addtocircule(addmemberemail, Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding member {Email} to circle {Name}", addmemberemail, Name);
            }

            if (addResult)
            {
                TempData["SuccessMessage"] = "You added successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Something went wrong while adding the member.";
                TempData["ShowAddInline"] = "1";
            }

            return RedirectToPage("/Memorization_Circle", new { Name = Name });
        }

        public IActionResult OnPostRemoveuser(string email)
        {
            if (db.removefromcircle(email, Name))
            {
                TempData["SuccessMessage"] = "You removed one person Successfuly";
            }
            else
            {
                TempData["ErrorMessage"] = "Somthing went wrong";
            }
            return RedirectToPage("/Memorization_Circle", new { Name = Name});
        }
       

        // Update
        public IActionResult OnPostUpdateCircle()
        {
            if (db.UpdateCircle(Name, NewCircleName, NewPublicAvailable))
            {
                TempData["SuccessMessage"] = "Circle updated successfully";
                return RedirectToPage("/Memorization_Circle", new { Name = NewCircleName });
            }
            TempData["ErrorMessage"] = "Error while updating circle";
            return RedirectToPage("/Memorization_Circle", new { Name = Name });
        }

        // Delete
        public IActionResult OnPostDeleteCircle()
        {
            if (db.DeleteCircle(Name))
            {
                TempData["SuccessMessage"] = "Circle deleted successfully";
                return RedirectToPage("/Home");
            }
            TempData["ErrorMessage"] = "Error while deleting circle";
            return RedirectToPage("/Memorization_Circle", new { Name = Name });
        }
     

        //  Add Announcement
        public IActionResult OnPostAddAnnouncement()
        {
            // Take care you relay on role which is empty by the time this is called
            if (HttpContext.Session.GetString("role") != "Sheikh")
            {
                TempData["ErrorMessage"] = "You are not allowed to post.";
                return RedirectToPage("/Memorization_Circle", new { Name = Name });
            }

            if (string.IsNullOrWhiteSpace(NewAnnouncement))
            {
                TempData["ErrorMessage"] = "Announcement cannot be empty.";
                return RedirectToPage("/Memorization_Circle", new { Name = Name });
            }

            bool status = db.AddAnnouncementToCircle(Name, HttpContext.Session.GetString("email"), NewAnnouncement);
            if (status)
                TempData["SuccessMessage"] = "Announcement posted successfully.";
            else
                TempData["ErrorMessage"] = "Something went wrong while posting.";

            return RedirectToPage("/Memorization_Circle", new { Name = Name });
        }



        // Edit
        public IActionResult OnPostEditAnnouncement()
        {
            if (string.IsNullOrWhiteSpace(EditAnnouncementText))
            {
                TempData["ErrorMessage"] = "Announcement cannot be empty.";
                return RedirectToPage("/Memorization_Circle", new { Name = Name });
            }

            // Authorization: allow if current user is Sheikh or the owner of announcement
            var sessionEmail = HttpContext.Session.GetString("email");
            var sessionRole = HttpContext.Session.GetString("role");

            var owner = db.GetAnnouncementOwner(EditAnnouncementId);
            if (sessionRole != "Sheikh" && (owner == null || sessionEmail == null || !string.Equals(owner, sessionEmail, StringComparison.OrdinalIgnoreCase)))
            {
                TempData["ErrorMessage"] = "You are not allowed to edit this announcement.";
                return RedirectToPage("/Memorization_Circle", new { Name = Name });
            }

            bool updated = db.UpdateAnnouncement(EditAnnouncementId, EditAnnouncementText);

            if (updated)
                TempData["SuccessMessage"] = "Announcement updated successfully.";
            else
                TempData["ErrorMessage"] = "Error while updating announcement.";

            return RedirectToPage("/Memorization_Circle", new { Name = Name });
        }


        // Delete
        public IActionResult OnPostDeleteAnnouncement()
        {
            int id = DeleteAnnouncementId;

            // Authorization: allow if current user is Sheikh or the owner of announcement
            var sessionEmail = HttpContext.Session.GetString("email");
            var sessionRole = HttpContext.Session.GetString("role");

            var owner = db.GetAnnouncementOwner(id);
            if (sessionRole != "Sheikh" && (owner == null || sessionEmail == null || !string.Equals(owner, sessionEmail, StringComparison.OrdinalIgnoreCase)))
            {
                TempData["ErrorMessage"] = "You are not allowed to delete this announcement.";
                return RedirectToPage("/Memorization_Circle", new { Name = Name });
            }

            if (db.DeleteAnnouncement(id))
                TempData["SuccessMessage"] = "Announcement deleted successfully.";
            else
                TempData["ErrorMessage"] = "Error while deleting announcement.";

            return RedirectToPage("/Memorization_Circle", new { Name = Name });
        }
        //comments
        public IActionResult OnPostAddComment()
        {
            string email = HttpContext.Session.GetString("email"); // الطالب
            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "You must be logged in.";
                return RedirectToPage("/Memorization_Circle", new { Name = Name });
            }

            if (string.IsNullOrWhiteSpace(NewCommentText))
            {
                TempData["ErrorMessage"] = "Comment cannot be empty.";
                return RedirectToPage("/Memorization_Circle", new { Name = Name });
            }

            if (db.AddComment(NewCommentAnnouncementId, email, NewCommentText))
                TempData["SuccessMessage"] = "Comment added.";
            else
                TempData["ErrorMessage"] = "Error adding comment.";

            return RedirectToPage("/Memorization_Circle", new { Name = Name });
        }

        public IActionResult OnPostEditComment()
        {
            string email = HttpContext.Session.GetString("email");
            if (db.UpdateComment(EditCommentId, email, EditCommentText))
                TempData["SuccessMessage"] = "Comment updated.";
            else
                TempData["ErrorMessage"] = "Error updating comment.";

            return RedirectToPage("/Memorization_Circle", new { Name = Name });
        }

       public IActionResult OnPostDeleteComment(int announcementId, string participantEmail, DateTime time)
{
    var sessionEmail = HttpContext.Session.GetString("email");
    if (sessionEmail == null || sessionEmail.ToLower() != participantEmail.ToLower())
    {
        TempData["ErrorMessage"] = "You cannot delete this comment.";
        return RedirectToPage("/Memorization_Circle", new { Name = Name });
    }

    if (db.DeleteComment(announcementId, participantEmail, time))
        TempData["SuccessMessage"] = "Comment deleted.";
    else
        TempData["ErrorMessage"] = "Error deleting comment.";

    return RedirectToPage("/Memorization_Circle", new { Name = Name });
}







    }
}
