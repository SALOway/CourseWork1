using BLL;
using BLL.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Enums;
using Core.Models;
using System.Collections.ObjectModel;
using System.Windows;
using UI.Enums;

namespace UI.ObservableModels;

public partial class ObservableQuestion : ObservableObject
{
    [ObservableProperty]
    private Guid _questionId;

    [ObservableProperty]
    private Guid _testId;

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
        TestId = question.Test.Id;
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

    public void Save(IQuestionService questionService)
    {
        var getQuestion = questionService.GetById(QuestionId);
        if (!getQuestion.IsSuccess)
        {
            MessageBox.Show("Виникла критична помилка\n" + getQuestion.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var question = getQuestion.Value;

        question.Content = Content;
        question.GradeValue = GradeValue;
        question.Type = QuestionType;
        question.UpdatedAt = DateTime.UtcNow;

        var update = questionService.Update(question);
        if (!update.IsSuccess)
        {
            MessageBox.Show("Виникла критична помилка\n" + update.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
    }
}
