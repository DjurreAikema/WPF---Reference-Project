using System.Windows;
using System.Windows.Controls;

namespace WpfApp1.Shared.Ui;

public partial class LoadingSpinner : UserControl
{
    public static readonly DependencyProperty IsLoadingProperty =
        DependencyProperty.Register(nameof(IsLoading), typeof(bool), typeof(LoadingSpinner), new PropertyMetadata(false));

    public bool IsLoading
    {
        get => (bool) GetValue(IsLoadingProperty);
        set => SetValue(IsLoadingProperty, value);
    }

    public LoadingSpinner()
    {
        InitializeComponent();
        DataContext = this;
    }
}