namespace SkyBox.API.Services;

public interface ITrashReminderService
{
    Task<int> SendDeletionRemindersAsync(CancellationToken cancellationToken = default);
}
