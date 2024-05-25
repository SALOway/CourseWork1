using CommunityToolkit.Mvvm.ComponentModel;
using Core.Models;

namespace UI.ObservableModels;

public partial class ObservableAnswerOption : ObservableObject
{
    [ObservableProperty]
    private AnswerOption _model;
    [ObservableProperty]
    private bool _isChecked;

    public ObservableAnswerOption(AnswerOption answerOption, bool isChecked = false) // isChecked should be set during creation
    {
        Model = answerOption;
        IsChecked = isChecked;
    }
}
