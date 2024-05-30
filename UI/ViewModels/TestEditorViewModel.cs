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

//public partial class TestEditorViewModel : ObservableObject
//{
//    [ObservableProperty]
//    private ObservableTest _test;

//    [ObservableProperty]
//    [NotifyPropertyChangedFor(nameof(IsSelectedQuestionNotFirst))]
//    [NotifyPropertyChangedFor(nameof(IsSelectedQuestionNotLast))]
//    [NotifyPropertyChangedFor(nameof(SelectedQuestionNumber))]
//    private ObservableQuestion? _selectedQuestion;

//    [ObservableProperty]
//    private ObservableAnswerOption? _selectedAnswerOption;

//    [ObservableProperty]
//    public ObservableCollection<QuestionType> _questionTypes = [];

//    [ObservableProperty]
//    public ObservableCollection<TestStatus> _testStatuses = [];

//    [ObservableProperty]
//    private TestStatus? _selectedTestStatus;

//    [ObservableProperty]
//    public ObservableCollection<StudentGroup> _studentGroups = [];

//    [ObservableProperty]
//    private StudentGroup? _selectedStudentGroup;

//    [ObservableProperty]
//    private DateTime? _timeLimit;

//    public TestEditorViewModel()
//    {
//        QuestionTypes = new ObservableCollection<QuestionType>((QuestionType[])Enum.GetValues(typeof(QuestionType)));
//        TestStatuses = new ObservableCollection<TestStatus>((TestStatus[])Enum.GetValues(typeof(TestStatus)));
//        SelectedTestStatus = TestStatuses.First();
        
//        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;

//        var test = context.CurrentTest!;
//        Test = new ObservableTest(test);

//        var getQuestions = ServiceProvider.QuestionService.Get(q => q.Test.Id == test.Id);
//        if (!getQuestions.IsSuccess)
//        {
//            MessageBox.Show(getQuestions.ErrorMessage);
//            return;
//        }

//        var questions = getQuestions.Value.ToList();

//        foreach (var question in questions)
//        {
//            var observableQuestion = new ObservableQuestion(question);

//            var getAnswerOption = ServiceProvider.AnswerOptionService.Get(o => o.Question.Id == question.Id);
//            if (!getAnswerOption.IsSuccess)
//            {
//                MessageBox.Show(getAnswerOption.ErrorMessage);
//                return;
//            }

//            var answerOptions = getAnswerOption.Value.ToList();

//            foreach (var answerOption in answerOptions)
//            {
//                var observableAnswerOption = new ObservableAnswerOption(answerOption, null);

//                observableQuestion.AnswerOptions.Add(observableAnswerOption);
//            }

//            Test.Questions.Add(observableQuestion);
//        }

//        var getGroups = ServiceProvider.StudentGroupService.Get();
//        if (!getGroups.IsSuccess)
//        {
//            MessageBox.Show("При завантажені груп виникла помилка");
//            Trace.WriteLine(getGroups.ErrorMessage);
//            return;
//        }

//        StudentGroups = new ObservableCollection<StudentGroup>(getGroups.Value);

//        var currentTest = context.CurrentTest!;
//        if (currentTest.HasTimer)
//        {
//            TimeLimit = new DateTime(currentTest.TimeLimit!.Value.Ticks).ToLocalTime();
//        }

//        SelectedStudentGroup = currentTest.StudentGroup;
//        SelectedTestStatus = currentTest.Status;
//        SelectedQuestion = Test.Questions.First();
//    }

//    public int SelectedQuestionNumber => SelectedQuestion != null ? Test.Questions.IndexOf(SelectedQuestion) + 1 : 0;
//    public bool IsSelectedQuestionFirst => SelectedQuestion != null && Test.Questions.IndexOf(SelectedQuestion) == 0;
//    public bool IsSelectedQuestionLast => SelectedQuestion != null && Test.Questions.IndexOf(SelectedQuestion) + 1 == Test.Questions.Count;
//    public bool IsSelectedQuestionNotFirst => !IsSelectedQuestionFirst;
//    public bool IsSelectedQuestionNotLast => !IsSelectedQuestionLast;

//    partial void OnSelectedStudentGroupChanging(StudentGroup? value)
//    {
//        if (value != null && Test != null)
//        {
//            Test.StudentGroup = value;
//        }
//    }

//    partial void OnSelectedTestStatusChanged(TestStatus? value)
//    {
//        if (value != null && Test != null)
//        {
//            Test.Status = value.Value;
//        }
//    }

//    [RelayCommand]
//    private void AddQuestion()
//    {
//        var question = CreateQuestion();
//        Test.Questions.Add(new ObservableQuestion(question));
//        SelectedQuestion = Test.Questions.LastOrDefault();
//    }


//    [RelayCommand]
//    private void RemoveQuestion()
//    {
//        if (Test.Questions.Count <= 1 || SelectedQuestion == null)
//        {
//            return;
//        }

//        var removeQuestion = ServiceProvider.QuestionService.Remove(q => q.Id == SelectedQuestion.Model.Id);
//        if (!removeQuestion.IsSuccess)
//        {
//            MessageBox.Show("При видалені питання виникла помилка");
//            Trace.WriteLine(removeQuestion.ErrorMessage);
//            return;
//        }

//        Test.Questions.Remove(SelectedQuestion);
//        SelectedQuestion = Test.Questions.LastOrDefault();
//    }

//    [RelayCommand]
//    private void Next()
//    {
//        if (Test.Questions.Count == 0 || SelectedQuestion == null)
//        {
//            SelectedQuestion = Test.Questions.First();
//            return;
//        }

//        SelectedQuestion = Test.Questions[Test.Questions.IndexOf(SelectedQuestion) + 1];
//    }

//    [RelayCommand]
//    private void Previous()
//    {
//        if (Test.Questions.Count == 0 || SelectedQuestion == null)
//        {
//            SelectedQuestion = Test.Questions.First();
//            return;
//        }

//        SelectedQuestion = Test.Questions[Test.Questions.IndexOf(SelectedQuestion) - 1];
//    }

//    [RelayCommand]
//    private void AddAnswerOption()
//    {
//        if (Test.Questions.Count == 0 || SelectedQuestion == null)
//        {
//            return;
//        }

//        var answerOption = CreateAnswerOption();
//        SelectedQuestion.AnswerOptions.Add(new ObservableAnswerOption(answerOption, null));
//        SelectedAnswerOption = SelectedQuestion.AnswerOptions.LastOrDefault();
//    }

//    [RelayCommand]
//    private void RemoveAnswerOption()
//    {
//        if (SelectedQuestion == null || SelectedQuestion.AnswerOptions.Count <= 2 || SelectedAnswerOption == null)
//        {
//            return;
//        }

//        var removeAnswerOption = ServiceProvider.AnswerOptionService.Remove(o => o.Id == SelectedAnswerOption.Model.Id);
//        if (!removeAnswerOption.IsSuccess)
//        {
//            MessageBox.Show("При видалені варіанту відповіді виникла помилка");
//            Trace.WriteLine(removeAnswerOption.ErrorMessage);
//            return;
//        }

//        SelectedQuestion.AnswerOptions.Remove(SelectedAnswerOption);
//        SelectedAnswerOption = SelectedQuestion.AnswerOptions.LastOrDefault();
//    }

//    [RelayCommand]
//    private void SaveTest()
//    {
//        Save();
//        MessageBox.Show("Тест було збережено");
//    }

//    [RelayCommand]
//    private void Back()
//    {
//        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;

//        var answer = MessageBox.Show("При виході з редактору внесені зміни можуть бути втрачені!\nЗберегти зміни?", "Збереження даних", MessageBoxButton.YesNoCancel);

//        switch (answer)
//        {
//            case MessageBoxResult.Cancel:
//                return;
//            case MessageBoxResult.Yes:
//                Save();
//                break;
//            default:
//                break;
//        }
        
//        context.CurrentTest = null;
//        context.CurrentState = AppState.TeacherTestBrowser;
//    }

//    private Question CreateQuestion()
//    {
//        var question = new Question()
//        {
//            Content = "",
//            Type = QuestionType.SingleChoice,
//            GradeValue = 0,
//            Test = Test.Model
//        };

//        var createQuestion = ServiceProvider.QuestionService.Add(question);
//        if (!createQuestion.IsSuccess)
//        {
//            MessageBox.Show("При створені нового питання виникла помилка");
//            Trace.WriteLine(createQuestion.ErrorMessage);
//            return null!;
//        }

//        return question;
//    }

//    private AnswerOption CreateAnswerOption()
//    {
//        var answerOption = new AnswerOption()
//        {
//            Content = "",
//            Question = SelectedQuestion!.Model
//        };

//        var createanswerOption = ServiceProvider.AnswerOptionService.Add(answerOption);
//        if (!createanswerOption.IsSuccess)
//        {
//            MessageBox.Show("При створені нового варіанту відповіді виникла помилка");
//            Trace.WriteLine(createanswerOption.ErrorMessage);
//            return null!;
//        }

//        return answerOption;
//    }

//    private void Save()
//    {

//        Test.SaveModel();
//        foreach (var question in Test.Questions)
//        {
//            question.SaveModel();

//            foreach (var answerOption in question.AnswerOptions)
//            {
//                answerOption.IsTrue = answerOption.IsChecked;
//                answerOption.SaveModel();
//            }
//        }
//    }
//}
