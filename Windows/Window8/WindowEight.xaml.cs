using System.Reactive.Disposables;
using System.Windows.Controls;
using ReactiveUI;

namespace WpfApp1.Windows.Window8;

public partial class WindowEight : ReactiveWindow<WindowEightViewModel>
{
    public WindowEight()
    {
        InitializeComponent();
        ViewModel = new WindowEightViewModel();

        this.WhenActivated(disposables =>
        {
            // Bind ViewModel properties to the View
            this.OneWayBind(ViewModel, vm => vm.Snacks, v => v.SnacksGrid.ItemsSource)
                .DisposeWith(disposables);

            this.Bind(ViewModel, vm => vm.SelectedSnack, v => v.SnacksGrid.SelectedItem)
                .DisposeWith(disposables);

            // Bind Commands
            this.BindCommand(ViewModel, vm => vm.LoadSnacksCommand, v => v.ReloadButton)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, vm => vm.CreateSnackCommand, v => v.AddButton, nameof(Button.Click))
                .DisposeWith(disposables);

            // Other bindings...
        });
    }
}