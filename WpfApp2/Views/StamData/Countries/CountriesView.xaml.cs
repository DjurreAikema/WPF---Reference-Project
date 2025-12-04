using System.Reactive;
using System.Reactive.Subjects;
using System.Windows;
using WpfApp2.Data.Classes.Stamdata;
using WpfApp2.Shared.Debugging.Extensions;
using WpfApp2.Shared.Navigation.Interfaces;

namespace WpfApp2.Views.StamData.Countries;

public partial class CountriesView : INavigationAware
{
    public CountriesViewModel Vm { get; set; } = new CountriesViewModel().RegisterWithTracker();
    public Subject<bool> TriggerDispose { get; set; } = new();

    // --- Internal Properties
    private bool _shouldDispose;

    // --- Navigation
    public void OnNavigatedTo()
    {
        if (!_shouldDispose) return;

        Vm = new CountriesViewModel().RegisterWithTracker();
        _shouldDispose = false;
    }

    public void OnNavigatedFrom(bool isInStack)
    {
        _shouldDispose = !isInStack;
        if (!_shouldDispose) return;

        Vm.Dispose();
        TriggerDispose.OnNext(true);
    }

    public CountriesView()
    {
        InitializeComponent();

        // Dispose of all subscriptions when the window is closed
        Unloaded += (_, _) =>
        {
            if (Application.Current.MainWindow == null || !_shouldDispose)
                return;

            Vm.Dispose();
            TriggerDispose.OnNext(true);
        };
    }

    // --- List
    private void OnCountrySelected(Country obj) => Vm.SelectedChanged.OnNext(obj);

    private void OnAddCountry() => Vm.SelectedChanged.OnNext(new Country());

    private void OnReloadCountries() => Vm.Reload.OnNext(Unit.Default);

    // --- Details
    private void OnCountrySaved(Country obj)
    {
        Console.WriteLine($"Saving: {obj.Name}, ID: {obj.Id}");

        if (obj.Id is 0 or null)
            Vm.Create.OnNext(obj);
        else
            Vm.Update.OnNext(obj);
    }

    private void OnCountryDeleted(int id) => Vm.Delete.OnNext(id);
}