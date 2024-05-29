using System.Windows.Controls;
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
