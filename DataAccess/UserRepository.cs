using Microsoft.Data.SqlClient;
using Noots.Models;
using Noots.Utilities;
using System;
using System.Data;
using System.Text.RegularExpressions;

namespace Noots.DataAccess
{
    public class UserRepository
    {
        // Check if username or email already exists (separately)
        private (bool UsernameExists, bool EmailExists) CheckUserExists(string username, string email)
        {
            using (SqlConnection conn = DatabaseManager.GetConnection())
            {
                string query = @"
                    SELECT 
                        SUM(CASE WHEN Username = @Username THEN 1 ELSE 0 END),
                        SUM(CASE WHEN Email = @Email THEN 1 ELSE 0 END)
                    FROM Users";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 50).Value = username;
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = email;

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            bool usernameExists = reader.IsDBNull(0) ? false : reader.GetInt32(0) > 0;
                            bool emailExists = reader.IsDBNull(1) ? false : reader.GetInt32(1) > 0;

                            return (usernameExists, emailExists);
                        }
                    }
                }
            }

            return (false, false);
        }

        // Register new user with full validation
        public (bool Success, string Message) RegisterUser(string username, string email, string password, string fullName)
        {
            // 1. Required fields validation
            if (string.IsNullOrWhiteSpace(username))
                return (false, "Username cannot be empty.");

            if (string.IsNullOrWhiteSpace(email))
                return (false, "Email cannot be empty.");

            if (string.IsNullOrWhiteSpace(password))
                return (false, "Password cannot be empty.");

            if (string.IsNullOrWhiteSpace(fullName))
                return (false, "Full name cannot be empty.");

            // 2. Username validation
            if (username.Length < 3 || username.Length > 30)
                return (false, "Username must be between 3 and 30 characters.");

            if (!Regex.IsMatch(username, @"^[a-zA-Z0-9_]+$"))
                return (false, "Username can only contain letters, numbers, and underscore (_).");

            // 3. Email validation
            if (email.Length > 100)
                return (false, "Email cannot exceed 100 characters.");

            if (!Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
                return (false, "Invalid email format. Example: user@example.com");

            // 4. Password validation
            if (password.Length < 8)
                return (false, "Password must be at least 8 characters long.");

            if (password.Length > 50)
                return (false, "Password cannot exceed 50 characters.");

            if (!Regex.IsMatch(password, @"[A-Z]"))
                return (false, "Password must contain at least one uppercase letter (A-Z).");

            if (!Regex.IsMatch(password, @"[a-z]"))
                return (false, "Password must contain at least one lowercase letter (a-z).");

            if (!Regex.IsMatch(password, @"[0-9]"))
                return (false, "Password must contain at least one number (0-9).");

            // 5. Full name validation
            if (fullName.Length < 2 || fullName.Length > 100)
                return (false, "Full name must be between 2 and 100 characters.");

            if (!Regex.IsMatch(fullName, @"^[\u0600-\u06FF\sa-zA-Z]+$"))
                return (false, "Full name can only contain Persian/English letters and spaces.");

            // 6. Check duplicates
            var exists = CheckUserExists(username, email);

            if (exists.UsernameExists)
                return (false, "Username already exists.");

            if (exists.EmailExists)
                return (false, "Email is already registered.");

            // 7. Hash password
            string hashedPassword = PasswordHelper.HashPassword(password);
            Guid newUserId = Guid.NewGuid();

            // 8. Insert into database
            using (SqlConnection conn = DatabaseManager.GetConnection())
            {
                string query = @"
                    INSERT INTO Users (Id, Username, Email, PasswordHash, FullName, CreatedAt)
                    VALUES (@Id, @Username, @Email, @PasswordHash, @FullName, @CreatedAt)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.UniqueIdentifier).Value = newUserId;
                    cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 50).Value = username;
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 100).Value = email;
                    cmd.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, -1).Value = hashedPassword;
                    cmd.Parameters.Add("@FullName", SqlDbType.NVarChar, 100).Value = fullName;
                    cmd.Parameters.Add("@CreatedAt", SqlDbType.DateTime).Value = DateTime.UtcNow;

                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                        return (true, "Registration completed successfully.");
                    else
                        return (false, "An error occurred while saving data to the database.");
                }
            }
        }

        // Login user (by username or email)
        public (bool Success, string Message, User LoggedInUser) LoginUser(string usernameOrEmail, string password)
        {
            if (string.IsNullOrWhiteSpace(usernameOrEmail) || string.IsNullOrWhiteSpace(password))
                return (false, "Username/email and password are required.", null);

            User user = null;

            using (SqlConnection conn = DatabaseManager.GetConnection())
            {
                string query = @"
                    SELECT Id, Username, Email, PasswordHash, FullName, CreatedAt
                    FROM Users
                    WHERE Username = @Input OR Email = @Input";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@Input", SqlDbType.NVarChar, 100).Value = usernameOrEmail;

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new User
                            {
                                Id = reader.GetGuid(0),
                                Username = reader.GetString(1),
                                Email = reader.GetString(2),
                                PasswordHash = reader.GetString(3),
                                FullName = reader.GetString(4),
                                CreatedAt = reader.GetDateTime(5)
                            };
                        }
                    }
                }
            }

            // Unified error message for security
            if (user == null || !PasswordHelper.VerifyPassword(password, user.PasswordHash))
                return (false, "Invalid username/email or password.", null);

            return (true, "Login successful.", user);
        }
        public User GetUserById(Guid userId)
        {
            using (SqlConnection conn = DatabaseManager.GetConnection())
            {
                string query = @"
            SELECT Id, Username, Email, PasswordHash, FullName, CreatedAt
            FROM Users
            WHERE Id = @UserId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.Add("@UserId", SqlDbType.UniqueIdentifier).Value = userId;

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Id = reader.GetGuid(0),
                                Username = reader.GetString(1),
                                Email = reader.GetString(2),
                                PasswordHash = reader.GetString(3),
                                FullName = reader.GetString(4),
                                CreatedAt = reader.GetDateTime(5)
                            };
                        }
                    }
                }
            }

            return null;
        }
    }
}
