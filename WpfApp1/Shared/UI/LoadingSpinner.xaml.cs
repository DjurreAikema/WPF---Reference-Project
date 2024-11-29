using System.Windows;

namespace WpfApp1.Shared.UI;

public partial class LoadingSpinner
{
    public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register(
        nameof(IsLoading), typeof(bool), typeof(LoadingSpinner),
        new PropertyMetadata(null));

    public bool IsLoading
    {
        get => (bool) GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public LoadingSpinner()
    {
        InitializeComponent();
    }
}