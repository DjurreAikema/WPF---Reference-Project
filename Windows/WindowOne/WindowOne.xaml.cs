using System.Windows;
using WpfApp1.Classes;

namespace WpfApp1.Windows.WindowOne;

public partial class WindowOne : Window
{
    public string HeaderText { get; set; } = "Default Header";
    public List<Snack> SnacksList { get; set; }

    public WindowOne()
    {
        InitializeComponent();
        DataContext = this;

        SnacksList = GetListOfSnacks();
    }

    private static List<Snack> GetListOfSnacks()
    {
        var snacks = new List<Snack>
        {
            new() {Name = "Chips", Price = 1.50, Quantity = 10},
            new() {Name = "Chocolate", Price = 2.50, Quantity = 5},
            new() {Name = "Candy", Price = 0.50, Quantity = 20},
            new() {Name = "Gum", Price = 0.25, Quantity = 50},
            new() {Name = "Soda", Price = 1.00, Quantity = 15},
            new() {Name = "Water", Price = 0.75, Quantity = 25},
            new() {Name = "Juice", Price = 1.25, Quantity = 10},
            new() {Name = "Cookies", Price = 1.75, Quantity = 5}
        };

        return snacks;
    }
}