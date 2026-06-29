using System;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using BrickByBrick.Models;
using BrickByBrick.ViewModels;

namespace BrickByBrick.Services
{
    /// <summary>
    /// Single shared source of truth for all registered users and their roles.
    /// Now backed by SQL Server instead of in-memory dummy data — Users is
    /// loaded from the Users table once at startup, and changes (role edits,
    /// new accounts) are written back to the database immediately.
    /// </summary>
    public sealed class UserStore
    {
        private static readonly UserStore _instance = new UserStore();
        public static UserStore Instance => _instance;

        public ObservableCollection<UserListItem> Users { get; }

        /// <summary>Maps each loaded UserListItem to its database row Id, so updates know which row to write to.</summary>
        private readonly System.Collections.Generic.Dictionary<UserListItem, int> _userIds = new();

        private UserStore()
        {
            Users = new ObservableCollection<UserListItem>();
            LoadFromDatabase();

            // Persist role/active-status changes back to the database whenever
            // they happen anywhere in the app (e.g. Admin's role dropdown).
            foreach (var user in Users)
            {
                AttachSaveOnChange(user);
            }
        }

        private void LoadFromDatabase()
        {
            Users.Clear();
            _userIds.Clear();

            using var connection = new SqlConnection(DatabaseConfig.ConnectionString);
            connection.Open();

            const string sql = @"
                SELECT Id, FullName, Email, Department, Role, IsActive
                FROM Users
                ORDER BY FullName;";

            using var command = new SqlCommand(sql, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                int id = reader.GetInt32(reader.GetOrdinal("Id"));
                string fullName = reader.GetString(reader.GetOrdinal("FullName"));
                string email = reader.GetString(reader.GetOrdinal("Email"));
                string department = reader.IsDBNull(reader.GetOrdinal("Department")) ? string.Empty : reader.GetString(reader.GetOrdinal("Department"));
                string role = reader.GetString(reader.GetOrdinal("Role"));
                bool isActive = reader.GetBoolean(reader.GetOrdinal("IsActive"));

                var user = new UserListItem
                {
                    FullName = fullName,
                    Email = email,
                    Department = department,
                    Role = Enum.Parse<UserRole>(role),
                    IsActive = isActive,
                    InitialsBadge = BuildInitials(fullName)
                    // Password is intentionally NOT loaded here — login checks
                    // the hash directly via AuthService, never via this object.
                };

                Users.Add(user);
                _userIds[user] = id;
            }
        }

        private void AttachSaveOnChange(UserListItem user)
        {
            user.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(UserListItem.Role) || e.PropertyName == nameof(UserListItem.IsActive))
                {
                    SaveUserToDatabase(user);
                }
            };
        }

        private void SaveUserToDatabase(UserListItem user)
        {
            if (!_userIds.TryGetValue(user, out int id)) return;

            using var connection = new SqlConnection(DatabaseConfig.ConnectionString);
            connection.Open();

            const string sql = @"
                UPDATE Users
                SET Role = @Role, IsActive = @IsActive
                WHERE Id = @Id;";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Role", user.Role.ToString());
            command.Parameters.AddWithValue("@IsActive", user.IsActive);
            command.Parameters.AddWithValue("@Id", id);
            command.ExecuteNonQuery();
        }

        /// <summary>
        /// Adds a brand-new user to both the database and the in-memory
        /// collection. The password should already be a BCrypt hash by the
        /// time it reaches here — see AuthService for hashing on creation.
        /// </summary>
        public void AddUser(UserListItem user, string passwordHash)
        {
            using var connection = new SqlConnection(DatabaseConfig.ConnectionString);
            connection.Open();

            const string sql = @"
                INSERT INTO Users (FullName, Email, PasswordHash, Department, Role, IsActive)
                OUTPUT INSERTED.Id
                VALUES (@FullName, @Email, @PasswordHash, @Department, @Role, @IsActive);";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@FullName", user.FullName);
            command.Parameters.AddWithValue("@Email", user.Email);
            command.Parameters.AddWithValue("@PasswordHash", passwordHash);
            command.Parameters.AddWithValue("@Department", string.IsNullOrEmpty(user.Department) ? (object)DBNull.Value : user.Department);
            command.Parameters.AddWithValue("@Role", user.Role.ToString());
            command.Parameters.AddWithValue("@IsActive", user.IsActive);

            int newId = (int)command.ExecuteScalar();

            Users.Add(user);
            _userIds[user] = newId;
            AttachSaveOnChange(user);
        }

        /// <summary>
        /// Looks up the stored password hash for a given email, for login
        /// verification in AuthService. Returns null if no active user has
        /// that email.
        /// </summary>
        public string? GetPasswordHashForEmail(string email)
        {
            using var connection = new SqlConnection(DatabaseConfig.ConnectionString);
            connection.Open();

            const string sql = @"
                SELECT PasswordHash FROM Users
                WHERE Email = @Email AND IsActive = 1;";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Email", email.Trim());

            var result = command.ExecuteScalar();
            return result as string;
        }

        private static string BuildInitials(string fullName)
        {
            var parts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return "?";
            if (parts.Length == 1) return parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpperInvariant();
            return $"{parts[0][0]}{parts[^1][0]}".ToUpperInvariant();
        }
    }
}
