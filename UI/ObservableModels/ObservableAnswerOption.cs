using BLL;
using BLL.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Enums;
using Core.Models;
using System.Windows;

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

    public void Save(IAnswerOptionService answerOptionService)
    {
        var get = answerOptionService.GetById(AnswerOptionId);
        if (!get.IsSuccess)
        {
            MessageBox.Show("Виникла критична помилка\n" + get.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var answerOption = get.Value;

        answerOption.Content = Content;
        answerOption.IsTrue = IsTrue;

        var update = answerOptionService.Update(answerOption);
        if (!update.IsSuccess)
        {
            MessageBox.Show("Виникла критична помилка\n" + update.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
    }
}
