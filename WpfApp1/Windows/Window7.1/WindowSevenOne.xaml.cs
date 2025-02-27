using System.ComponentModel;
using System.Reactive;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Windows;
using WpfApp1.Shared.Classes;
using WpfApp1.Shared.Locking.V1;

namespace WpfApp1.Windows.Window7._1;

public partial class WindowSevenOneV2 : INotifyPropertyChanged
{
    public WindowSevenOneViewModelV2 ViewModel { get; } = new();
    public Subject<bool> TriggerDispose { get; set; } = new();

    private string _statusText = "Ready";

    public string StatusText
    {
        get => _statusText;
        set
        {
            _statusText = value;
            OnPropertyChanged();
        }
    }

    private SnackV2? _selectedSnack;

    public SnackV2? SelectedSnack
    {
        get => _selectedSnack;
        set
        {
            _selectedSnack = value;
            OnPropertyChanged();
        }
    }

    public WindowSevenOneV2()
    {
        InitializeComponent();

        // Update status text and selected snack when ViewModel changes
        ViewModel.SelectedSnackObs.Subscribe(snack =>
        {
            SelectedSnack = snack;
            UpdateStatusText(snack);
        });

        // Update status for lock events
        ViewModel.LockStatusChanged.Subscribe(result => { StatusText = result.Message; });
    }

    private void UpdateStatusText(SnackV2? snack)
    {
        if (snack == null)
        {
            StatusText = "No snack selected";
            return;
        }

        var status = snack.LockState switch
        {
            LockState.Unlocked => "Not locked - available for editing",
            LockState.LockedByMe => $"Locked by you until {snack.LockExpiresAt?.ToLocalTime():g}",
            LockState.LockedByOther => $"Locked by {snack.LockedBy}",
            LockState.LockExpired => "Lock has expired",
            _ => "Unknown lock state"
        };

        StatusText = $"Snack: {snack.Name} - {status}";
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        // Ensure we release all locks and clean up resources
        ViewModel.ReleaseAllLocks.OnNext(Unit.Default);
        ViewModel.Dispose();
        TriggerDispose.OnNext(true);
    }

    // --- Grid
    private void SnacksGrid_SnackSelected(SnackV2 snack)
    {
        ViewModel.SelectedSnackChanged.OnNext(snack);
    }

    private void SnacksGrid_OnAddSnack()
    {
        ViewModel.SelectedSnackChanged.OnNext(new SnackV2());
    }

    private void SnacksGrid_OnReload()
    {
        ViewModel.Reload.OnNext(Unit.Default);
    }

    // --- Details
    private void SnackDetails_OnSnackSaved(SnackV2 snack)
    {
        if (snack.Id is 0 or null)
            ViewModel.Create.OnNext(snack);
        else
            ViewModel.Update.OnNext(snack);
    }

    private void SnackDetails_OnSnackDeleted(int snackId)
    {
        ViewModel.Delete.OnNext(snackId);
    }

    // --- Locking
    private void LockingControl_OnLockStatusChanged(object sender, LockResult result)
    {
        ViewModel.LockStatusChanged.OnNext(result);
    }

    // --- Toolbar
    private void Reload_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.Reload.OnNext(Unit.Default);
    }

    private void ReleaseAllLocks_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.ReleaseAllLocks.OnNext(Unit.Default);
    }

    private void CleanupExpiredLocks_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.CleanupExpiredLocks.OnNext(Unit.Default);
    }

    // --- INotifyPropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}