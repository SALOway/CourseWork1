using BLL.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Enums;
using Core.Models;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using UI.Enums;
using UI.Interfaces;
using UI.ObservableModels;

namespace UI.ViewModels;

public partial class TestEditorViewModel : ObservableValidator
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
    private ObservableTest _test;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSelectedQuestionNotFirst))]
    [NotifyPropertyChangedFor(nameof(IsSelectedQuestionNotLast))]
    [NotifyPropertyChangedFor(nameof(SelectedQuestionNumber))]
    private ObservableQuestion? _selectedQuestion;

    [ObservableProperty]
    private ObservableAnswerOption? _selectedAnswerOption;

    [ObservableProperty]
    public ObservableCollection<QuestionType> _questionTypes = [];

    [ObservableProperty]
    public ObservableCollection<TestStatus> _testStatuses = [];

    [ObservableProperty]
    private TestStatus? _selectedTestStatus;

    [ObservableProperty]
    public ObservableCollection<ObservableStudentGroup> _studentGroups = [];

    [ObservableProperty]
    private ObservableStudentGroup? _selectedStudentGroup;

    [ObservableProperty]
    [Range(0, int.MaxValue, ErrorMessage = "Значення годин не може бути від'ємним")]
    private int? _timeLimitHours;

    [ObservableProperty]
    [Range(0, 59, ErrorMessage = "Значення хвилин має бути у проміжку від 0 до 59")]
    private int? _timeLimitMinutes;

    [ObservableProperty]
    [Range(0, 59, ErrorMessage = "Значення секунд має бути у проміжку від 0 до 59")]
    private int? _timeLimitSeconds;

    public TestEditorViewModel(ISessionContext sessionContext, IUserService userService, ITestService testService, IQuestionService questionService, IAnswerOptionService answerOptionService, IUserAnswerService userAnswerService, ITestAttemptService testAttemptService, IStudentGroupService studentGroupService)
    {
        _sessionContext = sessionContext;
        _userService = userService;
        _testService = testService;
        _questionService = questionService;
        _answerOptionService = answerOptionService;
        _userAnswerService = userAnswerService;
        _testAttemptService = testAttemptService;
        _studentGroupService = studentGroupService;

        QuestionTypes = new ObservableCollection<QuestionType>((QuestionType[])Enum.GetValues(typeof(QuestionType)));
        TestStatuses = new ObservableCollection<TestStatus>((TestStatus[])Enum.GetValues(typeof(TestStatus)));
        SelectedTestStatus = TestStatuses.First();

        if (!TryGetObservableTest(out var test) || !TryGetObservableStudentGroups(out var studentGroups))
        {
            Test = null!;
            Back();
            return;
        }

        Test = test;
        StudentGroups = studentGroups;
        TimeLimitHours = Test.TimeLimit?.Hour;
        TimeLimitMinutes = Test.TimeLimit?.Minute;
        TimeLimitSeconds = Test.TimeLimit?.Second;

        SelectedStudentGroup = StudentGroups.First(g => g.StudentGroupId == Test.StudentGroup.StudentGroupId);
        SelectedTestStatus = Test.Status;
        SelectedQuestion = Test.Questions.First();
    }

    public int SelectedQuestionNumber => SelectedQuestion != null ? Test.Questions.IndexOf(SelectedQuestion) + 1 : 0;
    public bool IsSelectedQuestionFirst => SelectedQuestion != null && Test.Questions.IndexOf(SelectedQuestion) == 0;
    public bool IsSelectedQuestionLast => SelectedQuestion != null && Test.Questions.IndexOf(SelectedQuestion) + 1 == Test.Questions.Count;
    public bool IsSelectedQuestionNotFirst => !IsSelectedQuestionFirst;
    public bool IsSelectedQuestionNotLast => !IsSelectedQuestionLast;

    partial void OnSelectedStudentGroupChanging(ObservableStudentGroup? value)
    {
        if (value != null && Test != null)
        {
            Test.StudentGroup = value;
        }
    }

    partial void OnSelectedTestStatusChanged(TestStatus? value)
    {
        if (value != null && Test != null)
        {
            Test.Status = value.Value;
        }
    }

    [RelayCommand]
    private void Next()
    {
        if (Test.Questions.Count == 0 || SelectedQuestion == null)
        {
            SelectedQuestion = Test.Questions.First();
            return;
        }

        SelectedQuestion = Test.Questions[Test.Questions.IndexOf(SelectedQuestion) + 1];
    }

    [RelayCommand]
    private void Previous()
    {
        if (Test.Questions.Count == 0 || SelectedQuestion == null)
        {
            SelectedQuestion = Test.Questions.First();
            return;
        }

        SelectedQuestion = Test.Questions[Test.Questions.IndexOf(SelectedQuestion) - 1];
    }

    [RelayCommand]
    private void SaveTest()
    {
        if (Test.HasTimeLimit)
        {
            var timeLimit = new DateTime().AddHours(TimeLimitHours ?? 0)
                                          .AddMinutes(TimeLimitMinutes ?? 0)
                                          .AddSeconds(TimeLimitSeconds ?? 0);
            Test.TimeLimit = timeLimit;
        }

        Test.Save(_testService, _studentGroupService);
        foreach (var question in Test.Questions)
        {
            question.Save(_questionService);
            foreach (var answerOption in question.AnswerOptions)
            {
                SaveAnswerOption(answerOption);
            }
        }

        MessageBox.Show("Дані були збережені", "Збереження данних", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    private void Back()
    {
        var answer = MessageBox.Show("Зберегти дані перед поверненням?", "Збереження даних", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

        switch (answer)
        {
            case MessageBoxResult.Yes:
                SaveTest();
                break;
            case MessageBoxResult.No:
                break;
            default:
                return;
        }

        _sessionContext.CurrentTestId = null;
        _sessionContext.CurrentState = AppState.TeacherTestBrowser;
    }

    [RelayCommand]
    private void AddQuestion()
    {
        if (!TryCreateObservableQuestion(out var observableQuestion))
        {
            return;
        }

        Test.Questions.Add(observableQuestion);
        SelectedQuestion = Test.Questions.LastOrDefault();
    }

    [RelayCommand]
    private void RemoveQuestion()
    {
        if (Test.Questions.Count <= 1 || SelectedQuestion == null)
        {
            return;
        }

        var removeQuestion = _questionService.RemoveById(SelectedQuestion.QuestionId);
        if (!removeQuestion.IsSuccess)
        {
            MessageBox.Show("Виникла критична помилка\n" + removeQuestion.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        Test.Questions.Remove(SelectedQuestion);
        SelectedQuestion = Test.Questions.LastOrDefault();
    }

    [RelayCommand]
    private void AddAnswerOption()
    {
        if (Test.Questions.Count == 0 || SelectedQuestion == null || !TryCreateObservableAnswerOption(out var answerOption))
        {
            return;
        }

        SelectedQuestion.AnswerOptions.Add(answerOption);
        SelectedAnswerOption = SelectedQuestion.AnswerOptions.LastOrDefault();
    }

    [RelayCommand]
    private void RemoveAnswerOption()
    {
        if (SelectedQuestion == null || SelectedQuestion.AnswerOptions.Count <= 2 || SelectedAnswerOption == null)
        {
            return;
        }

        var removeAnswerOption = _answerOptionService.RemoveById(SelectedAnswerOption.AnswerOptionId);
        if (!removeAnswerOption.IsSuccess)
        {
            MessageBox.Show("Виникла критична помилка\n" + removeAnswerOption.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        SelectedQuestion.AnswerOptions.Remove(SelectedAnswerOption);
        SelectedAnswerOption = SelectedQuestion.AnswerOptions.LastOrDefault();
    }

    // Service Logic?

    private bool TryCreateQuestion(out Question newQuestion)
    {
        newQuestion = null!;

        var getTest = _testService.GetById(_sessionContext.CurrentTestId!.Value);
        if (!getTest.IsSuccess)
        {
            MessageBox.Show("Виникла критична помилка\n" + getTest.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        var test = getTest.Value;

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
    private bool TryCreateObservableQuestion(out ObservableQuestion observableQuestion)
    {
        observableQuestion = null!;

        if (!TryCreateQuestion(out var question) || !TryGetObservableAnswerOptions(question, out var observableAnswersOptions))
        {
            return false;
        }

        observableQuestion = new ObservableQuestion(question)
        {
            AnswerOptions = observableAnswersOptions,
        };

        return true;
    }
    private bool TryCreateObservableAnswerOption(out ObservableAnswerOption observableAnswerOption)
    {
        observableAnswerOption = null!;

        if (!TryGetSelectedQuestion(out var selectedQuestion))
        {
            return false;
        }

        var answerOption = new AnswerOption()
        {
            Content = string.Empty,
            Question = selectedQuestion
        };

        var add = _answerOptionService.Add(answerOption);
        if (!add.IsSuccess)
        {
            MessageBox.Show("Виникла критична помилка\n" + add.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        observableAnswerOption = new ObservableAnswerOption(answerOption);

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
    private bool TryGetSelectedQuestion(out Question question)
    {
        question = null!;

        var getSelectedQuestion = _questionService.GetById(SelectedQuestion!.QuestionId);
        if (!getSelectedQuestion.IsSuccess)
        {
            MessageBox.Show("Виникла критична помилка\n" + getSelectedQuestion.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        question = getSelectedQuestion.Value;

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
    private bool TryGetObservableStudentGroups(out ObservableCollection<ObservableStudentGroup> observableStudentGroups)
    {
        observableStudentGroups = [];

        var getGroups = _studentGroupService.Get();
        if (!getGroups.IsSuccess)
        {
            MessageBox.Show("Виникла критична помилка\n" + getGroups.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        var studentGroups = getGroups.Value;

        if (!studentGroups.Any())
        {
            MessageBox.Show("Не існує жодної групи", "Студентських груп не існує", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        observableStudentGroups = new ObservableCollection<ObservableStudentGroup>(studentGroups.Select(g => new ObservableStudentGroup(g)));
        return true;
    }
    private bool TryGetObservableTest(out ObservableTest observableTest)
    {
        observableTest = null!;

        var getCurrentTest = _testService.GetById(_sessionContext.CurrentTestId!.Value);
        if (!getCurrentTest.IsSuccess)
        {
            MessageBox.Show("Виникла критична помилка\n" + getCurrentTest.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        var test = getCurrentTest.Value;

        observableTest = new ObservableTest(test);

        if (!TryGetObservableQuestions(test, out var observableQuestions))
        {
            return false;
        }

        observableTest.Questions = observableQuestions;
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
    private void SaveAnswerOption(ObservableAnswerOption observableAnswerOption)
    {
        var getAnswerOption = _answerOptionService.GetById(observableAnswerOption.AnswerOptionId);
        if (!getAnswerOption.IsSuccess)
        {
            MessageBox.Show("Виникла критична помилка\n" + getAnswerOption.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        observableAnswerOption.IsTrue = observableAnswerOption.IsChecked;
        observableAnswerOption.Save(_answerOptionService);

        var answerOption = getAnswerOption.Value;

        if (!TryGetUserAnswer(answerOption, out var userAnswer))
        {
            return;
        }

        if (userAnswer == null)
        {
            // null exeption for Id of question
            var getQuestion = _questionService.GetById(answerOption.Question.Id);
            if (!getQuestion.IsSuccess)
            {
                MessageBox.Show("Виникла критична помилка\n" + getQuestion.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var question = getQuestion.Value;

            var getTestAttempt = _testAttemptService.GetById(_sessionContext.CurrentTestAttemptId!.Value);
            if (!getTestAttempt.IsSuccess)
            {
                MessageBox.Show("Виникла критична помилка\n" + getTestAttempt.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var testAttempt = getTestAttempt.Value;

            var newUserAnswer = new UserAnswer()
            {
                AnswerOption = answerOption,
                Question = question,
                TestAttempt = testAttempt,
                IsSelected = observableAnswerOption.IsChecked,
                User = testAttempt.User
            };

            var create = _userAnswerService.Add(newUserAnswer);
            if (!create.IsSuccess)
            {
                MessageBox.Show("Виникла критична помилка\n" + create.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
    }
}
