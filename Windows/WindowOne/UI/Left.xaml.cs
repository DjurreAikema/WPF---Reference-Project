using System.Windows;
using System.Windows.Controls;
using WpfApp1.Shared.Classes;

namespace WpfApp1.Windows.WindowOne.UI;

public partial class Left : UserControl
{
    public static readonly DependencyProperty SnacksListProperty = DependencyProperty.Register(
        nameof(SnacksList),
        typeof(List<Snack>),
        typeof(Left),
        new PropertyMetadata(new List<Snack>())
    );

    public List<Snack> SnacksList
    {
        get => (List<Snack>) GetValue(SnacksListProperty);
        set => SetValue(SnacksListProperty, value);
    }

    public event EventHandler<Snack> SnackSelected = null!;

    public Left()
    {
        InitializeComponent();
        // Ensure the DataGrid binds to the SnacksList property
        Loaded += (s, e) => { SnacksDataGrid.ItemsSource = SnacksList; };
    }

    private void SnacksDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
    {
        e.Column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
    }

    private void SnacksDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SnacksDataGrid.SelectedItem is Snack selectedSnack)
        {
            SnackSelected?.Invoke(this, selectedSnack);
        }
    }
}

// Approach Analysis

// Direct Binding with DataContext Inheritance:
// Pros:
//  - Simplifies the XAML and reduces the amount of boilerplate code.
//  - It's straightforward when the UserControl is used within the same data context.
// Cons:
//  - Tightly couples the UserControl to a specific data context structure, reducing its reusability across different parts of an application where the data context might differ.

// Using Dependency Properties:
// Pros:
//  - Increases the control's encapsulation and independence by allowing it to declare its own data requirements.
//  - This makes the UserControl more modular and easier to reuse in different contexts without assuming a specific data context.
// Cons:
//  - Requires more boilerplate code for declaring and using dependency properties.

// Conclusion
//  For a highly reusable UserControl, using dependency properties is the better approach.
//  It allows the control to be more self-contained, specifying its own data requirements and how it interacts with that data.
//  This modularity and independence from specific data contexts or parent controls enhance its reusability across various parts of an application or in different projects.