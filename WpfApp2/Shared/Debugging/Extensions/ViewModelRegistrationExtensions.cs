namespace WpfApp2.Shared.Debugging.Extensions;

/// <summary>
/// Extension methods to help with view model registration
/// </summary>
public static class ViewModelRegistrationExtensions
{
    /// <summary>
    /// Registers a view model with the ViewModelTracker
    /// </summary>
    public static T RegisterWithTracker<T>(this T viewModel) where T : class
    {
        ViewModelTracker.Instance.RegisterViewModel(viewModel);
        return viewModel;
    }

    /// <summary>
    /// Unregisters a view model from the ViewModelTracker
    /// </summary>
    public static void UnregisterFromTracker(this object viewModel)
    {
        ViewModelTracker.Instance.UnregisterViewModel(viewModel);
    }
}