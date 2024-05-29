using System.Windows.Controls;
using UI.ViewModels;

namespace UI.Views;

public partial class TestDetailsView : UserControl
{
    public TestDetailsView()
    {
        DataContext = new TestDetailsViewModel();
        InitializeComponent();
    }
}
