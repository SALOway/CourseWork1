using CommunityToolkit.Mvvm.ComponentModel;
using Core.Enums;
using Core.Models;
using System.Collections.ObjectModel;
using System.Windows;
using UI.Enums;
using UI.ViewModels;

namespace UI.ObservableModels;

public partial class ObservableQuestion : ObservableObject
{
    [ObservableProperty]
    private Question _model;

    [ObservableProperty]
    private ObservableCollection<ObservableAnswerOption> _answerOptions = [];

    [ObservableProperty]
    private QuestionState _state = QuestionState.None;

    public ObservableQuestion(Question question)
    {
        Model = question;

        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;

        var userAnswers = question.UserAnswers.Where(a => a.User.Id == context.CurrentUser!.Id);
        if (userAnswers.Any())
        {
            if (userAnswers.Any(a => a.IsSelected))
            {
                State = QuestionState.Answered;
            }
            
            AnswerOptions = new ObservableCollection<ObservableAnswerOption>(userAnswers.Select(a => new ObservableAnswerOption(a.AnswerOption, a.IsSelected)));
        }
        else
        {
            AnswerOptions = new ObservableCollection<ObservableAnswerOption>(question.AnswerOptions.Select(o => new ObservableAnswerOption(o)));
        }
    }
}
