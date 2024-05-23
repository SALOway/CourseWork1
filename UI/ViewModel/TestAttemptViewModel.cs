using System.Collections.ObjectModel;
using System.ComponentModel;

namespace UI.ViewModel;

public class TestAttemptViewModel : INotifyPropertyChanged
{
    private ObservableCollection<QuestionViewModel> _questions;
    private QuestionViewModel? _currentQuestion;

    public TestAttemptViewModel()
    {
        var questions = AppState.CurrentTestAttempt.Test.Questions.Select(q => new QuestionViewModel(q));
        _questions = new ObservableCollection<QuestionViewModel>(questions);
        _currentQuestion = _questions.FirstOrDefault();
    }

    public ObservableCollection<QuestionViewModel> Questions
    {
        get => _questions;
        set { _questions = value; OnPropertyChanged(nameof(Questions)); }
    }

    public QuestionViewModel? CurrentQuestion
    {
        get => _currentQuestion;
        set { _currentQuestion = value; OnPropertyChanged(nameof(CurrentQuestion)); }
    }

    public int CurrentQuestionNumber => _questions.IndexOf(_currentQuestion) + 1;

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
