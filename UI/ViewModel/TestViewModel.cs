using Core.Models;
using UI.Windows;

namespace UI.ViewModel;

public class TestViewModel(Test test)
{
    public Test Test { get; set; } = test;

    public Guid Id => Test.Id;
    public string Name => Test.Name;
    public string Description => Test.Description ?? "Тут міг би бути опис...";
    public int MaxAttempts => Test.MaxAttempts;
    public bool HasTermin => Test.HasTermin;
    public DateTime? Termin => Test.Termin;
    public bool HasTimer => Test.HasTimer;
    public TimeSpan TimeLimit => Test.TimeLimit;
    public int StudentAttemptCount
    {
        get
        {
            if (AppState.CurrentUser == null)
            {
                throw new NullReferenceException(nameof(AppState.CurrentUser));
            }

            return Test.TestAttempts.Where(a => a.User.Id == AppState.CurrentUser.Id 
                                                && AppState.CurrentUser.Role == Core.Enums.UserRole.Student)
                                    .Count();
        }
    }
    public TestAttempt? LastAttempt
    {
        get
        {
            if (AppState.CurrentUser == null)
            {
                throw new NullReferenceException(nameof(AppState.CurrentUser));
            }

            return Test.TestAttempts.Where(a => a.User.Id == AppState.CurrentUser.Id
                                                && AppState.CurrentUser.Role == Core.Enums.UserRole.Student)
                                    .OrderByDescending(attempt => attempt.StartedAt)
                                    .FirstOrDefault();
        }
    }
}
