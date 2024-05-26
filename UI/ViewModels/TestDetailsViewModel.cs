using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Enums;
using Core.Models;
using System.Diagnostics;
using System.Windows;
using UI.Enums;
using UI.ObservableModels;

namespace UI.ViewModels;

public partial class TestDetailsViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableTest _test;

    [ObservableProperty]
    private ObservableTestAttempt? _lastTestAttempt;

    [ObservableProperty]
    private bool _canStart;

    [ObservableProperty]
    private bool _canContinue;

    public TestDetailsViewModel()
    {
        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;

        Test = new ObservableTest(context.CurrentTest!);
        
        var getLastAttempt = ServiceProvider.TestAttemptService.GetLastAttempt(context.CurrentUser!, context.CurrentTest!);
        if (!getLastAttempt.IsSuccess)
        {
            MessageBox.Show("Виникла помилка при спробі отримати останню спробу");
            Trace.WriteLine(getLastAttempt.ErrorMessage);
            return;
        }

        var lastAttempt = getLastAttempt.Value;
        if (lastAttempt != null)
        {
            LastTestAttempt = new ObservableTestAttempt(lastAttempt);
            CanContinue = Test.LastAttemptStatus == TestAttemptStatus.InProcess;
        }

        CanStart = !CanContinue && Test.AttemptCount < Test.Model.MaxAttempts;
    }

    public bool HasLastTestAttempt => LastTestAttempt != null;

    [RelayCommand]
    private void StartNewAttempt()
    {
        var messageBoxResult = MessageBox.Show("Ви впевнені що хочете почати нову спробу?", "Пітвердження початку нової спроби", MessageBoxButton.YesNo);

        if (messageBoxResult != MessageBoxResult.Yes)
        {
            return;
        }
        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;

        var newAttempt = new TestAttempt()
        {
            Status = TestAttemptStatus.InProcess,
            User = context.CurrentUser!,
            Test = context.CurrentTest!,
            StartedAt = DateTime.UtcNow
        };

        var adding = ServiceProvider.TestAttemptService.Add(newAttempt);
        if (!adding.IsSuccess)
        {
            MessageBox.Show("Виникла помилка при спробі cтворення нової спроби");
            Trace.WriteLine(adding.ErrorMessage);
            return;
        }

        context.CurrentTestAttempt = newAttempt;
        context.CurrentState = AppState.TestAttempt;
    }

    [RelayCommand]
    private void ContinueLastAttempt()
    {
        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;

        context.CurrentTestAttempt = LastTestAttempt!.Model;
        context.CurrentState = AppState.TestAttempt;
    }

    [RelayCommand]
    private void Back()
    {
        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;

        context.CurrentTestAttempt = null;
        context.CurrentTest = null;
        context.CurrentState = AppState.StudentTestBrowser;
    }
}
