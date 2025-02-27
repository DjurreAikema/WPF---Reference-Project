using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfApp1.Shared.Classes;
using WpfApp1.Shared.Locking.V1;

namespace WpfApp1.Windows.Window7._1.UI;

public partial class LockingControl
    {
        // --- Dependency Properties
        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
            nameof(SelectedItem), typeof(ILockable), typeof(LockingControl),
            new PropertyMetadata(null, OnSelectedItemChanged));

        public static readonly DependencyProperty LockServiceProperty = DependencyProperty.Register(
            nameof(LockService), typeof(ILockService), typeof(LockingControl),
            new PropertyMetadata(null));

        // --- Events
        public event EventHandler<LockResult> LockStatusChanged;

        // --- Properties
        public ILockable SelectedItem
        {
            get => (ILockable)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public ILockService LockService
        {
            get => (ILockService)GetValue(LockServiceProperty);
            set => SetValue(LockServiceProperty, value);
        }

        // --- Initialization
        public LockingControl()
        {
            InitializeComponent();
        }

        private static void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not LockingControl control) return;
            control.OnPropertyChanged(nameof(SelectedItem));
        }

        // --- Click Handlers
        private async void AcquireLock_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem == null || LockService == null) return;

            // Show lock reason dialog
            var dialog = new LockReasonDialog();
            if (dialog.ShowDialog() == true)
            {
                var result = await LockService.AcquireLockAsync(SelectedItem, dialog.LockReason);

                if (result.Success)
                {
                    // Refresh UI with updated item
                    SelectedItem = result.Entity;
                    OnPropertyChanged(nameof(SelectedItem));
                }

                // Notify listeners
                LockStatusChanged?.Invoke(this, result);
            }
        }

        private async void ExtendLock_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem == null || LockService == null) return;

            // Show dialog to select extension time
            var dialog = new LockExtensionDialog();
            if (dialog.ShowDialog() == true)
            {
                var result = await LockService.RefreshLockAsync(SelectedItem, dialog.ExtensionTime);

                if (result.Success)
                {
                    // Refresh UI with updated item
                    SelectedItem = result.Entity;
                    OnPropertyChanged(nameof(SelectedItem));
                }

                // Notify listeners
                LockStatusChanged?.Invoke(this, result);
            }
        }

        private async void ReleaseLock_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem == null || LockService == null) return;

            var result = await LockService.ReleaseLockAsync(SelectedItem);

            if (result.Success)
            {
                // Refresh UI with updated item
                SelectedItem = result.Entity;
                OnPropertyChanged(nameof(SelectedItem));
            }

            // Notify listeners
            LockStatusChanged?.Invoke(this, result);
        }

        private async void BreakLock_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedItem == null || LockService == null) return;

            // Confirm breaking another user's lock
            var message = $"Are you sure you want to break the lock currently held by {SelectedItem.LockedBy}?";
            if (MessageBox.Show(message, "Confirm Break Lock", MessageBoxButton.YesNo, MessageBoxImage.Warning)
                == MessageBoxResult.Yes)
            {
                // Show dialog to enter reason
                var dialog = new LockReasonDialog { Title = "Break Lock Reason" };
                if (dialog.ShowDialog() == true)
                {
                    var result = await LockService.BreakLockAsync(SelectedItem, dialog.LockReason);

                    if (result.Success)
                    {
                        // Refresh UI with updated item
                        SelectedItem = result.Entity;
                        OnPropertyChanged(nameof(SelectedItem));
                    }

                    // Notify listeners
                    LockStatusChanged?.Invoke(this, result);
                }
            }
        }
    }

    /// <summary>
    /// Dialog to enter a lock reason
    /// </summary>
    public class LockReasonDialog : Window
    {
        private System.Windows.Controls.TextBox _reasonTextBox;

        public string LockReason { get; private set; }

        public LockReasonDialog()
        {
            Title = "Lock Reason";
            Width = 350;
            Height = 150;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            var panel = new System.Windows.Controls.StackPanel { Margin = new Thickness(10) };

            panel.Children.Add(new System.Windows.Controls.TextBlock
            {
                Text = "Enter a reason for this lock (optional):",
                Margin = new Thickness(0, 0, 0, 5)
            });

            _reasonTextBox = new System.Windows.Controls.TextBox
            {
                Margin = new Thickness(0, 0, 0, 10),
                Height = 23
            };
            panel.Children.Add(_reasonTextBox);

            var buttonPanel = new System.Windows.Controls.StackPanel
            {
                Orientation = System.Windows.Controls.Orientation.Horizontal,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Right
            };

            var okButton = new System.Windows.Controls.Button
            {
                Content = "OK",
                IsDefault = true,
                Margin = new Thickness(0, 0, 5, 0),
                Padding = new Thickness(10, 3, 10, 3)
            };
            okButton.Click += (s, e) =>
            {
                LockReason = _reasonTextBox.Text;
                DialogResult = true;
                Close();
            };
            buttonPanel.Children.Add(okButton);

            var cancelButton = new System.Windows.Controls.Button
            {
                Content = "Cancel",
                IsCancel = true,
                Padding = new Thickness(10, 3, 10, 3)
            };
            buttonPanel.Children.Add(cancelButton);

            panel.Children.Add(buttonPanel);

            Content = panel;
        }
    }

    /// <summary>
    /// Dialog to select lock extension time
    /// </summary>
    public class LockExtensionDialog : Window
    {
        private System.Windows.Controls.ComboBox _timeComboBox;

        public TimeSpan ExtensionTime { get; private set; }

        public LockExtensionDialog()
        {
            Title = "Extend Lock";
            Width = 300;
            Height = 150;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            var panel = new System.Windows.Controls.StackPanel { Margin = new Thickness(10) };

            panel.Children.Add(new System.Windows.Controls.TextBlock
            {
                Text = "Select extension time:",
                Margin = new Thickness(0, 0, 0, 5)
            });

            _timeComboBox = new System.Windows.Controls.ComboBox
            {
                Margin = new Thickness(0, 0, 0, 10),
                Height = 23
            };

            _timeComboBox.Items.Add(new TimeOption { DisplayName = "15 minutes", Time = TimeSpan.FromMinutes(15) });
            _timeComboBox.Items.Add(new TimeOption { DisplayName = "30 minutes", Time = TimeSpan.FromMinutes(30) });
            _timeComboBox.Items.Add(new TimeOption { DisplayName = "1 hour", Time = TimeSpan.FromHours(1) });
            _timeComboBox.Items.Add(new TimeOption { DisplayName = "2 hours", Time = TimeSpan.FromHours(2) });
            _timeComboBox.Items.Add(new TimeOption { DisplayName = "4 hours", Time = TimeSpan.FromHours(4) });
            _timeComboBox.Items.Add(new TimeOption { DisplayName = "8 hours", Time = TimeSpan.FromHours(8) });

            _timeComboBox.SelectedIndex = 1; // Default to 30 minutes

            panel.Children.Add(_timeComboBox);

            var buttonPanel = new System.Windows.Controls.StackPanel
            {
                Orientation = System.Windows.Controls.Orientation.Horizontal,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Right
            };

            var okButton = new System.Windows.Controls.Button
            {
                Content = "OK",
                IsDefault = true,
                Margin = new Thickness(0, 0, 5, 0),
                Padding = new Thickness(10, 3, 10, 3)
            };
            okButton.Click += (s, e) =>
            {
                if (_timeComboBox.SelectedItem is TimeOption option)
                {
                    ExtensionTime = option.Time;
                    DialogResult = true;
                    Close();
                }
            };
            buttonPanel.Children.Add(okButton);

            var cancelButton = new System.Windows.Controls.Button
            {
                Content = "Cancel",
                IsCancel = true,
                Padding = new Thickness(10, 3, 10, 3)
            };
            buttonPanel.Children.Add(cancelButton);

            panel.Children.Add(buttonPanel);

            Content = panel;
        }

        private class TimeOption
        {
            public string DisplayName { get; set; }
            public TimeSpan Time { get; set; }

            public override string ToString() => DisplayName;
        }
    }