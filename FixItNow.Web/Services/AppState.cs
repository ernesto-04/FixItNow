public class AppState
{
    public const string CustomerMode = "customer";
    public const string TechnicianMode = "technician";

    private string _mode = CustomerMode;
    public string Mode => _mode;

    public event Action? OnChange;

    public bool IsTechnicianOnline { get; set; }

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