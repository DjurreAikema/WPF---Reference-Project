﻿using System.Windows;

namespace WpfApp1.Windows.WindowTwo;

public partial class WindowTwo : Window
{
    public WindowTwoViewModel ViewModel { get; } = new();

    public WindowTwo()
    {
        InitializeComponent();
    }

    private void ToggleButton_OnChecked(object sender, RoutedEventArgs e)
    {
        // ViewModel.gaietsdoen();
    }
}