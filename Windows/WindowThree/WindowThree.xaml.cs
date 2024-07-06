﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using WpfApp1.Classes;

namespace WpfApp1.Windows.WindowThree;

public partial class WindowThree : Window, INotifyPropertyChanged
{
    private WindowThreeViewModel ViewModel { get; } = new();

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

    private Snack? _selectedSnack;

    public Snack? SelectedSnack
    {
        get => _selectedSnack;
        set
        {
            _selectedSnack = value;
            OnPropertyChanged();
        }
    }

    private bool _loading;

    public bool Loading
    {
        get => _loading;
        set
        {
            _loading = value;
            OnPropertyChanged();
        }
    }

    public WindowThree()
    {
        InitializeComponent();
        DataContext = this;

        ViewModel.Snacks.Subscribe(snacks =>
        {
            Snacks = snacks;
            OnPropertyChanged(nameof(Snacks));
        });

        ViewModel.SelectedSnack.Subscribe(snack =>
        {
            SelectedSnack = snack;
            OnPropertyChanged(nameof(SelectedSnack));
        });

        ViewModel.Loading.Subscribe(loading =>
        {
            Loading = loading;
            OnPropertyChanged(nameof(Loading));
        });
    }

    private void SnacksGrid_SnackSelected(Snack snack)
    {
        ViewModel.SelectedSnackChanged.OnNext(snack);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}