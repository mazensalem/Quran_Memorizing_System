using Quran_Memorizing_System.Pages;
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
            connectionstring = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=MemorizationSystem2;Integrated Security=True;";
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
            /*You can't just delete the circle you must delte all the posts and comments and shiecks_links and participant_links asosiated with it*/

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
                               (Circle_ID, Sh_Email, Description, Time) 
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

            // تحقق من وجود بيانات صالحة
            if (id <= 0 || string.IsNullOrWhiteSpace(description))
                return false;
            try
            {
                con.Open();
              string query = "UPDATE Announcment SET Description = @desc WHERE Announcment_ID = @id";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.Add("@desc", SqlDbType.NVarChar, 500).Value = description;
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;

                    int rowsAffected = cmd.ExecuteNonQuery();
                    status = rowsAffected > 0; 
                }
            }
            catch (Exception ex)
            {
               
                Console.WriteLine("Error updating announcement: " + ex.Message);
                status = false;
             }
            finally
            {
                con.Close();
            }

            return status;
        }
        ////////////////////
            

        public bool createExam(Exam exam)
        {
            bool status = false;
            try
            {
                con.Open();
                string query = "INSERT INTO Exams(PublicAvailablity, starttime, endtime, examduration, Sheikh_email, Circle_ID, Title) VALUES (@publicav, @stime, @etime, @examdur, @email, @cid, @title);";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@publicav", exam.PublicAvailabilty);
                cmd.Parameters.AddWithValue("@stime", exam.starttime);
                cmd.Parameters.AddWithValue("@etime", exam.endtime);
                cmd.Parameters.AddWithValue("@examdur", exam.examduration);
                cmd.Parameters.AddWithValue("@email", exam.Sheikh_email);
                if (exam.Circle_ID != null)
                {
                    cmd.Parameters.AddWithValue("@cid", exam.Circle_ID);
                }
                else
                {
                    _ = cmd.Parameters.AddWithValue("@cid", DBNull.Value);
                }
                cmd.Parameters.AddWithValue("@title", exam.Title);
                cmd.ExecuteNonQuery();

                int examid = 0;
                string temp = "SELECT Exam_ID FROM Exams WHERE Title = @title";
                SqlCommand cmdtemp = new SqlCommand(temp, con);
                cmdtemp.Parameters.AddWithValue("@title", exam.Title);
                examid = (int)cmdtemp.ExecuteScalar();

                foreach (var question in exam.Questions)
                {
                    string query1 = "INSERT INTO Questions(Title, Exam_ID, QType) VALUES (@Title, @Exam_ID, @QType)";
                    SqlCommand cmd1 = new SqlCommand(query1, con);
                    
                    cmd1.Parameters.AddWithValue("@Title", question.Title);
                    cmd1.Parameters.AddWithValue("@Exam_ID", examid);
                    cmd1.Parameters.AddWithValue("@QType", question.Type);
                    cmd1.ExecuteNonQuery();

                    int questionid = 0;
                    string temp2 = "SELECT Q_ID FROM Questions WHERE Title = @title";
                    SqlCommand cmdtemp2 = new SqlCommand(temp2, con);
                    cmdtemp2.Parameters.AddWithValue("@title", question.Title);
                    questionid = (int)cmdtemp2.ExecuteScalar();

                    if (question.Type == "mcq")
                    {
                        foreach (var choice in question.Choices)
                        {
                            string query2 = "INSERT INTO Choices (Choice, Q_ID) VALUES (@ch, @qid)";
                            SqlCommand cmd2 = new SqlCommand(query2, con);
                            cmd2.Parameters.AddWithValue("@ch", choice.Text);
                            cmd2.Parameters.AddWithValue("@qid", questionid);
                            cmd2.ExecuteNonQuery();
                        }

                        string finalquery = "UPDATE Questions SET correctchoice = @corect WHERE Q_ID = @id";
                        SqlCommand finalcmd = new SqlCommand(finalquery, con);
                        finalcmd.Parameters.AddWithValue("@corect", question.CorrectAnswerText);
                        finalcmd.Parameters.AddWithValue("@id", questionid);
                        finalcmd.ExecuteNonQuery();
                    }
                }


                status = true;
            }
            catch (SqlException ex)
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

        public DataTable getCreatedExams(string email)
        {
            DataTable data = new DataTable();
            try
            {
                con.Open();
                string query = "SELECT Exam_ID, Title FROM Exams WHERE Sheikh_email = @email";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@email", email);
                data.Load(cmd.ExecuteReader());
            }
            catch
            {

            }
            finally
            {
                con.Close();
            }

            return data;
        }

        public bool deleteExam(int examid)
        {
            bool status = false;
            try
            {
                con.Open();
                string querytemp = "UPDATE Questions SET correctchoice = null WHERE Exam_ID=@id";
                SqlCommand cmdtemp = new SqlCommand(querytemp, con);
                cmdtemp.Parameters.AddWithValue("@id", examid);
                cmdtemp.ExecuteNonQuery();



                string qury3 = "DELETE FROM Exams WHERE Exam_ID = @id";
                string qury2 = "DELETE FROM Questions WHERE Exam_ID = @id";
                string query1 = "DELETE FROM Choices WHERE Q_ID IN (SELECT Q_ID FROM Questions WHERE Exam_ID = @id)";

                SqlCommand cmd1 = new SqlCommand(query1, con);
                cmd1.Parameters.AddWithValue("@id", examid);
                cmd1.ExecuteNonQuery();

                SqlCommand cmd2 = new SqlCommand(qury2, con);
                cmd2.Parameters.AddWithValue("@id", examid);
                cmd2.ExecuteNonQuery();

                SqlCommand cmd3 = new SqlCommand(qury3, con);
                cmd3.Parameters.AddWithValue("@id", examid);
                cmd3.ExecuteNonQuery();

                status = true;
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                con.Close();
            }
            return status;
        }

        public DataTable getExam(int examid)
        {
            DataTable dt = new DataTable();

            try
            {
                con.Open();
                string query = "SELECT * FROM Exams WHERE Exam_ID=@id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", examid);
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

        public int startExam(int examid, string email)
        {
            int exam_sub_id = 0;
            try
            {
                con.Open();

                string query1 = "SELECT * FROM Exam_Submissions WHERE Exam_ID = @examid and Participant_Email = @email and Submited = 0";
                DataTable dt = new DataTable();
                SqlCommand cmd1 = new SqlCommand(query1, con);
                cmd1.Parameters.AddWithValue("@email", email);
                cmd1.Parameters.AddWithValue("@examid", examid);
                dt.Load(cmd1.ExecuteReader());
                if (dt.Rows.Count > 0)
                {
                    return (int)dt.Rows[0]["Exam_Sub_ID"];
                }

                string query = "INSERT INTO Exam_Submissions (Exam_ID, Participant_Email, Submited) VALUES (@examid, @email, 0)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@examid", examid);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.ExecuteNonQuery();

                string query2 = "SELECT Exam_Sub_ID FROM Exam_Submissions WHERE Exam_ID = @examid and Participant_Email = @email ORDER BY StartDate";
                SqlCommand cmd2 = new SqlCommand(query2, con);
                cmd2.Parameters.AddWithValue("@examid", examid);
                cmd2.Parameters.AddWithValue("@email", email);
                exam_sub_id = (int)cmd2.ExecuteScalar();
            }
            catch
            {
            }
            finally
            {
                con.Close();
            }
            return exam_sub_id;
        }

        public List<Question> getQuestiosn(int examid)
        {
            List<Question> Questions = new List<Question> { };
            try
            {
                con.Open();
                string query = "SELECT * FROM Questions WHERE Exam_ID = @examid";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@examid", examid);
                DataTable questions = new DataTable();
                questions.Load(cmd.ExecuteReader());
                foreach (DataRow question in questions.Rows)
                {
                    Question questionmodel = new Question();
                    questionmodel.Title = Convert.ToString(question["Title"]);
                    questionmodel.Type = Convert.ToString(question["QType"]);

                    string query2 = "SELECT * FROM Choices WHERE Q_ID = @qid";
                    DataTable choices = new DataTable();
                    SqlCommand cmd2 = new SqlCommand(query2, con);
                    cmd2.Parameters.AddWithValue("@qid", question["Q_ID"]);
                    choices.Load(cmd2.ExecuteReader());
                    questionmodel.Choices = new List<Choice> { };

                    foreach (DataRow choice in choices.Rows)
                    {
                        Choice ch = new Choice();
                        ch.Text = Convert.ToString(choice["Choice"]);
                        questionmodel.Choices.Add(ch);
                    }

                    Questions.Add(questionmodel);
                }
            }
            catch
            {

            }
            finally
            {
                con.Close();
            }
            return Questions;
        }

        public bool submitExam(string email, List<Question> questions, int subid)
        {
            bool status = false;
            try
            {
                con.Open();
                string query2 = "UPDATE Exam_Submissions SET EndDate = @date, Submited = 1 WHERE Exam_Sub_ID = @subid";
                SqlCommand cmd2 = new SqlCommand(query2, con);
                cmd2.Parameters.AddWithValue("@date", DateTime.Now);
                cmd2.Parameters.AddWithValue("@subid", subid);
                cmd2.ExecuteNonQuery();

                foreach (Question question in questions) 
                {
                    if (string.IsNullOrEmpty(question.CorrectAnswerText)) { continue; }

                    string querytemp = "SELECT Q_ID from Questions where Title = @title";
                    SqlCommand cmdtemp = new SqlCommand(querytemp, con);
                    cmdtemp.Parameters.AddWithValue("@title", question.Title);
                    int qid = (int)cmdtemp.ExecuteScalar();

                    string query = "INSERT INTO QuestionSubmition(Exam_Sub_ID, Question_ID, Answer) Values (@subid, @questionid, @answer)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@subid", subid);
                    cmd.Parameters.AddWithValue("@questionid", qid);
                    cmd.Parameters.AddWithValue("@answer", question.CorrectAnswerText);
                    cmd.ExecuteNonQuery();

                }
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

        public bool DeleteAnnouncement(int id)
        {
            bool status = false;
            try
            {
                con.Open();
                // MisMatch Name [Done]
                // Take care you must delete the comments on the post first
                string query = "DELETE FROM Announcment WHERE Announcment_ID = @id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                status = true;
            }
            catch { status = false; }
            finally { con.Close(); }
            return status;
        }
        
        public string GetAnnouncementOwner(int id)
        {
            try
            {
                con.Open();
                string query = "SELECT Sh_Email FROM Announcment WHERE Announcment_ID = @id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", id);
                object res = cmd.ExecuteScalar();
                if (res == null || res == DBNull.Value) return null;
                return Convert.ToString(res);
            }
            catch
            {
                return null;
            }
            finally { con.Close(); }
        }
        public DataTable GetCommentsForAnnouncement(int announcementId)
          {
            DataTable dt = new DataTable();
            try
            {
                con.Open();
              string query = @"SELECT announcement_Id, participant_Email, text, time
                         FROM Comments
                         WHERE announcement_Id = @aid
                         ORDER BY time";
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
                string query = @"INSERT INTO Comments (Announcement_Id, Participant_Email, text, time)
                         VALUES (@aid, @email, @text, GETDATE())";
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
                         SET text = @text 
                         WHERE Announcement_Id = @id AND Participant_Email = @email";
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

        public bool DeleteComment(int announcementId, string participantEmail, DateTime time)
        {
            bool status = false;
            try
            {
                con.Open();
                string query = @"DELETE FROM Comments 
                         WHERE Announcement_Id = @id 
                           AND Participant_Email = @email
                           AND time = @time";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", announcementId);
                cmd.Parameters.AddWithValue("@email", participantEmail.ToLower());
                cmd.Parameters.AddWithValue("@time", time);
                cmd.ExecuteNonQuery();
                status = true;
            }
            catch { status = false; }
            finally { con.Close(); }
            return status;
        }
        public bool Examnameexists(string examname)
        {
            bool status = true;
            try
            {
                con.Open();
                DataTable dt = new DataTable();
                string query = "SELECT * FROM Exams WHERE Title = @ename";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@ename", examname);
                dt.Load(cmd.ExecuteReader());
                if (dt.Rows.Count == 0)
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

        public int getavaliabletimeexam(int exam_sub_id)
        {
            int timeleft = 0;
            try
            {
                con.Open();
                DataTable dt = new DataTable();
                string query = "SELECT * FROM Exam_Submissions WHERE Exam_Sub_ID = @id and Submited = 0";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", exam_sub_id);
                dt.Load(cmd.ExecuteReader());
                DateTime d = Convert.ToDateTime(dt.Rows[0]["StartDate"]);
                timeleft = Convert.ToInt32((DateTime.Now - d).TotalMinutes);
            }
            catch
            {

            }
            finally
            {
                con.Close();
            }
            return timeleft;
        }

        public DataTable getallperformancereview(string email)
        {
            DataTable dt = new DataTable(); 
            try
            {
                con.Open();
                string query = "SELECT * FROM Performance_Review WHERE Participant_Email = @email";
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
      
        public DataTable GetExamsubmission(int examid)
        {
            DataTable dt = new DataTable();
            try
            {
                con.Open();
                string query = "SELECT * FROM Exam_Submissions WHERE Exam_ID = @examid";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@examid", examid);
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

        public string getExamtitlefromsubid(int subid)
        {
            string res = "";
            try
            {
                con.Open();
                string query = "SELECT Title FROM Exams WHERE Exam_ID IN (SELECT Exam_ID FROM Exam_Submissions WHERE Exam_Sub_ID = @examid)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@examid", subid);
                res = (string)cmd.ExecuteScalar();
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

        public void requestsession(string email, string date, int spage, int epage)
        {
            try
            {
                con.Open();
                string query = "INSERT INTO Sessions (participant_email, session_date, start_page, end_page) values (@pemail, @date, @spage, @epage)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@pemail", email);
                cmd.Parameters.AddWithValue("@date", date);
                cmd.Parameters.AddWithValue("@spage", spage);
                cmd.Parameters.AddWithValue("@epage", epage);

                cmd.ExecuteNonQuery();
            }
            catch
            {

            }
            finally
            {
                con.Close();
            }
          }

        public Dictionary<string, string> getsubmission(int subid, string email)
        {
            Dictionary<string, string> dict = new Dictionary<string, string> { };
            try
            {
                con.Open();
                DataTable dt = new DataTable();
                string query = "SELECT Questions.Title, Answer FROM Exam_Submissions JOIN Exams ON Exams.Exam_ID = Exam_Submissions.Exam_ID JOIN Questions ON Questions.Exam_ID = Exam_Submissions.Exam_ID JOIN QuestionSubmition ON QuestionSubmition.Question_ID = Questions.Q_ID WHERE Exam_Submissions.Exam_Sub_ID = @subid AND Sheikh_email = @semail";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@subid", subid);
                cmd.Parameters.AddWithValue("@semail", email);
                dt.Load(cmd.ExecuteReader());

                foreach (DataRow r in dt.Rows)
                {
                    dict.Add(Convert.ToString(r["Title"]), Convert.ToString(r["Answer"]));
                }
            }
            catch
            {

            }
            finally
            {
                con.Close();
            }
        return dict;
        }

        public void addperformancereview(int subid, int rating, string sh_email)
          {
            try
            {
                con.Open();
                string p_email = "";
                string querytemp = "SELECT Participant_Email FROM Exam_Submissions WHERE Exam_Sub_ID = @subid;";
                SqlCommand cmdtemp = new SqlCommand(querytemp, con);
                cmdtemp.Parameters.AddWithValue("@subid", subid);
                p_email = (string)cmdtemp.ExecuteScalar();



                string query = "INSERT INTO Performance_Review VALUES (@rating, @subid, @semail, @pemail)";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@subid", subid);
                cmd.Parameters.AddWithValue("@rating", rating);
                cmd.Parameters.AddWithValue("@semail", sh_email);
                cmd.Parameters.AddWithValue("@pemail", p_email);
                cmd.ExecuteNonQuery();
              }
            catch
            {

            }
            finally
            {
                con.Close();
            }
        }
          //////////////

        public DataTable getallsessions()
        {
            DataTable dt = new DataTable();
            try
            {
                con.Open();
                string query = "SELECT * FROM Sessions WHERE sheikh_email is null";
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

        public void Acceptrequest(string email, int sid)
        {
            try
            {
                con.Open();
                string query = "UPDATE Sessions SET sheikh_email = @email, status = 'Done' WHERE Session_ID = @id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@id", sid);
                cmd.ExecuteNonQuery();
            }
            catch
            {

            }
            finally
            {
                con.Close();
            }
        }

        public void Denyrequest(int sid)
        {
            try
            {
                con.Open();
                string query = "Delete Sessions WHERE Session_ID = @id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", sid);
                cmd.ExecuteNonQuery();
            }
            catch
            {

            }
            finally
            {
                con.Close();
            }
        }

        public DataTable getsessionsrequestedbyuser(string email)
        {
            DataTable dt = new DataTable();
            try
            {
                con.Open();
                string query = "SELECT * FROM Sessions WHERE participant_email = @email";
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
    }
}
