using System.Reactive;
using System.Reactive.Disposables;
using ReactiveUI;
using WpfApp1.Shared.Classes;

namespace WpfApp1.Windows.Window8.UI;

public partial class SnackDetailsEight : ReactiveUserControl<SnackDetailsEightViewModel>
{
    public SnackDetailsEight()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            // Bind the DataContext of the control to the ViewModel.Snack
            this.OneWayBind(ViewModel, vm => vm, v => v.DataContext)
                .DisposeWith(disposables);
        });
    }
}

public class SnackDetailsEightViewModel : ReactiveObject
{
    private Snack? _snack;
    public Snack? Snack
    {
        get => _snack;
        set => this.RaiseAndSetIfChanged(ref _snack, value);
    }

    public ReactiveCommand<Unit, Unit> SaveCommand { get; set; }
    public ReactiveCommand<Unit, Unit> DeleteCommand { get; set; }

    // Constructor
    public SnackDetailsEightViewModel()
    {
        // Commands will be assigned from the parent ViewModel
    }
}