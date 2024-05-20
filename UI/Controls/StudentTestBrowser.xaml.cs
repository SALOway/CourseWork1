using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UI.ViewModel;
using UI.Windows;

namespace UI.Controls;

public partial class StudentTestBrowser : UserControl
{
    public StudentTestBrowser()
    {
        DataContext = new StudentTestBrowserViewModel();
        InitializeComponent();
    }

    private void LogOut_Click(object sender, RoutedEventArgs e)
    {
        MainWindow.Instance.LogOut();
    }

    private void Search_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            ((StudentTestBrowserViewModel)DataContext).Search();
        }
    }

    private void GoToDetails_OnClick(object sender, RoutedEventArgs e)
    {
        var selectedTest = ((StudentTestBrowserViewModel)DataContext).SelectedTest;
        if (selectedTest != null)
        {
            MainWindow.CurrentTest = selectedTest.Test;
            MainWindow.Instance.GoTo(Enums.Menus.TestDetails);
        }
    }
}
