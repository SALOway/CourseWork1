using CommunityToolkit.Mvvm.ComponentModel;
using Core.Enums;
using System.Diagnostics;
using System.Windows;
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
            Trace.WriteLine(getLastAttempt);
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
}
