using System.Data;
using System.Data.SqlClient;

namespace Quran_Memorizing_System.Models
{
    public class DB
    {
        private string connectionstring;

        private SqlConnection con;

        public DB()
        {
            connectionstring = "Data Source=Elabd;Initial Catalog=MemorizationSystem;Integrated Security=True;";
            con = new SqlConnection(connectionstring);
        }

        public void AddUser(User user)
        {
            try
            {
                con.Open();

                string query = "";
                if (user.role == "Participant") {
                    query = "INSERT INTO Participants (Email, UserName, Password, Gender, Phone, Phonevisability, DateofBirth) VALUES (@Email, @UserName, @Passwordhased, @Gender, @Phone, @Phonevisiablity, @DateOfBirth)";
                }
                else {
                    query = "INSERT INTO Sheikhs (Email, UserName, Password, Gender, Phone, Phonevisability, DateofBirth) VALUES (@Email, @UserName, @Passwordhased, @Gender, @Phone, @Phonevisiablity, @DateOfBirth)";
                }

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@Email", user.Email.ToLower());
                cmd.Parameters.AddWithValue("@UserName", user.UserName);

                string hashedpassword = BCrypt.Net.BCrypt.HashPassword(user.Password);
                cmd.Parameters.AddWithValue("@Passwordhased", hashedpassword);

                cmd.Parameters.AddWithValue("@Gender", user.gender[0]);
                cmd.Parameters.AddWithValue("@Phone", user.PhoneNumber);
                cmd.Parameters.AddWithValue("@Phonevisiablity", user.PhoneVisability);
                cmd.Parameters.AddWithValue("@DateOfBirth", user.DateOfBirth);


                cmd.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                con.Close();
            }
        }
        public DataTable FindUser(string email, string password, string type)
        {
            string table = "";
            if (type == "Participant") { table = "Participants"; }
            else { table = "Sheikhs"; }
            string query = $"SELECT * FROM {table} WHERE Email = @Email";
            DataTable res = new DataTable();

            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Email", email.ToLower());
                SqlDataReader reader = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);

                if (BCrypt.Net.BCrypt.Verify(password, Convert.ToString(dt.Rows[0]["Password"])))
                {
                    res = dt;
                }
            }
            catch
            {
            }
            finally
            {
                con.Close();
            }
            return res;
        }

        public bool EmailExists(string email, string type)
        {
            bool found = true;
            try
            {
                con.Open();
                string table = "";
                if (type == "Participant")
                {
                    table = "Participants";
                }
                else
                {
                    table = "Sheikhs";
                }

                string query = $"SELECT COUNT(*) FROM {table} WHERE Email=@email";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@email", email.ToLower());

                int res = Convert.ToInt32(cmd.ExecuteScalar());
                if (res == 0)
                    found = false;
            }
            catch
            {

            }
            finally
            {
                con.Close();
            }
            return found;
        }
        public bool PhoneExists(int phone, string type)
        {
            bool found = true;
            try
            {
                con.Open();
                string table = "";
                if (type == "Participant")
                {
                    table = "Participants";
                }
                else
                {
                    table = "Sheikhs";
                }

                string query = $"SELECT COUNT(*) FROM {table} WHERE Phone=@phone";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@phone", phone);

                int res = Convert.ToInt32(cmd.ExecuteScalar());
                if (res == 0)
                    found = false;
            }
            catch
            {

            }
            finally
            {
                con.Close();
            }
            return found;
        }
        public DataTable GetUser(string email, string type)
        {
            string table = "";
            if (type == "Participant") { table = "Participants"; }
            else { table = "Sheikhs"; }
            string query = $"SELECT * FROM {table} WHERE Email = @Email";
            DataTable res = new DataTable();

            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Email", email.ToLower());
                SqlDataReader reader = cmd.ExecuteReader();
                res.Load(reader);
            }
            catch
            {
            }
            finally
            {
                con.Close();
            }
            return res;
        }
        public bool DeleteUser(string email, string type)
        {
            bool status = false;
            string table = "";
            if (type == "Participant") { table = "Participants"; }
            else { table = "Sheikhs"; }
            string query = $"DELETE {table} WHERE Email = @email";
            DataTable res = new DataTable();
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.ExecuteNonQuery();
                status = true;
            }
            catch
            {

            }
            finally
            {
                con.Close();
            }
            return status;
        }
        public bool changepassword(string email, string role, string oldpass, string newpass)
        {
            bool status = false;
            try
            {
                con.Open();
                string table = (role == "Participant") ? "Participants" : "Sheikhs";
                string query = $"SELECT * FROM {table} WHERE Email = @email";
                SqlCommand cmd1 = new SqlCommand(query, con);
                cmd1.Parameters.AddWithValue("@email", email.ToLower());
                
                DataTable dt = new DataTable();
                dt.Load(cmd1.ExecuteReader());
                
                if (BCrypt.Net.BCrypt.Verify(oldpass, Convert.ToString(dt.Rows[0]["password"])))
                {
                    string q2 = $"Update {table} SET password=@password WHERE Email=@email";
                    SqlCommand cmd2 = new SqlCommand(q2, con);
                    cmd2.Parameters.AddWithValue("@email", email.ToLower());
                    cmd2.Parameters.AddWithValue("@password", BCrypt.Net.BCrypt.HashPassword(newpass));

                    cmd2.ExecuteNonQuery();
                    status = true;
                }
                else
                {
                    status = false;
                }
            }
            catch
            {

            }
            finally
            {
                con.Close();
            }
            return status;
        }
        public bool EditUser(string email, string role, User user)
        {
            bool status = false;
            try
            {
                con.Open();
                string query = "";
                if (user.role == "Participant")
                {
                    query = "UPDATE Participants SET UserName=@username,Gender=@gender, Phone=@phone, Phonevisability=@phonevis, DateofBirth=@dateofbirth WHERE Email=@email";
                    
                }
                else
                {
                    query = "UPDATE Sheikhs SET UserName=@username,Gender=@gender, Phone=@phone, Phonevisability=@phonevis, DateofBirth=@dateofbirth WHERE Email=@email";
                }

                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@email", user.Email.ToLower());
                cmd.Parameters.AddWithValue("@username", user.UserName);
                cmd.Parameters.AddWithValue("@gender", user.gender[0]);
                cmd.Parameters.AddWithValue("@phone", user.PhoneNumber);
                cmd.Parameters.AddWithValue("@phonevis", user.PhoneVisability);
                cmd.Parameters.AddWithValue("@dateofbirth", user.DateOfBirth);

                cmd.ExecuteNonQuery();
                status = true;
            }
            catch
            {}
            finally
            {
                con.Close();
            }

            return status;
        }

        public void SaveResetToken(string email, string role, string token, DateTime expiry)
        {
            string table = (role == "Participant") ? "Participants" : "Sheikhs";
            string query = $@"UPDATE {table} 
                     SET ResetToken = @Token, ResetTokenExpiry = @Expiry 
                     WHERE Email = @Email";
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Email", email.ToLower());
                cmd.Parameters.AddWithValue("@Token", token);
                cmd.Parameters.AddWithValue("@Expiry", expiry);

                cmd.ExecuteNonQuery();
            }
            catch {}
            finally
            {
                con.Close();
            }
        }

        public DataTable ValidateResetToken(string role, string token)
        {
            string table = (role == "Participant") ? "Participants" : "Sheikhs";
            string query = $@"SELECT Email FROM {table} 
                     WHERE ResetToken = @Token 
                     AND ResetTokenExpiry > GETDATE()";
            DataTable dt = new DataTable();
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Token", token);
                dt.Load(cmd.ExecuteReader());
            }
            catch
            {

            }
            finally
            {
                con.Close();
            }

            return dt;
        }

        public void ResetPasswordWithToken(string email, string role, string newPassword)
        {
            string table = (role == "Participant") ? "Participants" : "Sheikhs";
            string query = $@"UPDATE {table} 
                     SET Password = @Password, 
                         ResetToken = NULL, 
                         ResetTokenExpiry = NULL 
                     WHERE Email = @Email";

            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Email", email.ToLower());
                cmd.Parameters.AddWithValue("@Password", BCrypt.Net.BCrypt.HashPassword(newPassword));
                cmd.ExecuteNonQuery();
            }
            catch {  } finally { con.Close(); }
        }

        public DataTable getAllUnderReviewSheikhs()
        {
            DataTable result = new DataTable();

            try
            {
                con.Open();
                string query = "SELECT * FROM Sheikhs WHERE isverifed = 0 or isverifed is null";
                SqlCommand cmd = new SqlCommand(query, con);

                result.Load(cmd.ExecuteReader());
            }
            catch
            {

            }
            finally
            {
                con.Close();
            }

            return result;
        }

        public DataTable getUnderReviewShikhs(string emailorname)
        {
            DataTable result = new DataTable();

            try
            {
                con.Open();

                string query = "SELECT * FROM Sheikhs WHERE (isverifed = 0 or isverifed is null) and (Username LIKE @user OR Email LIKE @user)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@user", "%" + emailorname + "%");

                result.Load(cmd.ExecuteReader());
            }
            catch
            {

            }
            finally
            {
                con.Close();
            }

            return result;
        }

        public bool admitSheikh(string email)
        {
            bool status = true;
            try
            {
                con.Open();
                string query = "UPDATE Sheikhs SET isverifed = 1 WHERE Email = @email";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@email", email);

                cmd.ExecuteNonQuery();
            }
            catch
            {
                status = false;
            }
            finally
            {
                con.Close();
            }
            return status;
        }

        public bool denySheikh(string email)
        {
            bool status = true;
            try
            {
                con.Open();
                string query = "DELETE FROM Sheikhs WHERE Email = @email";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.ExecuteNonQuery();
            }
            catch
            {
                status = false;
            }
            finally
            {
                con.Close();
            }
            return status;
        }

        public bool addCircule(string name, bool p, string email)
        {
            bool status = true;

            try
            {
                con.Open();
                string query1 = "INSERT INTO Memorization_Circles(Name, Public_Avaliable) VALUES (@cname, @public)";
                string query2 = "SELECT ID FROM Memorization_Circles WHERE Name = @cname";
                string query3 = "INSERT INTO Admin_Sheikhs_Circles(Sheikhs_Email, Circle_ID) VALUES (@semail, @cid);"; 
                SqlCommand cmd1 = new SqlCommand(query1, con);
                SqlCommand cmd2 = new SqlCommand(query2, con);
                SqlCommand cmd3 = new SqlCommand(query3, con);

                cmd1.Parameters.AddWithValue("@cname", name);
                cmd1.Parameters.AddWithValue("@public", p);

                cmd2.Parameters.AddWithValue("@cname", name);

                cmd3.Parameters.AddWithValue("@semail", email);


                cmd1.ExecuteNonQuery();
                int cid = Convert.ToInt32(cmd2.ExecuteScalar());
                
                cmd3.Parameters.AddWithValue("@cid", cid);
                cmd3.ExecuteNonQuery();
            }
            catch (SqlException ex)
            {
                status = false;
            }
            finally
            {
                con.Close();
            }

            return status;
        }

        public bool circuleNameExists(string name)
        {
            bool found = false;

            try
            {
                con.Open();
                string query = "SELECT COUNT(*) FROM Memorization_Circles WHERE Name = @name";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@name", name);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                if (count >= 1)
                {
                    found = true;
                }
            }
            catch
            {

            }
            finally
            {
                con.Close();
            }

            return found;
        }

        public DataTable getusercirules(string email, string role)
        {
            DataTable dt = new DataTable();
            try
            {
                con.Open();
                string query = "";
                if (role == "Participant")
                {
                    query = "SELECT * FROM Memorization_Circles WHERE ID in (SELECT Circle_ID FROM Participant_Circle_Attend WHERE Participant_Email = @email)";
                }
                else
                {
                    query = "SELECT * FROM Memorization_Circles WHERE ID in (SELECT Circle_ID FROM Admin_Sheikhs_Circles WHERE Sheikhs_Email = @email)";
                }
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@email", email);

                dt.Load(cmd.ExecuteReader());
            }
            catch
            {

            }
            finally
            {
                con.Close();
            }
            return dt;
        }

        public DataTable getallCircles()
        {
            DataTable dt = new DataTable();

            try
            {
                con.Open();
                string query = "SELECT * FROM Memorization_Circles WHERE Public_Avaliable = 1";
                SqlCommand cmd = new SqlCommand(query, con);
                dt.Load(cmd.ExecuteReader());
            }
            catch
            {

            }
            finally
            {
                con.Close();
            }

            return dt;
        }

        public DataTable findCircuits(string name)
        {
            DataTable dt = new DataTable();

            try
            {
                con.Open();
                string query = "SELECT * FROM Memorization_Circles WHERE Public_Avaliable = 1 AND Name LIKE @name";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@name", "%" + name + "%");
                dt.Load(cmd.ExecuteReader());
            }
            catch
            {

            }
            finally
            {
                con.Close();
            }

            return dt;
        }

        public DataTable GetAnnouncments(string c_name)
        {
            DataTable dt = new DataTable();
            try
            {
                con.Open();
                string query = "SELECT * FROM Announcment WHERE Circle_ID = (SELECT ID FROM Memorization_Circles WHERE Name = @cname)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@cname", c_name);
                dt.Load(cmd.ExecuteReader());
            }
            catch
            {

            }
            finally
            {
                con.Close();
            }
            return dt;
        }

        public bool addtocircule(string email, string cname)
        {
            bool status = false;
            try
            {
                con.Open();
                string query = "INSERT INTO Participant_Circle_Attend VAlUES (@email, @cid)";
                string querytemp = "SELECT ID FROM Memorization_Circles WHERE Name = @cname";
                SqlCommand cmd = new SqlCommand(query, con);
                SqlCommand cmd2 = new SqlCommand(querytemp, con);


                cmd2.Parameters.AddWithValue("@cname", cname);

                int cid = Convert.ToInt32(cmd2.ExecuteScalar());
                
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@cid", cid);
                cmd.ExecuteNonQuery();
                status = true;
            }
            catch(SqlException ex)
            {
                Console.WriteLine(ex);
                status = false;
            }
            finally
            {
                con.Close();
            }
            return status;
        }

        public DataTable getusersincircule(string cname)
        {
            DataTable dt = new DataTable();
            try
            {
                con.Open();
                string query = "SELECT * FROM Participant_Circle_Attend WHERE Circle_ID IN (SELECT ID FROM Memorization_Circles WHERE Name = @cname)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@cname", cname);
                dt.Load(cmd.ExecuteReader());
            }
            catch
            {

            }
            finally
            {
                con.Close();
            }
            return dt;
        }

        public bool removefromcircle(string email, string cname)
        {
            bool status = false;

            try
            {
                con.Open();
                string query = "DELETE FROM Participant_Circle_Attend WHERE Participant_Email = @email and Circle_ID IN (SELECT ID FROM Memorization_Circles WHERE Name = @cname)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@cname", cname);
                cmd.ExecuteNonQuery();
                status = true;
            }
            catch
            {
                status = false;
            }
            finally
            {
                con.Close();
            }

            return status;
        }

        public bool isincircle(string email, string cname) {
            bool status = false;

            try
            {
                con.Open();
                string query = "SELECT COUNT(*) FROM Participant_Circle_Attend WHERE Participant_Email=@email and Circle_ID IN (SELECT ID FROM Memorization_Circles WHERE Name = @cname)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@cname", cname);

                int count = Convert.ToInt32(cmd.ExecuteScalar());
                if (count >= 1)
                    status = true;
            }
            catch
            {

            }
            finally
            {
                con.Close();
            }

            return status;
        }
        //for abdate circle 
        public bool UpdateCircle(string oldName, string newName, bool isPublic)
        {
            bool status = false;
            try
            {
                con.Open();
                string query = "UPDATE Memorization_Circles SET Name = @newName, Public_Avaliable = @public WHERE Name = @oldName";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@newName", newName);
                cmd.Parameters.AddWithValue("@public", isPublic);
                cmd.Parameters.AddWithValue("@oldName", oldName);
                cmd.ExecuteNonQuery();
                status = true;
            }
            catch { status = false; }
            finally { con.Close(); }
            return status;
        }
        //for delete circle
        public bool DeleteCircle(string name)
        {
            bool status = false;
            try
            {
                con.Open();
                string query = "DELETE FROM Memorization_Circles WHERE Name = @name";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.ExecuteNonQuery();
                status = true;
            }
            catch { status = false; }
            finally { con.Close(); }
            return status;
        }

        public bool AddAnnouncementToCircle(string circleName, string sheikhEmail, string description)
        {
            bool status = false;
            try
            {
                con.Open();

                string getIdQuery = "SELECT ID FROM Memorization_Circles WHERE Name = @cname";
                SqlCommand cmdGetId = new SqlCommand(getIdQuery, con);
                cmdGetId.Parameters.AddWithValue("@cname", circleName);
                object res = cmdGetId.ExecuteScalar();
                if (res == null) return false;
                int cid = Convert.ToInt32(res);

                string insertQuery = @"INSERT INTO Announcment
                               (Circle_ID, Sh_Email, Description, Date) 
                               VALUES (@cid, @email, @desc, GETDATE())";
                SqlCommand cmdInsert = new SqlCommand(insertQuery, con);
                cmdInsert.Parameters.AddWithValue("@cid", cid);
                cmdInsert.Parameters.AddWithValue("@email", sheikhEmail.ToLower());
                cmdInsert.Parameters.AddWithValue("@desc", description);

                cmdInsert.ExecuteNonQuery();
                status = true;
            }
            catch
            {
                status = false;
            }
            finally
            {
                con.Close();
            }
            return status;
        }
        public bool UpdateAnnouncement(int id, string description)
        {
            bool status = false;
            try
            {
                con.Open();
                string query = "UPDATE Announcment SET Description = @desc WHERE Id = @id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@desc", description);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                status = true;
            }
            catch { status = false; }
            finally { con.Close(); }
            return status;
        }

        public bool DeleteAnnouncement(int id)
        {
            bool status = false;
            try
            {
                con.Open();
                string query = "DELETE FROM Announcment WHERE Id = @id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                status = true;
            }
            catch { status = false; }
            finally { con.Close(); }
            return status;
        }
        public DataTable GetCommentsForAnnouncement(int announcementId)
        {
            DataTable dt = new DataTable();
            try
            {
                con.Open();
                string query = @"SELECT Id, Announcement_Id, Participant_Email, Comment_Text, Comment_Date
                         FROM Comments
                         WHERE Announcement_Id = @aid
                         ORDER BY Comment_Date";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@aid", announcementId);
                dt.Load(cmd.ExecuteReader());
            }
            finally { con.Close(); }
            return dt;
        }

        public bool AddComment(int announcementId, string participantEmail, string text)
        {
            bool status = false;
            try
            {
                con.Open();
                string query = @"INSERT INTO Comments (Announcement_Id, Participant_Email, Comment_Text)
                         VALUES (@aid, @email, @text)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@aid", announcementId);
                cmd.Parameters.AddWithValue("@email", participantEmail.ToLower());
                cmd.Parameters.AddWithValue("@text", text);
                cmd.ExecuteNonQuery();
                status = true;
            }
            catch { status = false; }
            finally { con.Close(); }
            return status;
        }

        public bool UpdateComment(int id, string participantEmail, string text)
        {
            bool status = false;
            try
            {
                con.Open();
                string query = @"UPDATE Comments 
                         SET Comment_Text = @text 
                         WHERE Id = @id AND Participant_Email = @email";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@email", participantEmail.ToLower());
                cmd.Parameters.AddWithValue("@text", text);
                cmd.ExecuteNonQuery();
                status = true;
            }
            catch { status = false; }
            finally { con.Close(); }
            return status;
        }

        public bool DeleteComment(int id, string participantEmail)
        {
            bool status = false;
            try
            {
                con.Open();
                string query = @"DELETE FROM Comments 
                         WHERE Id = @id AND Participant_Email = @email";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@email", participantEmail.ToLower());
                cmd.ExecuteNonQuery();
                status = true;
            }
            catch { status = false; }
            finally { con.Close(); }
            return status;
        }











    }
}
