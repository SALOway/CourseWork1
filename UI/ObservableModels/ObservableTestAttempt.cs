using CommunityToolkit.Mvvm.ComponentModel;
using Core.Enums;
using Core.Models;

namespace UI.ObservableModels;

public partial class ObservableTestAttempt : ObservableObject
{
    [ObservableProperty]
    private TestAttempt _model;

    [ObservableProperty]
    private int _amountOfAnsweredQuestions;

    public ObservableTestAttempt(TestAttempt testAttempt)
    {
        _model = testAttempt;
        AmountOfAnsweredQuestions = Model.Answers.Select(a => a.Question.Id).ToHashSet().Count;
    }

    public bool WasEnded => Model.Status == TestAttemptStatus.Completed;
    public DateTime StartedAtLocal => Model.StartedAt.ToLocalTime();
    public DateTime? EndedAtLocal => Model.EndedAt?.ToLocalTime();
}
