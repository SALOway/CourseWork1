using CommunityToolkit.Mvvm.ComponentModel;
using Core.Enums;
using Core.Models;

namespace UI.ObservableModels;

public partial class ObservableTestAttempt : ObservableObject
{
    [ObservableProperty]
    private TestAttempt _model;

    [ObservableProperty]
    private ObservableTest _test;

    [ObservableProperty]
    private DateTime _startedAt;

    [ObservableProperty]
    private DateTime? _endedAt;

    [ObservableProperty]
    private bool _hasLeftoverTime;

    [ObservableProperty]
    private TimeSpan? _leftoverTime;

    [ObservableProperty]
    private TestAttemptStatus _status;

    [ObservableProperty]
    private bool _hasGrade;

    [ObservableProperty]
    private int? _grade;

    [ObservableProperty]
    private int _amountOfAnsweredQuestions;

    public ObservableTestAttempt(TestAttempt testAttempt)
    {
        Model = testAttempt;
        StartedAt = testAttempt.StartedAt;
        EndedAt = testAttempt.EndedAt;
        HasLeftoverTime = testAttempt.HasLeftoverTime;
        LeftoverTime = testAttempt.LeftoverTime;
        Status = testAttempt.Status;
        HasGrade = testAttempt.HasGrade;
        Grade = testAttempt.Grade;

        AmountOfAnsweredQuestions = testAttempt.Answers.Select(a => a.Question.Id).ToHashSet().Count;
    }

    public void SaveModel()
    {
        Model.Status = Status;
        Model.EndedAt = EndedAt;
        Model.HasGrade = HasGrade;
        if (Model.HasGrade)
        {
            var getQuestions = ServiceProvider.QuestionService.Get(q => q.Test.Id == Model.Test.Id);
            var questions = getQuestions.Value;
            var grade = 0;
            foreach (var question in questions)
            {
                var debug = question.UserAnswers.Where(a => a.TestAttempt.Id == Model.Id).ToList();
                var userAnswers = question.UserAnswers.Where(a => a.TestAttempt.Id == Model.Id)
                                                      .ToDictionary(a => a.AnswerOption.Id, a => a.IsSelected);

                bool answerIsRight = true;
                foreach (var answerOption in question.AnswerOptions)
                {
                    if (answerOption.IsTrue != userAnswers[answerOption.Id])
                    {
                        answerIsRight = false;
                        break;
                    }
                }

                if (answerIsRight)
                {
                    grade += question.GradeValue;
                }
            }
            Grade = grade;
            Model.Grade = Grade;
        }
        Model.HasLeftoverTime = HasLeftoverTime;
        if (Model.HasLeftoverTime)
        {
            Model.LeftoverTime = LeftoverTime;
        }
        Model.UpdatedAt = DateTime.UtcNow;
        ServiceProvider.TestAttemptService.Update(Model);
    }

    public bool WasEnded => Status == TestAttemptStatus.Completed;
    public DateTime StartedAtLocal => StartedAt.ToLocalTime();
    public DateTime? EndedAtLocal => EndedAt?.ToLocalTime();
}
