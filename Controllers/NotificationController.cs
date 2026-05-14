using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Seiiarty.clsMod;
using SeiiartyApi.Services.Firebase;

[ApiController]
[Route("[controller]")]
public class NotificationController : ControllerBase
{
    private readonly NotificationService _notificationService;

    public NotificationController(NotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    // ── Send ──────────────────────────────────────────────────────────────────
    // The app sends: Title, Message, NotificationTypeId, UserId, Data (optional)
    // The API reads FCM tokens from the DB and sends via Firebase Admin SDK.
    // The app never knows about FCM tokens.
    [HttpPost("Send"), Authorize]
    [Obsolete]
    public async Task<IActionResult> SendNotification([FromBody] NotificationRequest request)
    {
        var (success, message) = await _notificationService.SendAndSaveNotificationAsync(
            title: request.Title,
            body: request.Message,
            notificationTypeId: request.NotificationTypeId,
            userId: request.UserId,
            data: request.Data
        // fcmToken is NOT passed — API fetches the right tokens from DB
        );

        return success
            ? Ok(new { success, message })
            : BadRequest(new { success, message });
    }
}