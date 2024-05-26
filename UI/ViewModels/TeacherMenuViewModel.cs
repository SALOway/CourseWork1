using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using UI.Enums;

namespace UI.ViewModels;

public partial class TeacherMenuViewModel : ObservableObject
{
    [RelayCommand]
    private static void LogOut()
    {
        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;

        context.CurrentUser = null;
        context.CurrentState = AppState.LogIn;
    }

    [RelayCommand]
    private void OpenTestBrowser()
    {
        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;

        context.CurrentState = AppState.TeacherTestBrowser;
    }

    //[RelayCommand]
    //private void OpenStudentBrowser()
    //{
    //    var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;

    //    context.CurrentState = AppState.StudentBrowser;
    //}
}
