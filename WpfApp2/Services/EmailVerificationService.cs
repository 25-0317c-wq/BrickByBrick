using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BrickByBrick.Services
{
    /// <summary>
    /// Verifies whether an email address is real/deliverable using Hunter.io's
    /// email-verifier endpoint, before a new account is created.
    ///
    /// SECURITY NOTE: the API key below is a placeholder. Hardcoding a real
    /// key into a compiled desktop app means anyone with the .exe can extract
    /// it with basic tools — this is acceptable only for local testing/coursework.
    /// For anything beyond that, this call should go through your own backend
    /// server, which holds the real key and the desktop app never sees it.
    /// If a real key was ever pasted anywhere (chat, screenshots, committed
    /// code), regenerate it in the Hunter.io dashboard — a key that has been
    /// shared anywhere outside your own machine should be treated as
    /// compromised, even if nothing bad has happened yet.
    /// </summary>
    public static class EmailVerificationService
    {
        // Placeholder text — change ONLY this string to your real key.
        // Do not change the comparison further down; it must keep checking
        // against this exact placeholder text, not against the key itself.
        private const string ApiKeyPlaceholder = "a16ae15a5fee80508f8b3ee8d8d1903cbbb8b267";

        private const string ApiKey = ApiKeyPlaceholder;

        private static readonly HttpClient _httpClient = new HttpClient();

        public class EmailVerificationResult
        {
            public bool IsValid { get; set; }
            public string Status { get; set; } = string.Empty;
            public string Message { get; set; } = string.Empty;
        }

        // Matches Hunter.io's email-verifier response shape:
        // { "data": { "status": "valid" | "invalid" | "disposable" | ... } }
        private class HunterApiResponse
        {
            public HunterApiData? Data { get; set; }
        }

        private class HunterApiData
        {
            public string Status { get; set; } = string.Empty;
        }

        /// <summary>
        /// Checks an email address against Hunter.io. Returns IsValid = true
        /// only for a "valid" status; "invalid", "disposable", "unknown", etc.
        /// are all treated as not valid for account creation purposes.
        /// Never throws — network/API failures come back as a failed result
        /// with an explanatory Message instead.
        /// </summary>
        public static async Task<EmailVerificationResult> VerifyAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            {
                return new EmailVerificationResult
                {
                    IsValid = false,
                    Message = "Enter a valid-looking email address first."
                };
            }

            // This correctly checks against the PLACEHOLDER text, not against
            // whatever real key you've set above — so it only blocks
            // verification when nobody has configured a key yet.
            if (ApiKey == ApiKeyPlaceholder)
            {
                return new EmailVerificationResult
                {
                    IsValid = false,
                    Message = "Email verification isn't configured yet (missing API key)."
                };
            }

            string url = $"https://api.hunter.io/v2/email-verifier?email={Uri.EscapeDataString(email)}&api_key={ApiKey}";

            try
            {
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    return new EmailVerificationResult
                    {
                        IsValid = false,
                        Message = $"Email verification service returned an error ({(int)response.StatusCode})."
                    };
                }

                string json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<HunterApiResponse>(json);

                string status = result?.Data?.Status ?? "unknown";

                return new EmailVerificationResult
                {
                    IsValid = status == "valid",
                    Status = status,
                    Message = status == "valid"
                        ? "Email looks good."
                        : $"This email address looks {status} — double-check it before continuing."
                };
            }
            catch (Exception ex)
            {
                return new EmailVerificationResult
                {
                    IsValid = false,
                    Message = $"Couldn't verify email right now: {ex.Message}"
                };
            }
        }
    }
}
