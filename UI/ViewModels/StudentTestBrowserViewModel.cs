using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using UI.Enums;
using UI.ObservableModels;

namespace UI.ViewModels;

public partial class StudentTestBrowserViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<ObservableTest> _tests = [];

    [ObservableProperty]
    private ObservableCollection<ObservableTest> _filteredTests = [];

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private ObservableTest? _selectedTest;

    public StudentTestBrowserViewModel()
    {
        LoadTests();
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
    private static void LogOut()
    {
        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;

        context.CurrentUser = null;
        context.CurrentState = AppState.LogIn;
    }

    [RelayCommand]
    private void OpenDetails()
    {
        if (SelectedTest == null)
        {
            return;
        }

        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;

        context.CurrentTest = SelectedTest.Model;
        context.CurrentState = AppState.TestDetails;
    }

    partial void OnSelectedTestChanged(ObservableTest? value)
    {
        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;

        context.CurrentTest = value?.Model;
    }

    private void LoadTests()
    {
        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;
        var user = context.CurrentUser;

        var getTests = ServiceProvider.TestService.GetTestsByGroup(user!.StudentGroup!);
        if (!getTests.IsSuccess)
        {
            MessageBox.Show(getTests.ErrorMessage);
            return;
        }

        var tests = getTests.Value.ToList();

        foreach (var test in tests)
        {
            var observableTest = new ObservableTest(test);

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

                foreach(var answerOption in answerOptions)
                {
                    var getUserAnswers = ServiceProvider.UserAnswerService.Get(a => a.AnswerOption.Id == answerOption.Id && a.User.Id == user.Id);
                    var userAnswer = getUserAnswers.Value.FirstOrDefault();
                    var observableAnswerOption = new ObservableAnswerOption(answerOption, userAnswer);

                    observableQuestion.AnswerOptions.Add(observableAnswerOption);
                }

                observableTest.Questions.Add(observableQuestion);
            }

            Tests.Add(observableTest);
        }

        FilteredTests = new ObservableCollection<ObservableTest>(Tests);
    }
}
