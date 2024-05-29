using BLL.Interfaces;
using BLL;
using Core.Models;
using DAL.Data;
using DAL.Repositories;

namespace UI;

public class ServiceProvider
{
    public static readonly IAnswerOptionService AnswerOptionService;
    public static readonly IQuestionService QuestionService;
    public static readonly IStudentGroupService StudentGroupService;
    public static readonly ITestAttemptService TestAttemptService;
    public static readonly ITestService TestService;
    public static readonly IUserAnswerService UserAnswerService;
    public static readonly IUserService UserService;

    // provokes freeze at the begining since doesn't created until accessed first time
    static ServiceProvider()
    {
        var context = new CourseWorkAppContext();
        context.Database.EnsureCreated();

        AnswerOptionService = new AnswerOptionService(new AnswerOptionRepository(context));
        QuestionService = new QuestionService(new QuestionRepository(context));
        StudentGroupService = new StudentGroupService(new StudentGroupRepository(context));
        TestAttemptService = new TestAttemptService(new TestAttemptRepository(context));
        TestService = new TestService(new TestRepository(context));
        UserAnswerService = new UserAnswerService(new GenericRepository<UserAnswer>(context));
        UserService = new UserService(new UserRepository(context));
    }

    private ServiceProvider() { }

    public static void Init()
    {
        // AHAHAHAHAHAHAHAH
    }
}
