public class AppState
{
    private string _mode = "customer";

    public string Mode => _mode;

    public event Action? OnChange;

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