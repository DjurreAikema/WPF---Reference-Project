using System.Collections.ObjectModel;

namespace WpfApp2.Shared.Debugging;

public class ViewModelTracker
{
    // Singleton instance
    private static readonly Lazy<ViewModelTracker> _instance = new(() => new ViewModelTracker());
    public static ViewModelTracker Instance => _instance.Value;

    // Dictionary to track view model statistics by type
    private readonly Dictionary<Type, ViewModelStats> _stats = new();

    // Enable/disable tracking
    public bool IsEnabled { get; set; } = true;

    // Private constructor to enforce singleton pattern
    private ViewModelTracker()
    {
    }

    public void RegisterViewModel(object viewModel)
    {
        if (!IsEnabled) return;

        var type = viewModel.GetType();

        if (!_stats.TryGetValue(type, out var stats))
        {
            stats = new ViewModelStats {TypeName = type.Name};
            _stats[type] = stats;
        }

        stats.CurrentCount++;
        stats.TotalCreated++;
    }

    public void UnregisterViewModel(object viewModel)
    {
        if (!IsEnabled) return;

        var type = viewModel.GetType();

        if (_stats.TryGetValue(type, out var stats))
        {
            stats.CurrentCount--;
            stats.TotalClosed++;
        }
    }

    public ObservableCollection<ViewModelStats> GetAllStats()
    {
        return new ObservableCollection<ViewModelStats>(_stats.Values.OrderBy(s => s.TypeName));
    }

    public class ViewModelStats
    {
        public string TypeName { get; set; } = string.Empty;
        public int CurrentCount { get; set; }
        public int TotalCreated { get; set; }
        public int TotalClosed { get; set; }
    }
}