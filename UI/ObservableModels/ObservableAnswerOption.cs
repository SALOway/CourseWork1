using CommunityToolkit.Mvvm.ComponentModel;
using Core.Enums;
using Core.Models;

namespace UI.ObservableModels;

public partial class ObservableAnswerOption : ObservableObject
{
    [ObservableProperty]
    private Guid _answerOptionId;

    [ObservableProperty]
    private string _content;

    [ObservableProperty]
    private bool _isTrue;

    [ObservableProperty]
    private bool _isChecked;

    [ObservableProperty]
    private QuestionType _questionType;

    public ObservableAnswerOption(AnswerOption answerOption, bool isChecked = false)
    {
        AnswerOptionId = answerOption.Id;
        Content = answerOption.Content;
        IsTrue = answerOption.IsTrue;
        QuestionType = answerOption.Question.Type;
        IsChecked = isChecked;
    }
}
