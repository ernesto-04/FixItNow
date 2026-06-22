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

    public bool IsTechnicianPending { get; private set; }
    public bool IsTechnicianApproved { get; private set; }
    public bool IsTechnicianRejected { get; private set; }

    private bool _showTechnicianPanel;
    public bool ShowTechnicianPanel
    {
        get => _showTechnicianPanel;
        set
        {
            _showTechnicianPanel = value;
            NotifyStateChanged();
        }
    }

    public bool IsAdmin { get; private set; }

    public void SetAdminStatus(bool isAdmin)
    {
        IsAdmin = isAdmin;
        NotifyStateChanged();
    }

    public void OpenTechnicianPanel()
    {
        ShowTechnicianPanel = true;
        NotifyStateChanged();
    }

    public void SetTechnicianStatus(bool exists, bool isApproved, bool isRejected = false)
    {
        IsTechnicianApproved = exists && isApproved;
        IsTechnicianRejected = exists && isRejected;
        IsTechnicianPending = exists && !isApproved && !isRejected;
        NotifyStateChanged();
    }

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

    public void Reset()
    {
        _mode = CustomerMode;
        IsAdmin = false;
        IsTechnicianOnline = false;
        IsTechnicianPending = false;
        IsTechnicianApproved = false;
        IsTechnicianRejected = false;
        _showTechnicianPanel = false;
        UnreadNotifications = new();
        UnreadNotificationCount = 0;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}