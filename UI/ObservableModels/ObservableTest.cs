using CommunityToolkit.Mvvm.ComponentModel;
using Core.Enums;
using Core.Models;
using System.Collections.ObjectModel;
using UI.Interfaces;

namespace UI.ObservableModels;

public partial class ObservableTest : ObservableObject
{
    [ObservableProperty]
    private Guid _testId;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private bool _hasDescription;

    [ObservableProperty]
    private string? _description;

    [ObservableProperty]
    private TestStatus _status;

    [ObservableProperty]
    private bool _hasAttempts;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AttemptsRatio))]
    private int? _maxAttempts;

    [ObservableProperty]
    private bool _hasRequiredGrade;

    [ObservableProperty]
    private int? _requiredGrade;

    [ObservableProperty]
    private bool _hasTermin;

    [ObservableProperty]
    private DateTime? _termin;

    [ObservableProperty]
    private bool _hasTimeLimit;

    [ObservableProperty]
    private DateTime? _timeLimit;

    [ObservableProperty]
    private DateTime _updatedAt;

    [ObservableProperty]
    private ObservableStudentGroup _studentGroup;

    [ObservableProperty]
    private ObservableCollection<ObservableQuestion> _questions = [];

    [ObservableProperty]
    private TestAttemptStatus? _lastAttemptStatus;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AttemptsRatio))]
    private int _attemptsCount;

    public int MaxPossibleGrade => Questions.Sum(q => q.GradeValue);

    public ObservableTest(Test test)
    {
        TestId = test.Id;
        Name = test.Name;
        HasDescription = test.HasDesription;
        Description = test.Description;
        Status = test.Status;
        MaxAttempts = test.MaxAttempts;
        HasRequiredGrade = test.HasRequiredGrade;
        HasTermin = test.HasTermin;
        Termin = test.Termin.HasValue ? test.Termin.Value.ToLocalTime() : test.Termin;
        HasTimeLimit = test.HasTimeLimit;
        TimeLimit = test.TimeLimit.HasValue ? test.TimeLimit.Value.ToLocalTime() : test.TimeLimit;
        UpdatedAt = test.UpdatedAt.ToLocalTime();
        StudentGroup = new ObservableStudentGroup(test.StudentGroup);
        //if (context.CurrentUser!.Role == UserRole.Student && test.TestAttempts.Any())
        //{
        //    LastAttemptStatus = test.TestAttempts
        //                            .Where(a => a.User.Id == context.CurrentUser!.Id)
        //                            .OrderByDescending(a => a.StartedAt)
        //                            .Select(a => (TestAttemptStatus?)a.Status)
        //                            .First();
        //    AttemptsCount = test.TestAttempts.Count(a => a.User.Id == context.CurrentUser!.Id);
        //}
    }

    public string AttemptsRatio => $"{AttemptsCount} з {MaxAttempts}";
}
