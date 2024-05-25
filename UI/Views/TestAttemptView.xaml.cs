using System.Windows.Controls;
using UI.ViewModels;

namespace UI.Views;

public partial class TestAttemptView : UserControl
{
    public TestAttemptView()
    {
        DataContext = new TestAttemptViewModel();
        InitializeComponent();
    }
}
