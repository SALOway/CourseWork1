﻿using BLL.Interfaces;
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
    private ObservableCollection<ObservableTest> _filteredTests = [];

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private ObservableTest? _selectedTest;

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
        if (!TryGetCurrentUser(out var user) || !TryGetObservableTests(user, out var tests))
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
    private void Back()
    {
        _sessionContext.CurrentState = AppState.TeacherMenu;
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
            Creator = user,
            Status = TestStatus.Draft,
            MaxAttempts = 1,
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
            MessageBox.Show("Не існує жодної групи");
            return false;
        }

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

    private bool TryGetObservableTests(User user, out ObservableCollection<ObservableTest> observableTests)
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

    private bool TryGetLastUserAttempt(Test test, out TestAttempt? lastTestAttempt)
    {
        lastTestAttempt = null;

        var getTestAttempt = _testAttemptService.GetLastAttempt(_sessionContext.CurrentUserId!.Value, test.Id);
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

        var getUserAnswers = _userAnswerService.Get(a => a.AnswerOption.Id == answerOption.Id && a.User.Id == _sessionContext.CurrentUserId);

        if (!getUserAnswers.IsSuccess)
        {
            MessageBox.Show("Виникла критична помилка\n" + getUserAnswers.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }

        userAnswer = getUserAnswers.Value.FirstOrDefault();

        return true;
    }
}
