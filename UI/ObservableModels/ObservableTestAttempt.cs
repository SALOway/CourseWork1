using BLL.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Enums;
using Core.Models;
using System.Windows;

namespace UI.ObservableModels;

public partial class ObservableTestAttempt : ObservableObject
{
    [ObservableProperty]
    private Guid _testAttemptId;

    [ObservableProperty]
    private ObservableTest? _test;

    [ObservableProperty]
    private DateTime _startedAt;

    public bool WasEnded => Status == TestAttemptStatus.Completed;

    [ObservableProperty]
    private DateTime? _endedAt;

    [ObservableProperty]
    private bool _hasLeftoverTime;

    [ObservableProperty]
    private DateTime? _leftoverTime;

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
        TestAttemptId = testAttempt.Id;
        StartedAt = testAttempt.StartedAt.ToLocalTime();
        EndedAt = testAttempt.EndedAt.HasValue ? testAttempt.EndedAt.Value.ToLocalTime() : testAttempt.EndedAt;
        HasLeftoverTime = testAttempt.HasLeftoverTime;
        LeftoverTime = testAttempt.LeftoverTime.HasValue ? testAttempt.LeftoverTime.Value : testAttempt.LeftoverTime;
        Status = testAttempt.Status;
        HasGrade = testAttempt.HasGrade;
        Grade = testAttempt.Grade;
        AmountOfAnsweredQuestions = testAttempt.Answers.Where(a => a.IsSelected).Select(a => a.Question.Id).ToHashSet().Count;
    }

    public void Save(ITestAttemptService testAttemptService, IQuestionService questionService)
    {
        var get = testAttemptService.GetById(TestAttemptId);
        if (!get.IsSuccess)
        {
            MessageBox.Show("Виникла критична помилка\n" + get.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var testAttempt = get.Value;

        testAttempt.Status = Status;
        testAttempt.EndedAt = EndedAt;
        testAttempt.HasGrade = HasGrade;

        

        if (testAttempt.HasGrade)
        {
            var getQuestions = questionService.Get(q => q.Test.Id == testAttempt.Test.Id);
            if (!getQuestions.IsSuccess)
            {
                MessageBox.Show("Виникла критична помилка\n" + getQuestions.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var questions = getQuestions.Value.ToList();

            int grade = 0;
            foreach (var question in questions)
            {
                var userAnswers = question.UserAnswers.Where(a => a.TestAttempt.Id == testAttempt.Id)
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
            testAttempt.Grade = Grade;
        }

        testAttempt.HasLeftoverTime = HasLeftoverTime;

        if (testAttempt.HasLeftoverTime)
        {
            testAttempt.LeftoverTime = LeftoverTime;
        }

        testAttempt.UpdatedAt = DateTime.UtcNow;

        var update = testAttemptService.Update(testAttempt);
        if (!update.IsSuccess)
        {
            MessageBox.Show("Виникла критична помилка\n" + update.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
    }
}
