using CommunityToolkit.Mvvm.ComponentModel;
using Core.Enums;
using Core.Models;
using System.Windows;
using UI.ViewModels;

namespace UI.ObservableModels;

public partial class ObservableTest : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AttemptsRatio))]
    private Test _test;

    [ObservableProperty]
    private string _lastAttemptStatus;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AttemptsRatio))]
    private string _attemptCount;

    public ObservableTest(Test test)
    {
        Test = test;

        LastAttemptStatus = GetLastAttemptStatus();
        AttemptCount = GetAttemptCount();
    }

    public string AttemptsRatio => $"{AttemptCount} з {Test.MaxAttempts}";

    private string GetAttemptCount()
    {
        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;
        var currentUser = context.CurrentUser;

        var userTestAttempts = Test.TestAttempts.Where(a => a.User.Id == currentUser!.Id);

        return userTestAttempts.Count().ToString();
    }

    private string GetLastAttemptStatus()
    {
        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;
        var currentUser = context.CurrentUser;

        var userTestAttempts = Test.TestAttempts.Where(a => a.User.Id == currentUser!.Id);
        var lastAttempt = userTestAttempts.OrderByDescending(attempt => attempt.StartedAt).FirstOrDefault();

        if (lastAttempt == null)
        {
            return "-";
        }

        return lastAttempt.Status switch
        {
            TestAttemptStatus.InProcess => "В процесі",
            TestAttemptStatus.Completed => "Завершено",
            TestAttemptStatus.Expired => "Прострочено",
            TestAttemptStatus.Failed => "Провалено",
            TestAttemptStatus.Cancelled => "Відменено",
            _ => throw new NotImplementedException(),
        };
    }
}
