using System.Windows;
using UI.ViewModels;

namespace UI.Views;

public partial class MainWindowView : Window
{
    public MainWindowView()
    {
        DataContext = new MainWindowViewModel();
        InitializeComponent();
    }
}