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
            connectionstring = "Data Source=MAZEN\\SQLEXPRESS;Initial Catalog=MemorizationSystem;Integrated Security=True;";
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

                Console.WriteLine(cmd.ToString());

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
                Console.WriteLine(email);
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

            }
            finally
            {
                con.Close();
            }
            return status;
        }
    }
}
