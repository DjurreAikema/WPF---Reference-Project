using ReactiveUI;

namespace WpfApp1.Windows.Window8
{
    public partial class WindowEight : ReactiveWindow<WindowEightViewModel>
    {
        public WindowEight()
        {
            InitializeComponent();
            ViewModel = new WindowEightViewModel();

            this.WhenActivated(disposables =>
            {
                // You can add any additional bindings here if needed
                // For example, handling notifications or other UI elements
            });
        }
    }
}