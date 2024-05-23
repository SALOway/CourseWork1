using Core.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace UI.ViewModels;

public class QuestionViewModel : INotifyPropertyChanged
{
    //private readonly Question _question;
    //private ObservableCollection<AnswerOptionViewModel> _answerOptions = [];
    //public QuestionViewModel(Question question) 
    //{ 
    //    _question = question;
    //    var userAnswers = _question.UserAnswers.Where(a => AppState.CurrentUser.Id == a.TestAttempt.User.Id && a.TestAttempt.Id == AppState.CurrentTestAttempt.Id);
    //    if (userAnswers.Any())
    //    {
    //        AnswerOptions = new ObservableCollection<AnswerOptionViewModel>(userAnswers.Select(a => new AnswerOptionViewModel(a.AnswerOption, a.IsSelected)));
    //    }
    //    else
    //    {
    //        AnswerOptions = new ObservableCollection<AnswerOptionViewModel>(_question.AnswerOptions.Select(ao => new AnswerOptionViewModel(ao)));
    //    }
    //}

    //public Question Question
    //{
    //    get => _question;
    //}

    //public ObservableCollection<AnswerOptionViewModel> AnswerOptions
    //{
    //    get => _answerOptions;
    //    set { _answerOptions = value; OnPropertyChanged(nameof(AnswerOptions)); }
    //}

    //protected void OnPropertyChanged(string propertyName)
    //{
    //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    //}

    public event PropertyChangedEventHandler? PropertyChanged;
}
