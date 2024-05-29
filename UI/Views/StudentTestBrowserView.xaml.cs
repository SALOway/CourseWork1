using System.Windows.Controls;
using UI.ViewModels;

namespace UI.Views;

public partial class StudentTestBrowserView : UserControl
{
    public StudentTestBrowserView()
    {
        DataContext = new StudentTestBrowserViewModel();
        InitializeComponent();
    }
}
