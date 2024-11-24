using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace WpfApp1.Windows.WindowSix.Shared.UI;

public partial class NotificationControl
{
    // --- Dependency Properties
    public static readonly DependencyProperty NotificationsObsProperty = DependencyProperty.Register(
        nameof(NotificationsObs), typeof(IObservable<string>), typeof(NotificationControl),
        new PropertyMetadata(null, (d, _) =>
        {
            if (d is not NotificationControl c) return;
            c.Disposables.Add(c.NotificationsObs.Subscribe(message => { c.ShowNotification(message); }));
        }));

    public IObservable<string> NotificationsObs
    {
        get => (IObservable<string>) GetValue(NotificationsObsProperty);
        set => SetValue(NotificationsObsProperty, value);
    }

    // --- Constructor
    public NotificationControl()
    {
        InitializeComponent();
    }

    private void ShowNotification(string message)
    {
        var textBlock = new TextBlock
        {
            Text = message,
            Background = new SolidColorBrush(Color.FromRgb(240, 240, 240)),
            Margin = new Thickness(0, 0, 0, 5),
            Padding = new Thickness(10),
            Opacity = 0
        };

        NotificationPanel.Children.Insert(0, textBlock);

        // Fade-in animation
        var fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.3));
        textBlock.BeginAnimation(OpacityProperty, fadeIn);

        // Timer to start fade-out animation after x seconds
        var timer = new System.Windows.Threading.DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(3)
        };
        timer.Tick += (_, _) =>
        {
            timer.Stop();
            var fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(0.5));
            fadeOut.Completed += (_, _) => NotificationPanel.Children.Remove(textBlock);
            textBlock.BeginAnimation(OpacityProperty, fadeOut);
        };
        timer.Start();
    }
}