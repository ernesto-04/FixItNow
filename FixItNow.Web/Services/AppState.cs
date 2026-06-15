using FixItNow.Domain.Models.BookingRequest.DTOs;

public class AppState
{
    public const string CustomerMode = "customer";
    public const string TechnicianMode = "technician";

    private string _mode = CustomerMode;
    public string Mode => _mode;

    public event Action? OnChange;

    public bool IsTechnicianOnline { get; set; }

    public int UnreadNotificationCount { get; private set; }
    public List<NotificationDto> UnreadNotifications { get; private set; } = new();

    public void SetNotifications(List<NotificationDto> notifications)
    {
        UnreadNotifications = notifications;
        UnreadNotificationCount = notifications.Count;
        NotifyStateChanged();
    }

    public void AddNotification(NotificationDto notification)
    {
        UnreadNotifications.Insert(0, notification);
        UnreadNotificationCount++;
        NotifyStateChanged();
    }

    public void ClearNotifications()
    {
        UnreadNotifications.Clear();
        UnreadNotificationCount = 0;
        NotifyStateChanged();
    }

    public void SetOnlineStatus(bool isOnline)
    {
        IsTechnicianOnline = isOnline;
        NotifyStateChanged();
    }

    public void SetMode(string mode)
    {
        if (_mode != mode)
        {
            _mode = mode;
            NotifyStateChanged();
        }
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}