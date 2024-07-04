using System.Windows.Controls;
using WpfApp1.Classes;

namespace WpfApp1.Windows.WindowOne.UI;

public partial class Left : UserControl
{
    public Left()
    {
        InitializeComponent();
    }

    private void SnacksDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
    {
        e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
    }
}