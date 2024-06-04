using BLL.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Enums;
using Core.Models;
using System.Windows;
using System.Windows.Threading;
using UI.Enums;
using UI.Interfaces;
using UI.ObservableModels;

namespace UI.ViewModels;

public partial class TestAttemptViewModel : ObservableObject
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
    private ObservableTestAttempt _testAttempt;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSelectedQuestionNotFirst))]
    [NotifyPropertyChangedFor(nameof(IsSelectedQuestionNotLast))]
    [NotifyPropertyChangedFor(nameof(SelectedQuestionNumber))]
    private ObservableQuestion? _selectedQuestion;

    [ObservableProperty]
    private bool _hasTimer;
    [ObservableProperty]
    private TimeSpan? _timeout;

    public TestAttemptViewModel(ISessionContext sessionContext, IUserService userService, ITestService testService, IQuestionService questionService, IAnswerOptionService answerOptionService, IUserAnswerService userAnswerService, ITestAttemptService testAttemptService)
    {
        _sessionContext = sessionContext;
        _userService = userService;
        _testService = testService;
        _questionService = questionService;
        _answerOptionService = answerOptionService;
        _userAnswerService = userAnswerService;
        _testAttemptService = testAttemptService;

        if (!TryGetCurrentObservableTestAttempt(out var observableTestAttempt))
        {
            TestAttempt = null!;
            _sessionContext.CurrentTestAttemptId = null;
            _sessionContext.CurrentState = AppState.TestDetails;
            return;
        }

        TestAttempt = observableTestAttempt;

        SelectedQuestion = TestAttempt.Test!.Questions.FirstOrDefault();

        if (TestAttempt.Test.HasTimeLimit)
        {
            HasTimer = true;
            var timeDifference = DateTime.Now.Ticks - TestAttempt.StartedAt.Ticks;
            var timelimit = TestAttempt.Test.TimeLimit!.Value.Ticks;
            if (timeDifference > timelimit)
            {
                TestAttempt.EndedAt = DateTime.Now;
                TestAttempt.Status = TestAttemptStatus.Failed;
                TestAttempt.HasGrade = true;

                Save();

                _sessionContext.CurrentTestAttemptId = null;
                _sessionContext.CurrentState = AppState.TestDetails;
            }
            else
            {
                Timeout = TimeSpan.FromTicks(timelimit - timeDifference);
                _timer = new DispatcherTimer();
                _timer.Tick += Timer_Tick;
                _timer.Interval = TimeSpan.FromMilliseconds(100);
                _timer.Start();
            }
        }
    }

    public int SelectedQuestionNumber => SelectedQuestion != null ? TestAttempt.Test!.Questions.IndexOf(SelectedQuestion) + 1 : 0;
    public bool IsSelectedQuestionFirst => SelectedQuestion != null && TestAttempt.Test!.Questions.IndexOf(SelectedQuestion) == 0;
    public bool IsSelectedQuestionLast => SelectedQuestion != null && TestAttempt.Test!.Questions.IndexOf(SelectedQuestion) + 1 == TestAttempt.Test.Questions.Count;
    public bool IsSelectedQuestionNotFirst => !IsSelectedQuestionFirst;
    public bool IsSelectedQuestionNotLast => !IsSelectedQuestionLast;


    [RelayCommand]
    private void Back()
    {
        _timer?.Stop();
        Save();
        _sessionContext.CurrentTestAttemptId = null;
        _sessionContext.CurrentState = AppState.TestDetails;
    }

    [RelayCommand]
    private void Next()
    {
        if (SelectedQuestion == null)
        {
            SelectedQuestion = TestAttempt.Test!.Questions.First();
            return;
        }

        SelectedQuestion = TestAttempt.Test!.Questions[TestAttempt.Test.Questions.IndexOf(SelectedQuestion) + 1];
    }

    [RelayCommand]
    private void Previous()
    {
        if (SelectedQuestion == null)
        {
            SelectedQuestion = TestAttempt.Test!.Questions.First();
            return;
        }

        SelectedQuestion = TestAttempt.Test!.Questions[TestAttempt.Test.Questions.IndexOf(SelectedQuestion) - 1];
    }

    [RelayCommand]
    private void Finish()
    {
        MessageBoxResult answer;
        if (TestAttempt.Test!.Questions.Any(q => q.State == QuestionState.None) || !SelectedQuestion!.AnswerOptions.Any(o => o.IsChecked))
        {
            answer = MessageBox.Show("Ви впевнені що бажаєте завершити тест? На деякі питання не було надано відповіді", "Завершити тест?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (answer == MessageBoxResult.No)
            {
                return;
            }
        }
        else
        {
            answer = MessageBox.Show("Ви впевнені що бажаєте завершити тест?", "Завершити тест?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (answer == MessageBoxResult.No)
            {
                return;
            }
        }

        _timer?.Stop();
        TestAttempt.EndedAt = DateTime.Now;
        TestAttempt.Status = TestAttemptStatus.Completed;
        TestAttempt.HasGrade = true;
        Save();

        _sessionContext.CurrentTestAttemptId = null;
        _sessionContext.CurrentState = AppState.TestDetails;
    }

    partial void OnSelectedQuestionChanged(ObservableQuestion? oldValue, ObservableQuestion? newValue)
    {
        if (oldValue != null)
        {
            if (oldValue.AnswerOptions.Any(o => o.IsChecked))
            {
                oldValue.State = QuestionState.Answered;
            }
            else
            {
                oldValue.State = QuestionState.None;
            }
        }

        if (newValue != null)
        {
            newValue.State = QuestionState.Selected;
        }
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        var timeDifference = DateTime.Now.Ticks - TestAttempt.StartedAt.Ticks;
        var timelimit = TestAttempt.Test!.TimeLimit!.Value.Ticks;
        if (timeDifference > timelimit)
        {
            _timer!.Stop();

            TestAttempt.EndedAt = DateTime.Now;
            TestAttempt.Status = TestAttemptStatus.Failed;
            TestAttempt.HasGrade = true;

            Save();

            _sessionContext.CurrentTestAttemptId = null;
            _sessionContext.CurrentState = AppState.TestDetails;
        }
        else
        {
            Timeout = TimeSpan.FromTicks(timelimit - timeDifference);
        }
    }

    private void Save()
    {
        if (TestAttempt.Status == TestAttemptStatus.Completed && TestAttempt.Test!.HasTimeLimit && Timeout > TimeSpan.Zero)
        {
            TestAttempt.HasLeftoverTime = true;
            TestAttempt.LeftoverTime = new DateTime() + Timeout;
        }

        foreach (var observableQuestion in TestAttempt.Test!.Questions)
        {
            foreach (var observableAnswerOption in observableQuestion.AnswerOptions)
            {
                var get = _answerOptionService.GetById(observableAnswerOption.AnswerOptionId);
                if (!get.IsSuccess)
                {
                    MessageBox.Show("Виникла критична помилка\n" + get.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var answerOption = get.Value;

                // Get user answer (return if error)
                if (!TryGetUserAnswer(answerOption, out var userAnswer))
                {
                    return;
                }

                if (userAnswer == null && !TryCreateUserAnswer(answerOption, observableAnswerOption.IsChecked))
                {
                    // If user answer is null, create new user answer (return if error)
                    return;
                }
                else if (userAnswer != null)
                {
                    // If user answer is not null, set user answer IsSelected to observableAnswerOption.IsChecked
                    userAnswer.IsSelected = observableAnswerOption.IsChecked;

                    // update userAnswer
                    var update = _userAnswerService.Update(userAnswer);
                    if (!update.IsSuccess)
                    {
                        MessageBox.Show("Виникла критична помилка\n" + update.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
        TestAttempt.Save(_testAttemptService, _questionService);
    }

    private bool TryCreateUserAnswer(AnswerOption answerOption, bool isChecked)
    {
        if (!TryGetCurrentTestAttempt(out var testAttempt))
        {
            return false;
        }

        var userAnswer = new UserAnswer()
        {
            TestAttempt = testAttempt,
            User = testAttempt.User,
            Question = answerOption.Question,
            AnswerOption = answerOption,
            IsSelected = isChecked
        };

        var create = _userAnswerService.Add(userAnswer);
        if (!create.IsSuccess)
        {
            MessageBox.Show("Виникла критична помилка\n" + create.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        return true;
    }
    private bool TryGetCurrentObservableTestAttempt(out ObservableTestAttempt observableTestAttempt)
    {
        observableTestAttempt = null!;

        if (!TryGetCurrentTestAttempt(out var testAttempt)
            || !TryGetCurrentTest(out var test)
            || !TryGetQuestions(test, out var questions))
        {
            return false;
        }

        observableTestAttempt = new ObservableTestAttempt(testAttempt)
        {
            Test = new ObservableTest(test)
        };

        foreach (var question in questions)
        {
            var observableQuestion = new ObservableQuestion(question);

            if (!TryGetAnswerOptions(question, out var answerOptions))
            {
                return false;
            }

            foreach (var answerOption in answerOptions)
            {
                if (!TryGetUserAnswer(answerOption, out var userAnswer))
                {
                    return false;
                }

                var observableAnswerOption = new ObservableAnswerOption(answerOption);
                if (userAnswer == null)
                {
                    observableAnswerOption.IsChecked = false;
                }
                else
                {
                    if (observableQuestion.State != QuestionState.Answered)
                    {
                        observableQuestion.State = userAnswer.IsSelected ? QuestionState.Answered : QuestionState.None;
                    }

                    observableAnswerOption.IsChecked = userAnswer.IsSelected;
                }

                observableQuestion.AnswerOptions.Add(observableAnswerOption);
            }

            observableTestAttempt.Test.Questions.Add(observableQuestion);
        }

        return true;
    }
    private bool TryGetCurrentTestAttempt(out TestAttempt testAttempt)
    {
        testAttempt = null!;

        var get = _testAttemptService.GetById(_sessionContext.CurrentTestAttemptId!.Value);
        if (!get.IsSuccess)
        {
            MessageBox.Show("Виникла критична помилка\n" + get.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        testAttempt = get.Value;
        return true;
    }
    private bool TryGetCurrentTest(out Test test)
    {
        test = null!;

        var get = _testService.GetById(_sessionContext.CurrentTestId!.Value);
        if (!get.IsSuccess)
        {
            MessageBox.Show("Виникла критична помилка\n" + get.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        test = get.Value;
        return true;
    }
    private bool TryGetQuestions(Test test, out List<Question> questions)
    {
        questions = [];

        var get = _questionService.Get(q => q.Test.Id == test.Id);
        if (!get.IsSuccess)
        {
            MessageBox.Show("Виникла критична помилка\n" + get.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        questions = get.Value.ToList();
        return true;
    }
    private bool TryGetAnswerOptions(Question question, out List<AnswerOption> answerOptions)
    {
        answerOptions = [];

        var get = _answerOptionService.GetAllOptionsForQuestion(question);
        if (!get.IsSuccess)
        {
            MessageBox.Show("Виникла критична помилка\n" + get.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        answerOptions = get.Value.ToList();
        return true;
    }
    private bool TryGetUserAnswer(AnswerOption answerOption, out UserAnswer? userAnswer)
    {
        userAnswer = null;

        var get = _userAnswerService.Get(a => a.AnswerOption.Id == answerOption.Id && a.TestAttempt.Id == _sessionContext.CurrentTestAttemptId && a.User.Id == _sessionContext.CurrentUserId!.Value);
        if (!get.IsSuccess)
        {
            MessageBox.Show("Виникла критична помилка\n" + get.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        userAnswer = get.Value.FirstOrDefault();
        return true;
    }
}
