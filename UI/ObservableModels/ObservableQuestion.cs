using CommunityToolkit.Mvvm.ComponentModel;
using Core.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
            // Probaly will create user answers in the DB
            var getOptions = ServiceProvider.AnswerOptionService.GetAllOptionsForQuestion(question);
            if (!getOptions.IsSuccess)
            {
                MessageBox.Show("Не вдалося завантажити варіанти відповіді");
                Trace.WriteLine(getOptions.ErrorMessage);
                return;
            }

            var answerOptions = getOptions.Value.ToList();
            AnswerOptions = new ObservableCollection<ObservableAnswerOption>(answerOptions.Select(o => new ObservableAnswerOption(o)));
        }
    }
}
