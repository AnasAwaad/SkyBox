using Microsoft.EntityFrameworkCore;
using SkyBox.API.Persistence;
using SkyBox.API.Settings;

namespace SkyBox.API.Services;

public class TrashReminderService(ApplicationDbContext dbContext):ITrashReminderService
{

    public async Task<int> SendDeletionRemindersAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var threshold = now.AddDays(2); 

        var files = await dbContext.Files
            .Where(f => f.DeletedAt != null &&
                        f.DeletedAt.Value.AddDays(FileSettings.TrashRetentionDays) <= threshold &&
                        f.DeletedAt.Value.AddDays(FileSettings.TrashRetentionDays) > now)
            .ToListAsync(cancellationToken);


        //TODO: Implement notification service to notify users about upcoming permanent deletion


        //foreach (var file in files)
        //{
        //    await _notificationService.NotifyUserAsync(file.OwnerId, new NotificationMessage
        //    {
        //        Type = "trash-expiry",
        //        Message = $"Your file '{file.FileName}' will be permanently deleted soon.",
        //        FileId = file.Id
        //    });
        //}

        return files.Count;
    }

}
