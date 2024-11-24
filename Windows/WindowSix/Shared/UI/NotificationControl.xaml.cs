using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using WpfApp1.Classes;

namespace WpfApp1.Windows.WindowSix.Shared.UI;

public partial class NotificationControl
{
    // --- Dependency Properties
    public static readonly DependencyProperty NotificationsObsProperty = DependencyProperty.Register(
        nameof(NotificationsObs), typeof(IObservable<NotificationMessage>), typeof(NotificationControl),
        new PropertyMetadata(null, (d, _) =>
        {
            if (d is not NotificationControl c) return;
            c.Disposables.Add(c.NotificationsObs.Subscribe(message => { c.ShowNotification(message); }));
        }));

    public IObservable<NotificationMessage> NotificationsObs
    {
        get => (IObservable<NotificationMessage>) GetValue(NotificationsObsProperty);
        set => SetValue(NotificationsObsProperty, value);
    }

    // --- Constructor
    public NotificationControl()
    {
        InitializeComponent();
    }

    private void ShowNotification(NotificationMessage message)
    {
        var textBlock = new TextBlock
        {
            Text = message.Text,
            Background = message.IsSuccess
                ? new SolidColorBrush(Color.FromRgb(173, 216, 230)) // Light Blue for success
                : new SolidColorBrush(Color.FromRgb(240, 128, 128)), // Light Coral for failure
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
            Interval = TimeSpan.FromSeconds(2)
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