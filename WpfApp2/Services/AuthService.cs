using System.Linq;
using BrickByBrick.ViewModels;

namespace BrickByBrick.Services
{
    /// <summary>
    /// Simple credential check against the shared UserStore.
    /// UI-only / dummy-data stage: plain-text comparison, no hashing,
    /// no session/token management. Replace with real authentication
    /// before this app handles real accounts.
    /// </summary>
    public static class AuthService
    {
        /// <summary>
        /// Attempts to find a user matching the given email and password.
        /// Returns the matching UserListItem, or null if no match was found
        /// or the account is inactive.
        /// </summary>
        public static UserListItem? TryLogin(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            var match = UserStore.Instance.Users.FirstOrDefault(u =>
                u.Email.Equals(email.Trim(), System.StringComparison.OrdinalIgnoreCase) &&
                u.Password == password);

            if (match == null || !match.IsActive)
            {
                return null;
            }

            return match;
        }
    }
}
