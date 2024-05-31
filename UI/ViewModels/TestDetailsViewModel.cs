using BLL;
using BLL.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Enums;
using Core.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using UI.Enums;
using UI.Interfaces;
using UI.ObservableModels;

namespace UI.ViewModels;

public partial class TestDetailsViewModel : ObservableObject
{
    private readonly ISessionContext _sessionContext;
    private readonly IUserService _userService;
    private readonly ITestService _testService;
    private readonly IQuestionService _questionService;
    private readonly IAnswerOptionService _answerOptionService;
    private readonly IUserAnswerService _userAnswerService;
    private readonly ITestAttemptService _testAttemptService;

    private readonly DispatcherTimer? _timer;


    [ObservableProperty]
    private ObservableTest _test;

    [ObservableProperty]
    private ObservableTestAttempt? _lastTestAttempt;

    [ObservableProperty]
    private bool _canStart;

    [ObservableProperty]
    private bool _canContinue;

    public TestDetailsViewModel(ISessionContext sessionContext, IUserService userService, ITestService testService, IQuestionService questionService, IAnswerOptionService answerOptionService, IUserAnswerService userAnswerService, ITestAttemptService testAttemptService)
    {
        _sessionContext = sessionContext;
        _userService = userService;
        _testService = testService;
        _questionService = questionService;
        _answerOptionService = answerOptionService;
        _userAnswerService = userAnswerService;
        _testAttemptService = testAttemptService;

        if (!TryGetObservableTest(out var test))
        {
            Test = null!;
            Back();
            return;
        }

        Test = test;

        if (!TryGetLastUserAttempt(out var lastTestAttempt))
        {
            Back();
            return;
        }

        LastTestAttempt = lastTestAttempt != null ? new ObservableTestAttempt(lastTestAttempt) : null;

        if (LastTestAttempt != null && LastTestAttempt.Status == TestAttemptStatus.InProcess)
        {
            if (Test.HasTermin)
            {
                var termin = Test.Termin!.Value;
                var difference = termin.Ticks - DateTime.UtcNow.Ticks;
                if (difference > 0)
                {
                    LastTestAttempt.Status = TestAttemptStatus.Expired;
                    LastTestAttempt.HasGrade = true;
                    LastTestAttempt.EndedAt = Test.Termin;
                    LastTestAttempt.Save(_testAttemptService, _questionService);
                }
            }

            if (Test.HasTimeLimit)
            {
                var timelimit = Test.TimeLimit!.Value.Ticks;
                var start = LastTestAttempt.StartedAt.Ticks;
                var current = DateTime.UtcNow.Ticks;
                var difference = current - start;
                var timeout = timelimit - difference;
                if (timeout <= 0)
                {
                    LastTestAttempt.Status = TestAttemptStatus.Failed;
                    LastTestAttempt.HasGrade = true;
                    LastTestAttempt.EndedAt = DateTime.UtcNow;
                    LastTestAttempt.Save(_testAttemptService, _questionService);
                }
            }
        }

        CanContinue = LastTestAttempt?.Status == TestAttemptStatus.InProcess;
        CanStart = !CanContinue && (!Test.HasAttempts || Test.AttemptsCount < Test.MaxAttempts);

        _timer = LastTestAttempt != null && (Test.HasTermin || Test.HasTimeLimit) ? new DispatcherTimer() : null;

        if (_timer != null)
        {
            _timer.Interval = TimeSpan.FromMilliseconds(100);

            if (Test.HasTermin && LastTestAttempt!.Status != TestAttemptStatus.InProcess)
            {
                _timer.Tick += Termin_Tick;
            }

            if (Test.HasTimeLimit && LastTestAttempt!.Status == TestAttemptStatus.InProcess)
            {
                _timer.Tick += TimeLimit_Tick;
            }
        }

        _timer?.Start();
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

        if (!TryCreateNewTestAttempt(out var newAttempt))
        {
            return;
        }

        _timer?.Stop();
        _sessionContext.CurrentTestAttemptId = newAttempt.Id;
        _sessionContext.CurrentState = AppState.TestAttempt;
    }

    [RelayCommand]
    private void ContinueLastAttempt()
    {
        _timer?.Stop();
        _sessionContext.CurrentTestAttemptId = LastTestAttempt!.TestAttemptId;
        _sessionContext.CurrentState = AppState.TestAttempt;
    }

    [RelayCommand]
    private void Back()
    {
        _timer?.Stop();
        _sessionContext.CurrentTestId = null;
        _sessionContext.CurrentState = AppState.StudentTestBrowser;
    }

    private bool TryGetCurrentUser(out User user)
    {
        user = null!;

        if (_sessionContext.CurrentUserId == null)
        {
            MessageBox.Show("Поточний користувач не встановлений", "Помилка наявності користувача", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        var getUser = _userService.GetById(_sessionContext.CurrentUserId.Value);
        if (!getUser.IsSuccess)
        {
            MessageBox.Show("Неможливо завантажити дані користувача", "Помилка отримання користувача", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        user = getUser.Value;

        if (user == null)
        {
            MessageBox.Show("Неможливо завантажити тести без користувача.\n", "Помилка завантаження тестів", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
        else if (user.Role != UserRole.Student)
        {
            MessageBox.Show("Користувач не є студентом.\n", "Помилка ролі користувача", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
        else if (user.StudentGroup == null)
        {
            MessageBox.Show("Студент не належить жодній групі.\n", "Помилка належності до групи", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        return true;
    }

    private bool TryGetObservableTest(out ObservableTest observableTest)
    {
        observableTest = null!;

        if (!TryGetTest(out var test))
        {
            return false;
        }

        observableTest = new ObservableTest(test);

        if (!TryGetObservableQuestions(test, out var questions))
        {
            return false;
        }

        observableTest.Questions = questions;

        return true;
    }

    private bool TryGetTest(out Test test)
    {
        test = null!;

        var getTest = _testService.GetById(_sessionContext.CurrentTestId!.Value);
        if (!getTest.IsSuccess)
        {
            MessageBox.Show("Виникла критична помилка\n" + getTest.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        test = getTest.Value;
        return true;
    }

    private bool TryGetLastUserAttempt(out TestAttempt? lastTestAttempt)
    {
        lastTestAttempt = null;

        var getTestAttempt = _testAttemptService.GetLastAttempt(_sessionContext.CurrentUserId!.Value, _sessionContext.CurrentTestId!.Value);
        if (!getTestAttempt.IsSuccess)
        {
            MessageBox.Show("Виникла критична помилка\n" + getTestAttempt.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        lastTestAttempt = getTestAttempt.Value;

        return true;
    }

    private bool TryGetObservableQuestions(Test test, out ObservableCollection<ObservableQuestion> observableQuestions)
    {
        observableQuestions = [];

        var getQuestions = _questionService.Get(q => q.Test.Id == test.Id);
        if (!getQuestions.IsSuccess)
        {
            MessageBox.Show("Виникла критична помилка\n" + getQuestions.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        var questions = getQuestions.Value.ToList();

        foreach (var question in questions)
        {
            var observableQuestion = new ObservableQuestion(question);

            if (!TryGetObservableAnswerOptions(question, out var observableAnswerOptions))
            {
                return false;
            }

            observableQuestion.AnswerOptions = observableAnswerOptions;

            observableQuestions.Add(observableQuestion);
        }

        return true;
    }

    private bool TryGetObservableAnswerOptions(Question question, out ObservableCollection<ObservableAnswerOption> observableAnswerOptions)
    {
        observableAnswerOptions = [];

        var getAnswerOption = _answerOptionService.Get(o => o.Question.Id == question.Id);
        if (!getAnswerOption.IsSuccess)
        {
            MessageBox.Show("Виникла критична помилка\n" + getAnswerOption.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        var answerOptions = getAnswerOption.Value.ToList();

        foreach (var answerOption in answerOptions)
        {
            if (!TryGetUserAnswer(answerOption, out var userAnswer))
            {
                return false;
            }
            var isChecked = userAnswer?.IsSelected ?? false;

            var observableAnswerOption = new ObservableAnswerOption(answerOption, isChecked);

            observableAnswerOptions.Add(observableAnswerOption);
        }

        return true;
    }

    private bool TryGetUserAnswer(AnswerOption answerOption, out UserAnswer? userAnswer)
    {
        userAnswer = null!;

        Result<IQueryable<UserAnswer>> getUserAnswers;
        if (LastTestAttempt != null)
        {
            getUserAnswers = _userAnswerService.Get(a => a.AnswerOption.Id == answerOption.Id && a.User.Id == _sessionContext.CurrentUserId && a.TestAttempt.Id == LastTestAttempt!.TestAttemptId);
        }
        else
        {
            getUserAnswers = _userAnswerService.Get(a => a.AnswerOption.Id == answerOption.Id && a.User.Id == _sessionContext.CurrentUserId);
        }

        if (!getUserAnswers.IsSuccess)
        {
            MessageBox.Show("Виникла критична помилка\n" + getUserAnswers.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        userAnswer = getUserAnswers.Value.FirstOrDefault();

        return true;
    }

    private bool TryCreateNewTestAttempt(out TestAttempt newAttempt)
    {
        newAttempt = null!;

        if (!TryGetCurrentUser(out var user) || !TryGetTest(out var test))
        {
            return false;
        }

        newAttempt = new TestAttempt()
        {
            Status = TestAttemptStatus.InProcess,
            User = user,
            Test = test,
            StartedAt = DateTime.UtcNow,
        };

        var adding = _testAttemptService.Add(newAttempt);
        if (!adding.IsSuccess)
        {
            MessageBox.Show("Виникла критична помилка при спробі cтворення нової спроби\n" + adding.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        return true;
    }

    private void Timer_Tick(object sender, EventArgs e)
    {
        if (LastTestAttempt == null)
        {
            return;
        }

        if (LastTestAttempt.Status == TestAttemptStatus.InProcess)
        {
            if (Test.HasTermin)
            {
                var termin = Test.Termin!.Value;
                var difference = termin.Ticks - DateTime.UtcNow.Ticks;
                if (difference > 0)
                {
                    LastTestAttempt.Status = TestAttemptStatus.Expired;
                    LastTestAttempt.HasGrade = true;
                    LastTestAttempt.EndedAt = Test.Termin;
                    LastTestAttempt.Save(_testAttemptService, _questionService);
                }
            }

            if (Test.HasTimeLimit)
            {
                var timelimit = Test.TimeLimit!.Value.Ticks;
                var start = LastTestAttempt.StartedAt.Ticks;
                var current = DateTime.UtcNow.Ticks;
                var difference = current - start;
                var timeout = timelimit - difference;
                if (timeout <= 0)
                {
                    LastTestAttempt.Status = TestAttemptStatus.Failed;
                    LastTestAttempt.HasGrade = true;
                    LastTestAttempt.EndedAt = DateTime.UtcNow;
                    LastTestAttempt.Save(_testAttemptService, _questionService);
                }
            }
        }

        CanContinue = LastTestAttempt?.Status == TestAttemptStatus.InProcess;
        CanStart = !CanContinue && (!Test.HasAttempts || Test.AttemptsCount < Test.MaxAttempts);
    }

    private void Termin_Tick(object? sender, EventArgs e)
    {
        if (LastTestAttempt == null)
        {
            return;
        }

        var termin = Test.Termin!.Value;
        var difference = termin.Ticks - DateTime.UtcNow.Ticks;
        if (difference > 0)
        {
            LastTestAttempt.Status = TestAttemptStatus.Expired;
            LastTestAttempt.HasGrade = true;
            LastTestAttempt.EndedAt = Test.Termin;
            LastTestAttempt.Save(_testAttemptService, _questionService);
        }
    }

    private void TimeLimit_Tick(object? sender, EventArgs e) 
    {
        if (LastTestAttempt == null)
        {
            return;
        }

        var timeLimit = Test.TimeLimit!.Value;
        var startedAt = LastTestAttempt!.StartedAt;
        var difference = DateTime.UtcNow.Ticks - startedAt.Ticks + timeLimit.Ticks;
        if (difference >= 0)
        {
            LastTestAttempt.Status = TestAttemptStatus.Failed;
            LastTestAttempt.EndedAt = LastTestAttempt.StartedAt + Test.TimeLimit.Value.TimeOfDay;
            //LastTestAttempt.SaveModel();
            CanContinue = false;
        }
    }
}
