using CommunityToolkit.Mvvm.ComponentModel;
using Core.Enums;
using Core.Models;

namespace UI.ObservableModels;

public partial class ObservableAnswerOption : ObservableObject
{
    [ObservableProperty]
    private AnswerOption _model;

    [ObservableProperty]
    private string _content;

    [ObservableProperty]
    private bool _isTrue;

    [ObservableProperty] 
    private UserAnswer? _userAnswer;

    [ObservableProperty]
    private bool _isChecked;

    [ObservableProperty]
    private QuestionType _questionType;

    public ObservableAnswerOption(AnswerOption answerOption, UserAnswer? userAnswer)
    {
        Model = answerOption;
        Content = answerOption.Content;
        IsTrue = answerOption.IsTrue;
        QuestionType = answerOption.Question.Type;
        UserAnswer = userAnswer;
        IsChecked = userAnswer?.IsSelected ?? false;
    }

    public void SaveModel()
    {
        Model.Content = Content;
        Model.IsTrue = IsTrue;
        Model.UpdatedAt = DateTime.UtcNow;
        ServiceProvider.AnswerOptionService.Update(Model);
        if (UserAnswer != null)
        {
            UserAnswer.IsSelected = IsChecked;
            UserAnswer.UpdatedAt = DateTime.UtcNow;
            ServiceProvider.UserAnswerService.Update(UserAnswer);
        }
    }


}
