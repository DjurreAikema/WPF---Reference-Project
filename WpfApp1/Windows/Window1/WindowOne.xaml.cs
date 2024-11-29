using System.Reflection.Emit;
using System.Windows;
using WpfApp1.Shared.Classes;
using WpfApp1.Windows.Window1.UI;

namespace WpfApp1.Windows.Window1;

public partial class WindowOne : Window
{
    public string HeaderText { get; set; } = "Default Header";
    public List<Snack> SnacksLister { get; set; }

    public WindowOne()
    {
        InitializeComponent();
        DataContext = this;

        SnacksLister = GetListOfSnacks();

        var leftControl = (Left)this.FindName("LeftControl"); // Assuming the x:Name of the Left UserControl in XAML is "LeftControl"
        leftControl.SnackSelected += LeftControl_SnackSelected;
    }

    private void LeftControl_SnackSelected(object sender, Snack selectedSnack)
    {
        // Update the label in the center section with the selected snack's details
        Label centerLabel = (Label)FindName("CenterLabel");
        // centerLabel.Content = $"Selected: {selectedSnack.Name} - Price: {selectedSnack.Price} - Quantity: {selectedSnack.Quantity}";
    }

    private static List<Snack> GetListOfSnacks()
    {
        return
        [
            new Snack {Name = "Chips", Price = 1.50, Quantity = 10},
            new Snack {Name = "Chocolate", Price = 2.50, Quantity = 5},
            new Snack {Name = "Candy", Price = 0.50, Quantity = 20},
            new Snack {Name = "Gum", Price = 0.25, Quantity = 50},
            new Snack {Name = "Soda", Price = 1.00, Quantity = 15},
            new Snack {Name = "Water", Price = 0.75, Quantity = 25},
            new Snack {Name = "Juice", Price = 1.25, Quantity = 10},
            new Snack {Name = "Cookies", Price = 1.75, Quantity = 5}
        ];
    }
}