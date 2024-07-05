using System.Windows;

namespace WpfApp1.Shared;

// public class DataContextProxy : Freezable
// {
//     protected override Freezable CreateInstanceCore()
//     {
//         return new DataContextProxy();
//     }
//
//     public object Data
//     {
//         get => GetValue(DataProperty);
//         set => SetValue(DataProperty, value);
//     }
//
//     public static readonly DependencyProperty DataProperty =
//         DependencyProperty.Register(nameof(Data), typeof(object), typeof(DataContextProxy), new UIPropertyMetadata(null));
// }