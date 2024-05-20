using System.Windows;
using System.Windows.Controls;
using UI.Enums;
using UI.Windows;

namespace UI.Controls;

public partial class TestDetails : UserControl
{
    public TestDetails()
    {
        InitializeComponent();
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        MainWindow.Instance.GoTo(Menus.StudentTestBrowser);
    }
}
