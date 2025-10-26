using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using EpicFlowchartBuilder.Models;
using EpicFlowchartBuilder.ViewModels;

namespace EpicFlowchartBuilder.Views
{
    public partial class MainWindow : Window
    {
        private MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Epic Flowchart Builder v1.0.0\n\n" +
                "A professional questionnaire flowchart designer with:\n" +
                "• Visual node-based editing\n" +
                "• AI-powered generation & validation\n" +
                "• Draw.io XML export\n" +
                "• Healthcare compliance features\n\n" +
                "Built with WPF and .NET 8",
                "About Epic Flowchart Builder",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void ApiKeyBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox && _viewModel != null)
            {
                _viewModel.AiApiKey = passwordBox.Password;
            }
        }
    }

    /// <summary>
    /// Converts NodeType to a background color
    /// </summary>
    public class NodeTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is NodeType nodeType)
            {
                return nodeType switch
                {
                    NodeType.Start => new SolidColorBrush(Color.FromRgb(213, 232, 212)), // Green
                    NodeType.Decision => new SolidColorBrush(Color.FromRgb(255, 242, 204)), // Yellow
                    NodeType.Question => new SolidColorBrush(Color.FromRgb(218, 232, 252)), // Blue
                    NodeType.Action => new SolidColorBrush(Color.FromRgb(225, 213, 231)), // Purple
                    NodeType.End => new SolidColorBrush(Color.FromRgb(248, 206, 204)), // Red
                    _ => Brushes.White
                };
            }
            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts null to boolean for IsEnabled bindings
    /// </summary>
    public class NullToBooleanConverter : IValueConverter
    {
        public static NullToBooleanConverter Instance { get; } = new NullToBooleanConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
