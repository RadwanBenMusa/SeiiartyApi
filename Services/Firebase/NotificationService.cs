using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Seiiarty;
using Seiiarty.StoredProcedures;
using System.Text.Json;

namespace SeiiartyApi.Services.Firebase
{
    public class NotificationService
    {
        [Obsolete]
        public async Task<(bool Success, string Message)> SendAndSaveNotificationAsync( string title, string body, int notificationTypeId, int userId, object? data = null ,string ? fcmToken = null)
        {
            bool sent = false;
            Dictionary<string, string> firebaseData = MapToFirebaseData(data, notificationTypeId);

    
            switch (notificationTypeId)
            {
                case 1:
                    sent = await SendToAdmins(title, body, firebaseData);
                    break;
                case 2:
                    sent = await SendToStores(title, body, firebaseData);
                    break;
                case 3:
                    sent = await SendToAdmins(title, body, firebaseData);
                    break;
                default:
                    if (!string.IsNullOrEmpty(fcmToken))
                    {
                        // Wrap single token in a list to keep the main function consistent
                        sent = await SendNotificationAsync(new List<string> { fcmToken }, title, body, firebaseData);
                    }
                    break;
            }

            if (sent)
            {
                try
                {
                    string dbDataJson = JsonSerializer.Serialize(firebaseData);
                    SpNotification.Insert(new InsNotification
                    {
                        NotificationTypeId = notificationTypeId,
                        UserId = userId,
                        Title = title,
                        Body = body,
                        Data = dbDataJson
                    });

                    return (true, "Notification processed successfully");
                }
                catch (Exception ex)
                {
                    return (true, $"Sent but failed to save to DB: {ex.Message}");
                }
            }

            return (false, "Failed to send notification.");
        }

        [Obsolete]
        private async Task<bool> SendToAdmins(string title, string body, Dictionary<string, string> data)
        {
            dynamic users = SpUser.Get(new GetUser { Admin = true });
            List<string> tokens = GetTokensFromUsers(users);
            // Return the result and use await instead of .Wait()
            return await SendNotificationToMultipleTokensAsync(tokens, title, body, data);
        }

        [Obsolete]
        private async Task<bool> SendToStores(string title, string body, Dictionary<string, string> data)
        {
            dynamic users = SpUser.Get(new GetUser { HasStore = true });
            List<string> tokens = GetTokensFromUsers(users);
            return await SendNotificationToMultipleTokensAsync(tokens, title, body, data);
        }
        private async Task<bool> SendNotificationToMultipleTokensAsync(List<string> tokens, string title, string body, Dictionary<string, string> data)
        {
            // Just pass everything to the main function
            return await SendNotificationAsync(tokens, title, body, data);
        }

        // --- THE MAIN FUNCTION ---
        private async Task<bool> SendNotificationAsync(List<string> tokens, string title, string body, Dictionary<string, string> data)
        {
            try
            {
                var distinctTokens = tokens.Where(t => !string.IsNullOrWhiteSpace(t)).Distinct().ToList();

                if (!distinctTokens.Any()) return false;

                var message = new MulticastMessage()
                {
                    Tokens = distinctTokens,
                    Notification = new Notification { Title = title, Body = body },
                    Data = data
                };

                // Using the most reliable V1 HTTP method
                var response = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message);

                if (response.FailureCount > 0)
                {
                    foreach (var res in response.Responses.Where(r => !r.IsSuccess))
                    {
                        Console.WriteLine($"FCM individual failure: {res.Exception.Message}");
                    }
                }

                return response.SuccessCount > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CRITICAL FCM ERROR: {ex}");
                return false;
            }
        }

        public Dictionary<string, string> MapToFirebaseData(object? data, int typeId)
        {
            var dict = new Dictionary<string, string> { ["Type"] = typeId.ToString() };
            if (data != null)
            {
                try
                {
                    var json = JsonSerializer.Serialize(data);
                    var tempDict = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
                    if (tempDict != null)
                    {
                        foreach (var item in tempDict)
                        {
                            if (item.Key != "Type")
                                dict[item.Key] = item.Value?.ToString() ?? "";
                        }
                    }
                }
                catch { }
            }
            return dict;
        }

        private List<string> GetTokensFromUsers(dynamic users)
        {
            var tokens = new List<string>();
            if (General.IfEmptyOrNull(users) == null) return tokens;

            try
            {
                for (int i = 0; i < users.Rows.Count; i++)
                {
                    string token = General.GetValue<string>(users.Rows[i], "FirebaseToken");
                    token = token?.Trim();
                    if (!string.IsNullOrEmpty(token) && token.Length > 60)
                    {
                        tokens.Add(token);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token Extraction Error: {ex.Message}");
            }
            return tokens;
        }
    }
}