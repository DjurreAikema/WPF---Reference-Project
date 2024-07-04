using System.Windows;
using WpfApp1.Classes;

namespace WpfApp1.Windows.WindowOne;

public partial class WindowOne : Window
{
    public string HeaderText { get; set; } = "Default Header";
    public List<Snack> SnacksLister { get; set; }

    public WindowOne()
    {
        InitializeComponent();
        DataContext = this;

        SnacksLister = GetListOfSnacks();
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