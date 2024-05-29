using CommunityToolkit.Mvvm.ComponentModel;
using Core.Enums;
using Core.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using UI.ViewModels;

namespace UI.ObservableModels;

public partial class ObservableTest : ObservableObject
{
    [ObservableProperty]
    private Test _model;

    [ObservableProperty]
    private string _name;

    [ObservableProperty]
    private bool _hasDescription;

    [ObservableProperty]
    private string? _description;

    [ObservableProperty]
    private TestStatus _status;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AttemptsRatio))]
    private int _maxAttempts;

    [ObservableProperty]
    private bool _hasRequiredGrade;

    [ObservableProperty]
    private int? _requiredGrade;

    [ObservableProperty]
    private bool _hasTermin;

    [ObservableProperty]
    private DateTime? _termin;

    [ObservableProperty]
    private bool _hasTimer;

    [ObservableProperty]
    private TimeSpan? _timeLimit;

    [ObservableProperty]
    private DateTime _updatedAt;

    [ObservableProperty]
    private StudentGroup _studentGroup;

    [ObservableProperty]
    private ObservableCollection<ObservableQuestion> _questions = [];

    [ObservableProperty]
    private TestAttemptStatus? _lastAttemptStatus;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AttemptsRatio))]
    private int _attemptsCount;


    public int MaxGrade => Questions.Sum(q => q.GradeValue);

    public ObservableTest(Test test)
    {
        Model = test;
        Name = test.Name;
        HasDescription = test.HasDesription;
        Description = test.Description;
        Status = test.Status;
        MaxAttempts = test.MaxAttempts;
        HasRequiredGrade = test.HasRequiredGrade;
        HasTermin = test.HasTermin;
        Termin = test.Termin;
        HasTimer = test.HasTimer;
        TimeLimit = test.TimeLimit;
        UpdatedAt = test.UpdatedAt;
        StudentGroup = test.StudentGroup;

        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;
        if (context.CurrentUser!.Role == UserRole.Student && test.TestAttempts.Any())
        {
            LastAttemptStatus = test.TestAttempts
                                    .Where(a => a.User.Id == context.CurrentUser!.Id)
                                    .OrderByDescending(a => a.StartedAt)
                                    .Select(a => (TestAttemptStatus?)a.Status)
                                    .First();
            AttemptsCount = test.TestAttempts.Count(a => a.User.Id == context.CurrentUser!.Id);
        }
    }

    public void SaveModel()
    {
        Model.Name = Name;
        Model.HasDesription = HasDescription;
        Model.Description = Description;
        Model.Status = Status;
        Model.MaxAttempts = MaxAttempts;
        Model.HasRequiredGrade = HasRequiredGrade;
        Model.RequiredGrade = RequiredGrade;
        Model.HasTermin = HasTermin;
        Model.Termin = Termin;
        Model.HasTimer = HasTimer;
        Model.TimeLimit = TimeLimit;
        Model.StudentGroup = StudentGroup;
        Model.UpdatedAt = DateTime.UtcNow;
        var save = ServiceProvider.TestService.Update(Model);
        if (!save.IsSuccess)
        {
            MessageBox.Show("Помилка при збережені тесту");
            Trace.WriteLine(save.ErrorMessage);
        }
    }

    public string AttemptsRatio => $"{AttemptsCount} з {MaxAttempts}";
    public DateTime? TerminLocal => Termin?.ToLocalTime();
}
