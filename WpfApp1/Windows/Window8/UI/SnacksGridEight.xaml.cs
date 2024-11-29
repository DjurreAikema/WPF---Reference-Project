using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using ReactiveUI;
using WpfApp1.Shared.Classes;

namespace WpfApp1.Windows.Window8.UI
{
    public partial class SnacksGridEight : ReactiveUserControl<SnacksGridEightViewModel>
    {
        public SnacksGridEight()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                // Bind the DataContext of the control to the ViewModel
                this.OneWayBind(ViewModel, vm => vm, v => v.DataContext)
                    .DisposeWith(disposables);
            });
        }
    }

    public class SnacksGridEightViewModel : ReactiveObject
    {
        private ObservableCollection<Snack>? _snacks;
        public ObservableCollection<Snack>? Snacks
        {
            get => _snacks;
            set => this.RaiseAndSetIfChanged(ref _snacks, value);
        }

        private Snack? _selectedSnack;
        public Snack? SelectedSnack
        {
            get => _selectedSnack;
            set => this.RaiseAndSetIfChanged(ref _selectedSnack, value);
        }

        public ReactiveCommand<Unit, Unit> ReloadCommand { get; set; }
        public ReactiveCommand<Unit, Unit> AddCommand { get; set; }

        // Constructor
        public SnacksGridEightViewModel()
        {
            // Commands will be assigned from the parent ViewModel
        }
    }
}