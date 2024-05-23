using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;

namespace UI.ViewModels;

public partial class TestAttemptViewModel : ObservableObject
{
    //private ObservableCollection<QuestionViewModel> _questions;
    //private QuestionViewModel? _currentQuestion;

    //public TestAttemptViewModel()
    //{
    //    var questions = AppState.CurrentTestAttempt.Test.Questions.Select(q => new QuestionViewModel(q));
    //    _questions = new ObservableCollection<QuestionViewModel>(questions);
    //    _currentQuestion = _questions.FirstOrDefault();
    //}

    //public ObservableCollection<QuestionViewModel> Questions
    //{
    //    get => _questions;
    //    set { _questions = value; OnPropertyChanged(nameof(Questions)); }
    //}

    //public QuestionViewModel? CurrentQuestion
    //{
    //    get => _currentQuestion;
    //    set { _currentQuestion = value; OnPropertyChanged(nameof(CurrentQuestion)); }
    //}

    //public int CurrentQuestionNumber => _questions.IndexOf(_currentQuestion) + 1;
}
