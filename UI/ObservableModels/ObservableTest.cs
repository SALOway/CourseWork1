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
    private Test _model;

    [ObservableProperty]
    private TestAttemptStatus? _lastAttemptStatus;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AttemptsRatio))]
    private int _attemptCount;

    [ObservableProperty]
    private int _maxGrade;

    [ObservableProperty]
    private int _questionsCount;

    public ObservableTest(Test test)
    {
        Model = test;
        QuestionsCount = Model.Questions.Count;
        LastAttemptStatus = GetLastAttemptStatus();
        AttemptCount = GetAttemptCount();
        MaxGrade = Model.Questions.Sum(q => q.GradeValue);
    }

    public string AttemptsRatio => $"{AttemptCount} з {Model.MaxAttempts}";

    private int GetAttemptCount()
    {
        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;
        var currentUser = context.CurrentUser;

        var userTestAttempts = Model.TestAttempts.Where(a => a.User.Id == currentUser!.Id);

        return userTestAttempts.Count();
    }

    private TestAttemptStatus? GetLastAttemptStatus()
    {
        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;
        var currentUser = context.CurrentUser;

        var userTestAttempts = Model.TestAttempts.Where(a => a.User.Id == currentUser!.Id);
        var lastAttempt = userTestAttempts.OrderByDescending(attempt => attempt.StartedAt).FirstOrDefault();

        return lastAttempt?.Status;
    }
}
