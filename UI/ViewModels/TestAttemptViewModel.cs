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

//public partial class TestAttemptViewModel : ObservableObject
//{
//    private readonly DispatcherTimer? _timer;

//    [ObservableProperty]
//    private ObservableTestAttempt _testAttempt;

//    [ObservableProperty]
//    [NotifyPropertyChangedFor(nameof(IsSelectedQuestionNotFirst))]
//    [NotifyPropertyChangedFor(nameof(IsSelectedQuestionNotLast))]
//    [NotifyPropertyChangedFor(nameof(SelectedQuestionNumber))]
//    private ObservableQuestion? _selectedQuestion;

//    [ObservableProperty]
//    private bool _hasTimer;
//    [ObservableProperty]
//    private TimeSpan? _timeout;

//    public TestAttemptViewModel()
//    {
//        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;

//        var testAttempt = context.CurrentTestAttempt!;
//        var user = context.CurrentUser!;
//        TestAttempt = new ObservableTestAttempt(testAttempt);

//        var getTests = ServiceProvider.TestService.Get(t => t.Id == testAttempt.Test.Id);
//        if (!getTests.IsSuccess)
//        {
//            MessageBox.Show("Виникла помилка при спробі отримати тест");
//            Trace.WriteLine(getTests.ErrorMessage);
//            return;
//        }

//        var test = getTests.Value.First();

//        TestAttempt.Test = new ObservableTest(test);

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
//                var getUserAnswers = ServiceProvider.UserAnswerService.Get(a => a.User.Id == user.Id && a.AnswerOption.Id == answerOption.Id);
//                if (!getUserAnswers.IsSuccess)
//                {
//                    MessageBox.Show(getUserAnswers.ErrorMessage);
//                    return;
//                }

//                var userAnswer = getUserAnswers.Value.FirstOrDefault();

//                ObservableAnswerOption observableAnswerOption;
//                if (userAnswer == null)
//                {
//                    var newUserAnswer = new UserAnswer()
//                    {
//                        TestAttempt = TestAttempt.Model,
//                        AnswerOption = answerOption,
//                        User = user,
//                        Question = question,
//                    };

//                    var add = ServiceProvider.UserAnswerService.Add(newUserAnswer);
//                    if (!add.IsSuccess)
//                    {
//                        MessageBox.Show("Не вдалося додати відповідь користувача");
//                        Trace.WriteLine(add.ErrorMessage);
//                    }

//                    observableAnswerOption = new ObservableAnswerOption(answerOption, newUserAnswer);
//                }
//                else
//                {
//                    observableAnswerOption = new ObservableAnswerOption(answerOption, userAnswer);
//                }

//                observableQuestion.AnswerOptions.Add(observableAnswerOption);
//            }

//            TestAttempt.Test.Questions.Add(observableQuestion);
//        }

//        SelectedQuestion = TestAttempt.Test.Questions.FirstOrDefault();

//        if (TestAttempt.Test.HasTimer)
//        {
//            HasTimer = true;
//            Timeout = TestAttempt.Test.TimeLimit;
//            _timer = new DispatcherTimer();
//            _timer.Tick += Timer_Tick;
//            _timer.Interval = TimeSpan.FromMilliseconds(100);
//            _timer.Start();
//        }
//    }

//    public int SelectedQuestionNumber => SelectedQuestion != null ? TestAttempt.Test.Questions.IndexOf(SelectedQuestion) + 1 : 0;
//    public bool IsSelectedQuestionFirst => SelectedQuestion != null && TestAttempt.Test.Questions.IndexOf(SelectedQuestion) == 0;
//    public bool IsSelectedQuestionLast => SelectedQuestion != null && TestAttempt.Test.Questions.IndexOf(SelectedQuestion) + 1 == TestAttempt.Test.Questions.Count;
//    public bool IsSelectedQuestionNotFirst => !IsSelectedQuestionFirst;
//    public bool IsSelectedQuestionNotLast => !IsSelectedQuestionLast;


//    [RelayCommand]
//    private void Back()
//    {
//        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;

//        _timer?.Stop();
//        Save();
//        context.CurrentTestAttempt = null;
//        context.CurrentState = AppState.TestDetails;
//    }

//    [RelayCommand]
//    private void Next()
//    {
//        if (SelectedQuestion == null)
//        {
//            SelectedQuestion = TestAttempt.Test.Questions.First();
//            return;
//        }

//        SelectedQuestion = TestAttempt.Test.Questions[TestAttempt.Test.Questions.IndexOf(SelectedQuestion) + 1];
//    }

//    [RelayCommand]
//    private void Previous()
//    {
//        if (SelectedQuestion == null)
//        {
//            SelectedQuestion = TestAttempt.Test.Questions.First();
//            return;
//        }

//        SelectedQuestion = TestAttempt.Test.Questions[TestAttempt.Test.Questions.IndexOf(SelectedQuestion) - 1];
//    }

//    [RelayCommand]
//    private void Finish()
//    {
//        MessageBoxResult answer;
//        if (TestAttempt.Test.Questions.Any(q => q.State == QuestionState.None) || !SelectedQuestion!.AnswerOptions.Any(o => o.IsChecked))
//        {
//            answer = MessageBox.Show("Ви впевнені що бажаєте завершити тест? На деякі питання не було надано відповіді", "Завершити тест?", MessageBoxButton.YesNo);
//            if (answer == MessageBoxResult.No)
//            {
//                return;
//            }
//        }

//        answer = MessageBox.Show("Ви впевнені що бажаєте завершити тест?", "Завершити тест?", MessageBoxButton.YesNo);
//        if (answer == MessageBoxResult.No)
//        {
//            return;
//        }

//        _timer?.Stop();
//        TestAttempt.EndedAt = DateTime.UtcNow;
//        TestAttempt.Status = TestAttemptStatus.Completed;
//        TestAttempt.HasGrade = true;
//        Save();

//        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;
//        context.CurrentTestAttempt = null;
//        context.CurrentState = AppState.TestDetails;
//    }

//    partial void OnSelectedQuestionChanged(ObservableQuestion? oldValue, ObservableQuestion? newValue)
//    {
//        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;
//        if (oldValue != null)
//        {
//            if (oldValue.AnswerOptions.Any(o => o.IsChecked))
//            {
//                oldValue.State = QuestionState.Answered;
//            }
//            else
//            {
//                oldValue.State = QuestionState.None;
//            }
//        }

//        if (newValue != null)
//        {
//            newValue.State = QuestionState.Selected;
//        }
//    }

//    private void Timer_Tick(object? sender, EventArgs e)
//    {
//        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;

//        Timeout = TestAttempt.Test.TimeLimit - (DateTime.UtcNow - context.CurrentTestAttempt!.StartedAt);
//        if (Timeout <= TimeSpan.Zero)
//        {
//            Timeout = TimeSpan.Zero;
//            _timer!.Stop();
//        }
//    }

//    private void Save()
//    {
//        if (TestAttempt.Status == TestAttemptStatus.Completed && TestAttempt.Test.HasTimer && Timeout > TimeSpan.Zero)
//        {
//            TestAttempt.HasLeftoverTime = true;
//            TestAttempt.LeftoverTime = Timeout;
//        }
//        TestAttempt.SaveModel();
//        foreach (var question in TestAttempt.Test.Questions)
//        {
//            question.SaveModel();

//            foreach (var answerOption in question.AnswerOptions)
//            {
//                answerOption.SaveModel();
//            }
//        }
//    }
//}
