using BLL;
using BLL.Interfaces;
using Core.Models;
using DAL.Data;
using DAL.Repositories;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UI.Controls;
using UI.Enums;

namespace UI.Windows;

public partial class MainWindow : Window
{
    private readonly IAnswerOptionService _answerOptionService;
    private readonly IQuestionService _questionService;
    private readonly IStudentGroupService _studentGroupService;
    private readonly ITestAttemptService _testAttemptService;
    private readonly ITestService _testService;
    private readonly IUserAnswerService _userAnswerService;
    private readonly IUserService _userService;

    public MainWindow()
    {
        InitializeComponent();

        var context = new CourseWorkAppContext();
        context.Database.EnsureCreated();

        var answerOptionRepository = new AnswerOptionRepository(context);
        var answerOptionService = new AnswerOptionService(answerOptionRepository);
        _answerOptionService = answerOptionService;

        var questionRepository = new QuestionRepository(context);
        var questionService = new QuestionService(questionRepository);
        _questionService = questionService;

        var studentGroupRepository = new StudentGroupRepository(context);
        var studentGroupService = new StudentGroupService(studentGroupRepository);
        _studentGroupService = studentGroupService;

        var testAttemptRepository = new TestAttemptRepository(context);
        var testAttemptService = new TestAttemptService(testAttemptRepository);
        _testAttemptService = testAttemptService;

        var testRepository = new TestRepository(context);
        var testService = new TestService(testRepository);
        _testService = testService;

        var userAnswerRepository = new GenericRepository<UserAnswer>(context);
        var userAnswerService = new UserAnswerService(userAnswerRepository);
        _userAnswerService = userAnswerService;

        var userRepository = new UserRepository(context);
        var userService = new UserService(userRepository);
        _userService = userService;
    }

    public User? CurrentUser { get; set; }

    public void GoTo(Menus menu)
    {
        ContentControl.Content = menu switch
        {
            Menus.LogIn => new LogInControl(this, _userService),
            Menus.StudentManager => new StudentsManagerMenu(),
            Menus.TestBrowser => new TestBrowser(),
            Menus.TeacherMenu => new TeacherMenu(),
            _ => throw new NotImplementedException(),
        };
    }

    private void Window_Loaded(object sender, RoutedEventArgs e) => GoTo(Menus.LogIn);
}