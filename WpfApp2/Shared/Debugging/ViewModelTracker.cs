using System.Collections.ObjectModel;

namespace WpfApp2.Shared.Debugging;

public class ViewModelTracker
{
    // Singleton instance
    private static readonly Lazy<ViewModelTracker> _instance = new(() => new ViewModelTracker());
    public static ViewModelTracker Instance => _instance.Value;

    // Dictionary to track view model statistics by type
    private readonly Dictionary<Type, ViewModelStats> _stats = new();

    // List to track individual instances
    private readonly List<ViewModelInstance> _instances = [];

    // Counter dictionary for generating unique IDs
    private readonly Dictionary<string, int> _idCounters = new();

    // Enable/disable tracking
    public bool IsEnabled { get; set; } = true;

    // Private constructor to enforce singleton pattern
    private ViewModelTracker()
    {
    }

    public string RegisterViewModel(object viewModel, bool isSingleton = false)
    {
        if (!IsEnabled) return string.Empty;

        var type = viewModel.GetType();
        var typeName = type.Name;

        // Generate a unique ID
        if (!_idCounters.TryGetValue(typeName, out var counter))
        {
            counter = 0;
            _idCounters[typeName] = counter;
        }

        _idCounters[typeName] = counter + 1;
        var id = isSingleton ? $"{typeName}-Singleton-{counter + 1}" : $"{typeName}-{counter + 1}";

        // Track the instance
        var instance = new ViewModelInstance
        {
            Id = id,
            TypeName = typeName,
            CreatedAt = DateTime.Now,
            IsSingleton = isSingleton
        };

        _instances.Add(instance);

        // Update type statistics
        if (!_stats.TryGetValue(type, out var stats))
        {
            stats = new ViewModelStats {TypeName = type.Name};
            _stats[type] = stats;
        }

        stats.CurrentCount++;
        stats.TotalCreated++;

        return id;
    }

    public void UnregisterViewModel(object viewModel, string id)
    {
        if (!IsEnabled || string.IsNullOrEmpty(id)) return;

        var type = viewModel.GetType();

        // Find the instance and update its disposal time
        var instance = _instances.FirstOrDefault(i => i.Id == id);
        if (instance != null) instance.DisposedAt = DateTime.Now;

        // Update statistics
        if (!_stats.TryGetValue(type, out var stats)) return;
        stats.CurrentCount--;
        stats.TotalClosed++;
    }

    public ObservableCollection<ViewModelStats> GetAllStats() =>
        new(_stats.Values.OrderBy(s => s.TypeName));

    public ObservableCollection<ViewModelInstance> GetAllInstances() =>
        new(_instances.OrderByDescending(i => i.CreatedAt));

    public class ViewModelStats
    {
        public string TypeName { get; set; } = string.Empty;
        public int CurrentCount { get; set; }
        public int TotalCreated { get; set; }
        public int TotalClosed { get; set; }
    }

    public class ViewModelInstance
    {
        public string Id { get; set; } = string.Empty;
        public string TypeName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? DisposedAt { get; set; }
        public bool IsSingleton { get; set; }

        public string Status => DisposedAt.HasValue ? "Disposed" : "Active";

        public string Duration => DisposedAt.HasValue
            ? (DisposedAt.Value - CreatedAt).ToString(@"hh\:mm\:ss")
            : (DateTime.Now - CreatedAt).ToString(@"hh\:mm\:ss");
    }
}