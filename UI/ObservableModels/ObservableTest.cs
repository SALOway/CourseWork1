using BLL.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using Core.Enums;
using Core.Models;
using System.Collections.ObjectModel;
using System.Windows;

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
        HasAttempts = test.HasAttempts;
        MaxAttempts = test.MaxAttempts;
        HasRequiredGrade = test.HasRequiredGrade;
        RequiredGrade = test.RequiredGrade;
        HasTermin = test.HasTermin;
        Termin = test.Termin.HasValue ? test.Termin.Value.ToLocalTime() : test.Termin;
        HasTimeLimit = test.HasTimeLimit;
        TimeLimit = test.TimeLimit.HasValue ? test.TimeLimit.Value.ToLocalTime() : test.TimeLimit;
        UpdatedAt = test.UpdatedAt.ToLocalTime();
        StudentGroup = new ObservableStudentGroup(test.StudentGroup);
    }

    public void Save(ITestService testService, IStudentGroupService studentGroupService)
    {
        var getTest = testService.GetById(TestId);
        if (!getTest.IsSuccess)
        {
            MessageBox.Show("Виникла критична помилка\n" + getTest.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var test = getTest.Value;
        
        var getGroup = studentGroupService.GetById(StudentGroup.StudentGroupId);
        if (!getGroup.IsSuccess)
        {
            MessageBox.Show("Виникла критична помилка\n" + getTest.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var group = getGroup.Value;

        test.Name = Name;
        test.HasDesription = HasDescription;
        test.Description = Description;
        test.Status = Status;
        test.MaxAttempts = MaxAttempts;
        test.HasRequiredGrade = HasRequiredGrade;
        test.RequiredGrade = RequiredGrade;
        test.HasTermin = HasTermin;
        test.Termin = Termin.HasValue ? Termin.Value.ToUniversalTime() : test.Termin;
        test.HasTimeLimit = HasTimeLimit;
        test.TimeLimit = TimeLimit.HasValue ? TimeLimit.Value.ToUniversalTime() : test.TimeLimit;
        test.StudentGroup = group;
        test.UpdatedAt = DateTime.UtcNow;

        var update = testService.Update(test);
        if (!update.IsSuccess)
        {
            MessageBox.Show("Виникла критична помилка\n" + update.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
    }

    public string AttemptsRatio => $"{AttemptsCount} з {MaxAttempts}";
}
