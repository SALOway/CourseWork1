using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using UI.Enums;
using UI.ObservableModels;

namespace UI.ViewModels;

public partial class TestAttemptViewModel : ObservableObject
{
    private readonly DispatcherTimer? _timer;

    [ObservableProperty]
    private ObservableCollection<ObservableQuestion> _questions = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsSelectedQuestionNotFirst))]
    [NotifyPropertyChangedFor(nameof(IsSelectedQuestionNotLast))]
    [NotifyPropertyChangedFor(nameof(SelectedQuestionNumber))]
    private ObservableQuestion? _selectedQuestion;

    [ObservableProperty]
    private bool _hasTimer;
    [ObservableProperty]
    private TimeSpan? _timeout;

    public TestAttemptViewModel()
    {
        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;
        var getQuestions = ServiceProvider.QuestionService.Get(q => context.CurrentTest!.Id == q.Test.Id);
        if (!getQuestions.IsSuccess)
        {
            MessageBox.Show("Не вдалося завантажити питання");
            Trace.WriteLine(getQuestions.ErrorMessage);
            return;
        }
        var questions = getQuestions.Value.Select(q => new ObservableQuestion(q));
        Questions = new ObservableCollection<ObservableQuestion>(questions);
        SelectedQuestion = Questions.FirstOrDefault();
        if (context.CurrentTest!.HasTimer)
        {
            #region DEBUG SHIT TO BE REMOVED
            context.CurrentTestAttempt!.StartedAt = DateTime.UtcNow;
            if (!ServiceProvider.TestAttemptService.Update(context.CurrentTestAttempt!).IsSuccess) throw new Exception();
            #endregion
            HasTimer = true;
            Timeout = context.CurrentTest!.TimeLimit;
            _timer = new DispatcherTimer();
            _timer.Tick += Timer_Tick;
            _timer.Interval = TimeSpan.FromMilliseconds(100);
            _timer.Start();
        }
    }

    public int SelectedQuestionNumber => SelectedQuestion != null ? Questions.IndexOf(SelectedQuestion) + 1 : 0;
    public bool IsSelectedQuestionFirst => SelectedQuestion != null && Questions.IndexOf(SelectedQuestion) == 0;
    public bool IsSelectedQuestionLast => SelectedQuestion != null && Questions.IndexOf(SelectedQuestion) + 1 == Questions.Count;
    public bool IsSelectedQuestionNotFirst => !IsSelectedQuestionFirst;
    public bool IsSelectedQuestionNotLast => !IsSelectedQuestionLast;


    [RelayCommand]
    private void Back()
    {
        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;

        _timer!.Stop();
        context.CurrentTestAttempt = null;
        context.CurrentState = AppState.TestDetails;
    }

    [RelayCommand]
    private void Next()
    {
        if (SelectedQuestion == null)
        {
            SelectedQuestion = Questions.First();
            return;
        }

        SelectedQuestion = Questions[Questions.IndexOf(SelectedQuestion) + 1];
    }

    [RelayCommand]
    private void Previous()
    {
        if (SelectedQuestion == null)
        {
            SelectedQuestion = Questions.First();
            return;
        }

        SelectedQuestion = Questions[Questions.IndexOf(SelectedQuestion) - 1];
    }

    [RelayCommand]
    private void Finish()
    {
        MessageBoxResult answer;
        if (Questions.Any(q => q.State == QuestionState.None) || !SelectedQuestion.AnswerOptions.Any(o => o.IsChecked))
        {
            answer = MessageBox.Show("Ви впевнені що бажаєте завершити тест? На деякі питання не було надано відповіді", "Завершити тест?", MessageBoxButton.YesNo);
            if (answer == MessageBoxResult.No)
            {
                return;
            }
        }

        answer = MessageBox.Show("Ви впевнені що бажаєте завершити тест?", "Завершити тест?", MessageBoxButton.YesNo);
        if (answer == MessageBoxResult.No)
        {
            return;
        }

        MessageBox.Show("Я не помру в туалеті!!!");
    }

    partial void OnSelectedQuestionChanged(ObservableQuestion? oldValue, ObservableQuestion? newValue)
    {
        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;
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
        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;

        Timeout = context.CurrentTest!.TimeLimit - (DateTime.UtcNow - context.CurrentTestAttempt!.StartedAt);
        if (Timeout <= TimeSpan.Zero)
        {
            Timeout = TimeSpan.Zero;
            _timer!.Stop();
        }
    }
}
