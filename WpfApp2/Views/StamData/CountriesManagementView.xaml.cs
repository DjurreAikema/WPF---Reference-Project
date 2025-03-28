using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using WpfApp2.Data;
using WpfApp2.Shared.Classes;

namespace WpfApp2.Views.StamData;

public partial class CountriesManagementView : UserControl, INotifyPropertyChanged
{
    private readonly AppDbContext _dbContext;
    private Country? _selectedCountry;
    private ObservableCollection<Country> _countries = new();
    private bool _isFormEnabled = false;
    private bool _canSave = false;
    private bool _canDelete = false;

    public Country? SelectedCountry
    {
        get => _selectedCountry;
        set
        {
            _selectedCountry = value;
            OnPropertyChanged();
            UpdateButtonStates();
        }
    }

    public ObservableCollection<Country> Countries
    {
        get => _countries;
        set
        {
            _countries = value;
            OnPropertyChanged();
        }
    }

    public bool IsFormEnabled
    {
        get => _isFormEnabled;
        set
        {
            _isFormEnabled = value;
            OnPropertyChanged();
        }
    }

    public bool CanSave
    {
        get => _canSave;
        set
        {
            _canSave = value;
            OnPropertyChanged();
        }
    }

    public bool CanDelete
    {
        get => _canDelete;
        set
        {
            _canDelete = value;
            OnPropertyChanged();
        }
    }

    public CountriesManagementView()
    {
        InitializeComponent();
        DataContext = this;

        // Get the DbContext from the service provider
        var app = (App)Application.Current;
        _dbContext = app.ServiceProvider.GetService(typeof(AppDbContext)) as AppDbContext;

        LoadCountries();
    }

    private async void LoadCountries()
    {
        if (_dbContext == null) return;

        try
        {
            var countries = await _dbContext.Countries.ToListAsync();
            Countries = new ObservableCollection<Country>(countries);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading countries: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void UpdateButtonStates()
    {
        IsFormEnabled = SelectedCountry != null;
        CanSave = SelectedCountry != null && !string.IsNullOrWhiteSpace(SelectedCountry.Name);
        CanDelete = SelectedCountry != null && SelectedCountry.Id.HasValue && SelectedCountry.Id > 0;
    }

    private void NewCountry_Click(object sender, RoutedEventArgs e)
    {
        SelectedCountry = new Country();
        CountriesListView.SelectedItem = null;
        IsFormEnabled = true;
        CanDelete = false;
    }

    private void Refresh_Click(object sender, RoutedEventArgs e)
    {
        LoadCountries();
    }

    private void CountriesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CountriesListView.SelectedItem is Country country)
        {
            // Create a new instance to avoid directly modifying the list item
            SelectedCountry = new Country(country);
        }
    }

    private async void Save_Click(object sender, RoutedEventArgs e)
    {
        if (_dbContext == null || SelectedCountry == null) return;

        try
        {
            if (SelectedCountry.Id == null || SelectedCountry.Id <= 0)
            {
                // Add new country
                _dbContext.Countries.Add(SelectedCountry);
            }
            else
            {
                // Update existing country
                _dbContext.Entry(SelectedCountry).State = EntityState.Modified;
            }

            await _dbContext.SaveChangesAsync();
            LoadCountries();

            MessageBox.Show("Country saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving country: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void Delete_Click(object sender, RoutedEventArgs e)
    {
        if (_dbContext == null || SelectedCountry == null || !SelectedCountry.Id.HasValue) return;

        var result = MessageBox.Show("Are you sure you want to delete this country?",
            "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes) return;

        try
        {
            var country = await _dbContext.Countries.FindAsync(SelectedCountry.Id);
            if (country != null)
            {
                _dbContext.Countries.Remove(country);
                await _dbContext.SaveChangesAsync();
                LoadCountries();

                SelectedCountry = null;

                MessageBox.Show("Country deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error deleting country: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        if (CountriesListView.SelectedItem is Country country)
        {
            // Revert changes to selected country
            SelectedCountry = new Country(country);
        }
        else
        {
            // Clear form if creating new country
            SelectedCountry = null;
            IsFormEnabled = false;
        }
    }

    private void BackButton_Click(object sender, RoutedEventArgs e)
    {
        // Navigate back to Stamdata
        var mainWindow = (MainWindow)Application.Current.MainWindow;
        var navigationService = mainWindow.GetNavigationService();
        navigationService.NavigateBack();
    }

    // INotifyPropertyChanged implementation
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}