using CommunityToolkit.Mvvm.ComponentModel;
using Core.Enums;
using Core.Models;
using System.Collections.ObjectModel;
using UI.Enums;

namespace UI.ObservableModels;

public partial class ObservableQuestion : ObservableObject
{
    [ObservableProperty]
    private Guid _questionId;

    [ObservableProperty]
    private string _content;

    [ObservableProperty]
    private QuestionType _questionType;

    [ObservableProperty]
    private int _gradeValue;

    [ObservableProperty]
    private ObservableCollection<ObservableAnswerOption> _answerOptions = [];

    [ObservableProperty]
    private QuestionState _state = QuestionState.None;

    public ObservableQuestion(Question question)
    {
        QuestionId = question.Id;
        Content = question.Content;
        QuestionType = question.Type;
        GradeValue = question.GradeValue;
    }

    partial void OnQuestionTypeChanged(QuestionType value)
    {
        if (value == QuestionType.SingleChoice)
        {
            bool singleAnswerOptionChecked = false;
            foreach (var answerOption in AnswerOptions)
            {
                if (!singleAnswerOptionChecked && answerOption.IsChecked)
                {
                    singleAnswerOptionChecked = true;
                    continue;
                }
                answerOption.IsChecked = false;
            }
        }
    }
}
