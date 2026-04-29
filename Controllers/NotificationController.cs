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

    [HttpPost("Send"),Obsolete]
    
    public async Task<IActionResult> SendNotification([FromBody] NotificationRequest request)
    {
        var (success, message) = await _notificationService.SendAndSaveNotificationAsync(
            fcmToken: request.Token,
            title: request.Title,
            body: request.Message,
            notificationTypeId: request.NotificationTypeId,
            userId: request.UserId,
            data: request.Data 
        );

        return success ? Ok(new { success, message }) : BadRequest(new { success, message });
    }
}