using BLL.Interfaces;
using Core.Models;
using System.Windows;
using System.Windows.Controls;
using UI.Controls;
using UI.Enums;

namespace UI.Windows;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public User? CurrentUser => AppState.CurrentUser;
    public Test? CurrentTest => AppState.CurrentTest;

    public void GoTo(Menus menu)
    {
        ContentControl.Content = menu switch
        {
            Menus.LogIn => new LogInControl(this),
            Menus.StudentManager => new StudentsManagerMenu(this),
            Menus.StudentTestBrowser => new StudentTestBrowser(this),
            Menus.TeacherMenu => new TeacherMenu(this),
            Menus.TestDetails => new TestDetails(this),
            Menus.TestAttempt => new TestAttemptControl(),
            _ => throw new NotImplementedException(),
        };
    }

    public void LogOut()
    {
        AppState.CurrentUser = null;
        GoTo(Menus.LogIn);
    }

    private void Window_Loaded(object sender, RoutedEventArgs e) => GoTo(Menus.LogIn);
}