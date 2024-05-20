using BLL;
using BLL.Interfaces;
using Core.Models;
using DAL.Data;
using DAL.Repositories;
using System.Windows;
using System.Windows.Controls;
using UI.Controls;
using UI.Enums;

namespace UI.Windows;

public partial class MainWindow : Window
{
    public readonly IAnswerOptionService AnswerOptionService;
    public readonly IQuestionService QuestionService;
    public readonly IStudentGroupService StudentGroupService;
    public readonly ITestAttemptService TestAttemptService;
    public readonly ITestService TestService;
    public readonly IUserAnswerService UserAnswerService;
    public readonly IUserService UserService;

    public MainWindow()
    {
        Instance = this;

        InitializeComponent();

        var context = new CourseWorkAppContext();
        context.Database.EnsureCreated();

        var answerOptionRepository = new AnswerOptionRepository(context);
        var answerOptionService = new AnswerOptionService(answerOptionRepository);
        AnswerOptionService = answerOptionService;

        var questionRepository = new QuestionRepository(context);
        var questionService = new QuestionService(questionRepository);
        QuestionService = questionService;

        var studentGroupRepository = new StudentGroupRepository(context);
        var studentGroupService = new StudentGroupService(studentGroupRepository);
        StudentGroupService = studentGroupService;

        var testAttemptRepository = new TestAttemptRepository(context);
        var testAttemptService = new TestAttemptService(testAttemptRepository);
        TestAttemptService = testAttemptService;

        var testRepository = new TestRepository(context);
        var testService = new TestService(testRepository);
        TestService = testService;

        var userAnswerRepository = new GenericRepository<UserAnswer>(context);
        var userAnswerService = new UserAnswerService(userAnswerRepository);
        UserAnswerService = userAnswerService;

        var userRepository = new UserRepository(context);
        var userService = new UserService(userRepository);
        UserService = userService;
    }

    public static MainWindow Instance { get; private set; } = null!;
    public static User? CurrentUser { get; set; }
    public static Test? CurrentTest { get; set; }

    public void GoTo(Menus menu)
    {
        ContentControl.Content = menu switch
        {
            Menus.LogIn => new LogInControl(),
            Menus.StudentManager => new StudentsManagerMenu(),
            Menus.StudentTestBrowser => new StudentTestBrowser(),
            Menus.TeacherMenu => new TeacherMenu(),
            Menus.TestDetails => new TestDetails(),
            _ => throw new NotImplementedException(),
        };
    }

    public void LogOut()
    {
        CurrentUser = null;
        GoTo(Menus.LogIn);
    }

    private void Window_Loaded(object sender, RoutedEventArgs e) => GoTo(Menus.LogIn);
}