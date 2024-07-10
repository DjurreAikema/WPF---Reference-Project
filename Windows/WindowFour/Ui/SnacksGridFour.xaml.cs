using System.ComponentModel;
using System.Reactive.Disposables;
using System.Runtime.CompilerServices;
using System.Windows;
using WpfApp1.Classes;

namespace WpfApp1.Windows.WindowFour.Ui;

public partial class SnacksGridFour : INotifyPropertyChanged
{
    public static readonly DependencyProperty DisposablesProperty = DependencyProperty.Register(
        nameof(Disposables), typeof(CompositeDisposable), typeof(SnacksGridFour),
        new PropertyMetadata(null));

    public CompositeDisposable Disposables
    {
        get => (CompositeDisposable) GetValue(DisposablesProperty);
        set => SetValue(DisposablesProperty, value);
    }

    public static readonly DependencyProperty SnacksObsProperty = DependencyProperty.Register(
        nameof(SnacksObs), typeof(IObservable<IEnumerable<Snack>>), typeof(SnacksGridFour),
        new PropertyMetadata(null, OnSnacksObsChanged));

    public IObservable<IEnumerable<Snack>> SnacksObs
    {
        get => (IObservable<IEnumerable<Snack>>) GetValue(SnacksObsProperty);
        set => SetValue(SnacksObsProperty, value);
    }

    private IEnumerable<Snack>? _snacks;

    public IEnumerable<Snack>? Snacks
    {
        get => _snacks;
        set
        {
            _snacks = value;
            OnPropertyChanged();
        }
    }

    public event Action<Snack>? SnackSelected;

    public SnacksGridFour()
    {
        InitializeComponent();

        // if (SnacksObs != null)
        // {
        // Disposables.Add(SnacksObs.Subscribe(snacks =>
        // {
        //     Snacks = snacks;
        //     OnPropertyChanged(nameof(Snacks));
        // }));
        // }

        SnacksDataGrid.SelectionChanged += (s, e) =>
        {
            if (SnacksDataGrid.SelectedItem is Snack selectedSnack)
            {
                SnackSelected?.Invoke(selectedSnack);
            }
        };
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private static void OnSnacksObsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is SnacksGridFour component)
        {
            component.SubscribeToSnacksObservable(e.NewValue as IObservable<IEnumerable<Snack>>);
        }
    }

    private void SubscribeToSnacksObservable(IObservable<IEnumerable<Snack>>? snacksObs)
    {
        // Disposables.Clear();

        if (snacksObs != null)
        {
            var subscription = snacksObs.Subscribe(snacks =>
            {
                Snacks = snacks;
                OnPropertyChanged(nameof(Snacks));
            });

            // Disposables.Add(subscription);
        }
    }
}