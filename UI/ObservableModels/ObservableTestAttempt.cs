using CommunityToolkit.Mvvm.ComponentModel;
using Core.Models;

namespace UI.ObservableModels;

public partial class ObservableTestAttempt : ObservableObject
{
    [ObservableProperty]
    private TestAttempt _model;

    [ObservableProperty]
    private int _amountOfAnsweredQuestions;

    [ObservableProperty]
    private TimeSpan? _leftoverTime;

    public ObservableTestAttempt(TestAttempt testAttempt)
    {
        _model = testAttempt;
        AmountOfAnsweredQuestions = Model.Answers.Select(a => a.AnswerOption.Question.Id).ToHashSet().Count;
        LeftoverTime = Model.Test.TimeLimit - (Model.EndedAt - Model.StartedAt);
    }
}
