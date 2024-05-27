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

public partial class TestEditorViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<ObservableQuestion> _questions = [];

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
    private QuestionType? _selectedQuestionType;

    [ObservableProperty]
    public ObservableCollection<TestStatus> _testStatuses = [];

    [ObservableProperty]
    private TestStatus? _selectedTestStatus;

    [ObservableProperty]
    public ObservableCollection<StudentGroup> _studentGroups = [];

    [ObservableProperty]
    private StudentGroup? _selectedStudentGroup;

    [ObservableProperty]
    private DateTime? _timeLimit;

    public TestEditorViewModel()
    {
        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;

        QuestionTypes = new ObservableCollection<QuestionType>((QuestionType[])Enum.GetValues(typeof(QuestionType)));
        TestStatuses = new ObservableCollection<TestStatus>((TestStatus[])Enum.GetValues(typeof(TestStatus)));
        SelectedTestStatus = TestStatuses.First();

        var getGroups = ServiceProvider.StudentGroupService.Get();
        if (!getGroups.IsSuccess)
        {
            MessageBox.Show("При завантажені груп виникла помилка");
            Trace.WriteLine(getGroups.ErrorMessage);
            return;
        }

        StudentGroups = new ObservableCollection<StudentGroup>(getGroups.Value);

        var currentTest = context.CurrentTest;

        var questions = new List<ObservableQuestion>();
        if (currentTest != null)
        {
            var getQuestions = ServiceProvider.QuestionService.Get(q => q.Test.Id == currentTest.Id);
            if (!getQuestions.IsSuccess)
            {
                MessageBox.Show("При завантажені питань виникла помилка");
                Trace.WriteLine(getQuestions.ErrorMessage);
                return;
            }

            questions = getQuestions.Value.Select(q => new ObservableQuestion(q)).ToList();

            if (currentTest.HasTimer)
            {
                TimeLimit = new DateTime(currentTest.TimeLimit!.Value.Ticks).ToLocalTime();
            }

            SelectedStudentGroup = currentTest.StudentGroup;
            SelectedTestStatus = currentTest.Status;
        }
        else
        {
            var question = CreateNewQuestion();
            questions = [new ObservableQuestion(question)];
        }

        Questions = new ObservableCollection<ObservableQuestion>(questions);
        SelectedQuestion = Questions.First();
    }

    public int SelectedQuestionNumber => SelectedQuestion != null ? Questions.IndexOf(SelectedQuestion) + 1 : 0;
    public bool IsSelectedQuestionFirst => SelectedQuestion != null && Questions.IndexOf(SelectedQuestion) == 0;
    public bool IsSelectedQuestionLast => SelectedQuestion != null && Questions.IndexOf(SelectedQuestion) + 1 == Questions.Count;
    public bool IsSelectedQuestionNotFirst => !IsSelectedQuestionFirst;
    public bool IsSelectedQuestionNotLast => !IsSelectedQuestionLast;
    public Test CurrentTest
    {
        get
        {
            var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;
            return context.CurrentTest!;
        }
    }

    partial void OnSelectedQuestionChanged(ObservableQuestion? value)
    {
        if (value == null)
        {
            return;
        }

        SelectedQuestionType = value.Model.Type;
    }

    partial void OnSelectedQuestionTypeChanged(QuestionType? value)
    {
        if (SelectedQuestion == null || value == null)
        {
            return;
        }

        SelectedQuestion.Model.Type = value.Value;

        var previous = SelectedQuestion.AnswerOptions;
        var options = SelectedQuestion.AnswerOptions.ToDictionary(a => a.Model, a => a.IsChecked);

        SelectedQuestion.AnswerOptions = new ObservableCollection<ObservableAnswerOption>(SelectedQuestion.AnswerOptions.Select(x => new ObservableAnswerOption(x.Model)));

        var checkedOptions = previous.Where(a => a.IsChecked);
        if (checkedOptions.Any() && value == QuestionType.SingleChoice)
        {
            SelectedQuestion.AnswerOptions.First(a => a.Model == checkedOptions.First().Model).IsChecked = true;
        }
        else
        {
            foreach (var option in SelectedQuestion.AnswerOptions)
            {
                option.IsChecked = options[option.Model];
            }
        }
    }

    partial void OnTimeLimitChanged(DateTime? value)
    {
        if (value == null)
        {
            CurrentTest.TimeLimit = null!;
        }
        else
        {
            CurrentTest.TimeLimit = value.Value.ToUniversalTime().TimeOfDay;
        }
    }

    [RelayCommand]
    private void AddQuestion()
    {
        var question = CreateNewQuestion();

        Questions.Add(new ObservableQuestion(question));
        SelectedQuestion = Questions.LastOrDefault();
    }


    [RelayCommand]
    private void RemoveQuestion()
    {
        if (Questions.Count <= 1 || SelectedQuestion == null)
        {
            return;
        }

        Questions.Remove(SelectedQuestion);
        SelectedQuestion = Questions.LastOrDefault();
    }

    [RelayCommand]
    private void Next()
    {
        if (Questions.Count == 0 || SelectedQuestion == null)
        {
            SelectedQuestion = Questions.First();
            return;
        }

        SelectedQuestion = Questions[Questions.IndexOf(SelectedQuestion) + 1];
    }

    [RelayCommand]
    private void Previous()
    {
        if (Questions.Count == 0 || SelectedQuestion == null)
        {
            SelectedQuestion = Questions.First();
            return;
        }

        SelectedQuestion = Questions[Questions.IndexOf(SelectedQuestion) - 1];
    }

    [RelayCommand]
    private void AddAnswerOption()
    {
        if (Questions.Count == 0 || SelectedQuestion == null)
        {
            return;
        }

        var answerOption = new ObservableAnswerOption(new AnswerOption()
        {
            Content = string.Empty,
            Question = SelectedQuestion.Model
        });
        SelectedQuestion.Model.AnswerOptions.Add(answerOption.Model);
        SelectedQuestion.AnswerOptions.Add(answerOption);
        SelectedAnswerOption = answerOption;
    }

    [RelayCommand]
    private void RemoveAnswerOption()
    {
        if (SelectedQuestion == null || SelectedQuestion.AnswerOptions.Count <= 2 || SelectedAnswerOption == null)
        {
            return;
        }

        SelectedQuestion.Model.AnswerOptions.Remove(SelectedAnswerOption.Model);
        SelectedQuestion.AnswerOptions.Remove(SelectedAnswerOption);
        SelectedAnswerOption = SelectedQuestion.AnswerOptions.LastOrDefault();
    }

    [RelayCommand]
    private void SaveTest()
    {
        MessageBox.Show("Save my ass");
    }

    [RelayCommand]
    private void Back()
    {
        // Ensure that user want to save data
        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;

        context.CurrentTest = null;
        context.CurrentState = AppState.TeacherTestBrowser;
    }

    private Question CreateNewQuestion()
    {
        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;

        var question = new Question()
        {
            Content = "",
            Type = QuestionType.SingleChoice,
            GradeValue = 0,
            Test = context.CurrentTest!
        };

        question.AnswerOptions = new List<AnswerOption>
        {
            new()
            {
                Content = string.Empty,
                Question = question
            },
            new()
            {
                Content = string.Empty,
                Question = question
            }
        };

        return question;
    }
}
