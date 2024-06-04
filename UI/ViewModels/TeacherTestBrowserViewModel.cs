using BLL.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Enums;
using Core.Models;
using System.Collections.ObjectModel;
using System.Windows;
using UI.Enums;
using UI.Interfaces;
using UI.ObservableModels;

namespace UI.ViewModels;

public partial class TeacherTestBrowserViewModel : ObservableObject
{
    private readonly ISessionContext _sessionContext;
    private readonly IUserService _userService;
    private readonly ITestService _testService;
    private readonly IQuestionService _questionService;
    private readonly IAnswerOptionService _answerOptionService;
    private readonly IUserAnswerService _userAnswerService;
    private readonly ITestAttemptService _testAttemptService;
    private readonly IStudentGroupService _studentGroupService;

    [ObservableProperty]
    private ObservableCollection<ObservableTest> _tests = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AnyTest))]
    [NotifyPropertyChangedFor(nameof(NoAnyTest))]
    private ObservableCollection<ObservableTest> _filteredTests = [];

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private ObservableTest? _selectedTest;

    public bool AnyTest => FilteredTests.Any();
    public bool NoAnyTest => !AnyTest;

    public TeacherTestBrowserViewModel(ISessionContext sessionContext, IUserService userService, ITestService testService, IQuestionService questionService, IAnswerOptionService answerOptionService, IUserAnswerService userAnswerService, ITestAttemptService testAttemptService, IStudentGroupService studentGroupService)
    {
        _sessionContext = sessionContext;
        _userService = userService;
        _testService = testService;
        _questionService = questionService;
        _answerOptionService = answerOptionService;
        _userAnswerService = userAnswerService;
        _testAttemptService = testAttemptService;
        _studentGroupService = studentGroupService;

        LoadTests();
    }

    private void LoadTests()
    {
        if (!TryGetObservableTests(out var tests))
        {
            LogOut();
            return;
        }

        Tests = tests;
        FilteredTests = new ObservableCollection<ObservableTest>(Tests);
    }

    [RelayCommand]
    private void LogOut()
    {
        _sessionContext.CurrentUserId = null;
        _sessionContext.CurrentState = AppState.LogIn;
    }

    [RelayCommand]
    private void Search()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            FilteredTests = new ObservableCollection<ObservableTest>(Tests);
        }
        else
        {
            FilteredTests = new ObservableCollection<ObservableTest>(Tests.Where(t => t.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
        }
    }

    [RelayCommand]
    private void CreateNewTest()
    {
        var answer = MessageBox.Show("Ви впевнені що бажаєте cтворити новий тест?", "Створення тесу", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (answer != MessageBoxResult.Yes)
        {
            return;
        }

        if (!TryCreateNewTest(out var newTest))
        {
            return;
        }

        Tests.Add(new ObservableTest(newTest));
        SelectedTest = Tests.First();

        if (!TryCreateTestAttempt(out var newAttempt))
        {
            return;
        }

        _sessionContext.CurrentTestAttemptId = newAttempt.Id;
        _sessionContext.CurrentTestId = newTest.Id;
        _sessionContext.CurrentState = AppState.TestEditor;
    }

    [RelayCommand]
    private void ModifyTest()
    {
        if (SelectedTest == null)
        {
            return;
        }

        if (!TryGetTestAttempt(out var attempt))
        {
            return;
        }

        if (attempt == null)
        {
            if (!TryCreateTestAttempt(out var newAttempt))
            {
                return;
            }

            _sessionContext.CurrentTestAttemptId = newAttempt.Id;
        }
        else
        {
            _sessionContext.CurrentTestAttemptId = attempt.Id;
        }

        _sessionContext.CurrentTestId = SelectedTest.TestId;
        _sessionContext.CurrentState = AppState.TestEditor;
    }

    [RelayCommand]
    private void RemoveTest()
    {
        if (SelectedTest == null)
        {
            return;
        }

        var answer = MessageBox.Show("Ви впевнені що бажаєте видалити тест?", "Видалення тесу", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (answer != MessageBoxResult.Yes)
        {
            return;
        }

        var remove = _testService.RemoveById(SelectedTest.TestId);
        if (!remove.IsSuccess)
        {
            MessageBox.Show("Виникла критична помилка\n" + remove.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        Tests.Remove(SelectedTest);
        FilteredTests.Remove(SelectedTest);
        SelectedTest = FilteredTests.FirstOrDefault();
    }

    // Service Logic?

    private bool TryCreateNewTest(out Test newTest)
    {
        newTest = null!;

        if (!TryGetCurrentUser(out var user) || !TryGetAnyFirstGroup(out var group))
        {
            return false;
        }

        newTest = new Test()
        {
            Name = string.Empty,
            Status = TestStatus.Draft,
            MaxAttempts = 1,
            Creator = user,
            StudentGroup = group
        };

        var createTest = _testService.Add(newTest);
        if (!createTest.IsSuccess)
        {
            MessageBox.Show("Виникла критична помилка\n" + createTest.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        if (!TryCreateQuestion(newTest, out var newQuestion))
        {
            return false;
        }

        newTest.Questions = [newQuestion];

        return true;
    }
    private bool TryCreateQuestion(Test test, out Question newQuestion)
    {
        newQuestion = new Question()
        {
            Content = "",
            Type = QuestionType.SingleChoice,
            GradeValue = 0,
            Test = test
        };

        var createQuestion = _questionService.Add(newQuestion);
        if (!createQuestion.IsSuccess)
        {
            MessageBox.Show("Виникла критична помилка\n" + createQuestion.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        if (!TryCreateAnswerOptions(newQuestion, out var answerOptions))
        {
            return false;
        }

        newQuestion.AnswerOptions = answerOptions;

        return true;
    }
    private bool TryCreateAnswerOptions(Question question, out List<AnswerOption> answerOptions)
    {
        answerOptions =
        [
            new()
            {
                Content = string.Empty,
                Question = question
            },
            new()
            {
                Content = string.Empty,
                Question = question
            },
        ];

        foreach (var answerOption in answerOptions)
        {
            var createAnswerOption = _answerOptionService.Add(answerOption);
            if (!createAnswerOption.IsSuccess)
            {
                MessageBox.Show("Виникла критична помилка\n" + createAnswerOption.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        return true;
    }
    private bool TryCreateTestAttempt(out TestAttempt testAttempt)
    {
        testAttempt = null!;
        if (!TryGetCurrentUser(out var user))
        {
            return false;
        }

        if (!TryGetCurrentTest(out var test))
        {
            return false;
        }

        testAttempt = new TestAttempt()
        {
            User = user,
            Test = test,
            Status = TestAttemptStatus.InProcess,
        };

        var add = _testAttemptService.Add(testAttempt);
        if (!add.IsSuccess)
        {
            MessageBox.Show("Виникла критична помилка\n" + add.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        return true;
    }
    private bool TryGetAnyFirstGroup(out StudentGroup studentGroup)
    {
        studentGroup = null!;

        var getGroup = _studentGroupService.Get();
        if (!getGroup.IsSuccess)
        {
            MessageBox.Show("Виникла критична помилка\n" + getGroup.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        var group = getGroup.Value.FirstOrDefault();
        if (group == null)
        {
            MessageBox.Show("Не існує жодної групи", "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        studentGroup = group;

        return true;
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
        else if (user.Role != UserRole.Teacher)
        {
            MessageBox.Show("Користувач не є викладачем.\n", "Помилка ролі користувача", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        return true;
    }
    private bool TryGetCurrentTest(out Test test)
    {
        test = null!;

        var getTest = _testService.GetById(SelectedTest!.TestId);
        if (!getTest.IsSuccess)
        {
            MessageBox.Show("Виникла критична помилка\n" + getTest.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        test = getTest.Value;
        return true;
    }
    private bool TryGetTestAttempt(out TestAttempt? attempt)
    {
        attempt = null!;

        var get = _testAttemptService.Get(a => a.User.Id == _sessionContext.CurrentUserId!.Value && a.Test.Id == SelectedTest!.TestId);
        if (!get.IsSuccess)
        {
            MessageBox.Show("Виникла критична помилка\n" + get.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        attempt = get.Value.FirstOrDefault();
        return true;
    }
    private bool TryGetUserAnswer(AnswerOption answerOption, out UserAnswer? userAnswer)
    {
        userAnswer = null!;

        var getUserAnswers = _userAnswerService.Get(a => a.AnswerOption.Id == answerOption.Id && a.User.Id == _sessionContext.CurrentUserId);

        if (!getUserAnswers.IsSuccess)
        {
            MessageBox.Show("Виникла критична помилка\n" + getUserAnswers.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        userAnswer = getUserAnswers.Value.FirstOrDefault();

        return true;
    }
    private bool TryGetObservableTests(out ObservableCollection<ObservableTest> observableTests)
    {
        observableTests = [];

        var getTests = _testService.Get();
        if (!getTests.IsSuccess)
        {
            MessageBox.Show("Виникла критична помилка\n" + getTests.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        var tests = getTests.Value.ToList();

        foreach (var test in tests)
        {
            var observableTest = new ObservableTest(test);

            if (!TryGetObservableQuestions(test, out var questions))
            {
                return false;
            }

            observableTest.Questions = questions;

            observableTests.Add(observableTest);
        }

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
}
