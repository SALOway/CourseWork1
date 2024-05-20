using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UI.ViewModel;
using UI.Windows;

namespace UI.Controls;

public partial class StudentTestBrowser : UserControl
{
    private readonly MainWindow _mainWindow;

    public StudentTestBrowser(MainWindow mainWindow)
    {
        DataContext = new StudentTestBrowserViewModel();
        _mainWindow = mainWindow;
        InitializeComponent();
    }

    private void LogOut_Click(object sender, RoutedEventArgs e)
    {
        _mainWindow.LogOut();
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
            AppState.CurrentTest = selectedTest.Test;
            _mainWindow.GoTo(Enums.Menus.TestDetails);
        }
    }
}
