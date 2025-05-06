using System.Runtime.CompilerServices;

namespace WpfApp2.Shared.Debugging.Extensions;

/// <summary>
/// Extension methods to help with view model registration
/// </summary>
public static class ViewModelRegistrationExtensions
{
    // Store the IDs of registered view models
    private static readonly ConditionalWeakTable<object, string> VmIds = new();

    /// <summary>
    /// Registers a view model with the ViewModelTracker
    /// </summary>
    public static T RegisterWithTracker<T>(this T viewModel, bool isSingleton = false) where T : class
    {
        if (VmIds.TryGetValue(viewModel, out var existingId)) return viewModel;

        var id = ViewModelTracker.Instance.RegisterViewModel(viewModel, isSingleton);
        if (!string.IsNullOrEmpty(id)) VmIds.Add(viewModel, id);

        return viewModel;
    }

    /// <summary>
    /// Unregisters a view model from the ViewModelTracker
    /// </summary>
    public static void UnregisterFromTracker(this object viewModel)
    {
        if (VmIds.TryGetValue(viewModel, out var id))
            ViewModelTracker.Instance.UnregisterViewModel(viewModel, id);
    }
}