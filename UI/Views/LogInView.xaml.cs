using Core.Enums;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UI.Enums;
using UI.ViewModels;


namespace UI.Views;

public partial class LogInView : UserControl
{
    public LogInView()
    {
        DataContext = new LogInViewModel();
        InitializeComponent();
    }
}
