using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Enums;
using Core.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using UI.Enums;
using UI.ObservableModels;

namespace UI.ViewModels;

//public partial class TeacherTestBrowserViewModel : ObservableObject
//{
//    [ObservableProperty]
//    private ObservableCollection<ObservableTest> _tests = [];

//    [ObservableProperty]
//    private ObservableCollection<ObservableTest> _filteredTests = [];

//    [ObservableProperty]
//    private string _searchText = string.Empty;

//    [ObservableProperty]
//    private ObservableTest? _selectedTest;

//    public TeacherTestBrowserViewModel()
//    {
//        LoadTests();
//    }

//    private void LoadTests()
//    {
//        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;
//        var user = context.CurrentUser;

//        var getTests = ServiceProvider.TestService.Get();
//        if (!getTests.IsSuccess)
//        {
//            MessageBox.Show(getTests.ErrorMessage);
//            return;
//        }

//        var tests = getTests.Value.ToList();

//        foreach (var test in tests)
//        {
//            var observableTest = new ObservableTest(test);

//            var getQuestions = ServiceProvider.QuestionService.Get(q => q.Test.Id == test.Id);
//            if (!getQuestions.IsSuccess)
//            {
//                MessageBox.Show(getQuestions.ErrorMessage);
//                return;
//            }

//            var questions = getQuestions.Value.ToList();

//            foreach (var question in questions)
//            {
//                var observableQuestion = new ObservableQuestion(question);

//                var getAnswerOption = ServiceProvider.AnswerOptionService.Get(o => o.Question.Id == question.Id);
//                if (!getAnswerOption.IsSuccess)
//                {
//                    MessageBox.Show(getAnswerOption.ErrorMessage);
//                    return;
//                }

//                var answerOptions = getAnswerOption.Value.ToList();

//                foreach (var answerOption in answerOptions)
//                {
//                    var observableAnswerOption = new ObservableAnswerOption(answerOption, null);

//                    observableQuestion.AnswerOptions.Add(observableAnswerOption);
//                }

//                observableTest.Questions.Add(observableQuestion);
//            }

//            Tests.Add(observableTest);
//        }

//        FilteredTests = new ObservableCollection<ObservableTest>(Tests);
//    }

//    [RelayCommand]
//    private static void LogOut()
//    {
//        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;

//        context.CurrentUser = null;
//        context.CurrentState = AppState.LogIn;
//    }

//    [RelayCommand]
//    private void Back()
//    {
//        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;

//        context.CurrentState = AppState.TeacherMenu;
//    }

//    [RelayCommand]
//    private void Search()
//    {
//        if (string.IsNullOrWhiteSpace(SearchText))
//        {
//            FilteredTests = new ObservableCollection<ObservableTest>(Tests);
//        }
//        else
//        {
//            FilteredTests = new ObservableCollection<ObservableTest>(Tests.Where(t => t.Model.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
//        }
//    }

//    [RelayCommand]
//    private void CreateNewTest()
//    {
//        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;

//        var getGroup = ServiceProvider.StudentGroupService.Get();
//        if (!getGroup.IsSuccess)
//        {
//            MessageBox.Show("При отриманні групи виникла помилка");
//            Trace.WriteLine(getGroup.ErrorMessage);
//            return;
//        }

//        var group = getGroup.Value.FirstOrDefault();
//        if (group == null)
//        {
//            MessageBox.Show("Неможливо створити тест. Не існує жодної групи");
//            return;
//        }

//        var newTest = new Test()
//        {
//            Name = string.Empty,
//            Creator = context.CurrentUser!,
//            Status = TestStatus.Draft,
//            MaxAttempts = 1,
//            StudentGroup = group
//        };

//        var createTest = ServiceProvider.TestService.Add(newTest);
//        if (!createTest.IsSuccess)
//        {
//            MessageBox.Show("При створені нового тесту виникла помилка");
//            Trace.WriteLine(createTest.ErrorMessage);
//            return;
//        }
//        context.CurrentTest = newTest;

//        var question = new Question()
//        {
//            Content = "",
//            Type = QuestionType.SingleChoice,
//            GradeValue = 0,
//            Test = newTest
//        };

//        var createQuestion = ServiceProvider.QuestionService.Add(question);
//        if (!createQuestion.IsSuccess)
//        {
//            MessageBox.Show("При створені нового питання виникла помилка");
//            Trace.WriteLine(createQuestion.ErrorMessage);
//            return;
//        }

//        var answerOptions = new List<AnswerOption>
//        {
//            new()
//            {
//                Content = string.Empty,
//                Question = question
//            },
//            new()
//            {
//                Content = string.Empty,
//                Question = question
//            }
//        };

//        foreach (var answerOption in answerOptions)
//        {
//            var createAnswerOptions = ServiceProvider.AnswerOptionService.Add(answerOption);
//            if (!createAnswerOptions.IsSuccess)
//            {
//                MessageBox.Show("При створені нового варіанту відповіді виникла помилка");
//                Trace.WriteLine(createAnswerOptions.ErrorMessage);
//                return;
//            }
//        }

//        context.CurrentTest = newTest;
//        context.CurrentState = AppState.TestEditor;
//    }

//    [RelayCommand]
//    private void ModifyTest()
//    {
//        if (SelectedTest == null)
//        {
//            return;
//        }

//        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;

//        context.CurrentTest = SelectedTest.Model;
//        context.CurrentState = AppState.TestEditor;
//    }

//    [RelayCommand]
//    private void RemoveTest()
//    {
//        if (SelectedTest == null)
//        {
//            return;
//        }

//        var answer = MessageBox.Show("Ви впевнені що бажаєте видалити тест?", "Видалення тесу", MessageBoxButton.YesNo);
//        if (answer != MessageBoxResult.Yes)
//        {
//            return;
//        }

//        var remove = ServiceProvider.TestService.Remove(t => t.Id == SelectedTest.Model.Id);
//        if (!remove.IsSuccess)
//        {
//            MessageBox.Show("Щось пішло не так. Тест не видалено");
//            Trace.WriteLine(remove.ErrorMessage);
//            return;
//        }

//        Tests.Remove(SelectedTest);
//        FilteredTests.Remove(SelectedTest);
//        SelectedTest = FilteredTests.FirstOrDefault();
//    }
//}
