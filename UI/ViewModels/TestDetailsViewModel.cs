using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Enums;
using Core.Models;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using UI.Enums;
using UI.ObservableModels;

namespace UI.ViewModels;

public partial class TestDetailsViewModel : ObservableObject
{
    private readonly DispatcherTimer? _timer;

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

        var test = context.CurrentTest!;
        var user = context.CurrentUser!;
        Test = new ObservableTest(test);

        var getQuestions = ServiceProvider.QuestionService.Get(q => q.Test.Id == test.Id);
        if (!getQuestions.IsSuccess)
        {
            MessageBox.Show(getQuestions.ErrorMessage);
            return;
        }

        var questions = getQuestions.Value.ToList();

        foreach (var question in questions)
        {
            var observableQuestion = new ObservableQuestion(question);

            var getAnswerOption = ServiceProvider.AnswerOptionService.Get(o => o.Question.Id == question.Id);
            if (!getAnswerOption.IsSuccess)
            {
                MessageBox.Show(getAnswerOption.ErrorMessage);
                return;
            }

            var answerOptions = getAnswerOption.Value.ToList();

            foreach (var answerOption in answerOptions)
            {
                var userAnswer = answerOption.UserAnswers.Where(a => a.User.Id == user.Id).FirstOrDefault();
                var observableAnswerOption = new ObservableAnswerOption(answerOption, userAnswer!);

                observableQuestion.AnswerOptions.Add(observableAnswerOption);
            }

            Test.Questions.Add(observableQuestion);

            if (test.HasTermin || test.HasTimer)
            {
                _timer = new DispatcherTimer();
                _timer.Tick += (s, e) => {
                    if (LastTestAttempt != null)
                    {
                        if (test.HasTermin && LastTestAttempt.Status != TestAttemptStatus.InProcess)
                        {
                            var termin = test.Termin!.Value;
                            var difference = termin.Ticks - DateTime.UtcNow.Ticks;
                            CanStart = difference > 0;
                        }
                        else if (LastTestAttempt.Status == TestAttemptStatus.InProcess && test.HasTimer)
                        {
                            var timeLimit = test.TimeLimit!.Value;
                            var startedAt = LastTestAttempt.StartedAt;
                            var difference = DateTime.UtcNow.Ticks - startedAt.Ticks + timeLimit.Ticks;
                            if (difference >= 0)
                            {
                                LastTestAttempt.Status = TestAttemptStatus.Failed;
                                LastTestAttempt.EndedAt = LastTestAttempt.StartedAt + test.TimeLimit;
                                LastTestAttempt.SaveModel();
                                CanContinue = false;
                            }
                        }
                    }
                };
                _timer.Interval = TimeSpan.FromMilliseconds(100);
                _timer.Start();
            }
        }

        var getLastAttempt = ServiceProvider.TestAttemptService.GetLastAttempt(user, test);
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

        CanStart = !CanContinue && Test.AttemptsCount < Test.MaxAttempts;

        if (test.HasTermin && LastTestAttempt?.Status != TestAttemptStatus.InProcess)
        {
            var termin = test.Termin!.Value;
            var difference = termin.Ticks - DateTime.UtcNow.Ticks;
            CanStart = difference > 0;
        }
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

        _timer?.Stop();
        context.CurrentTestAttempt = newAttempt;
        context.CurrentState = AppState.TestAttempt;
    }

    [RelayCommand]
    private void ContinueLastAttempt()
    {
        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;

        _timer?.Stop();
        context.CurrentTestAttempt = LastTestAttempt!.Model;
        context.CurrentState = AppState.TestAttempt;
    }

    [RelayCommand]
    private void Back()
    {
        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;

        _timer?.Stop();
        context.CurrentTestAttempt = null;
        context.CurrentTest = null;
        context.CurrentState = AppState.StudentTestBrowser;
    }
}
