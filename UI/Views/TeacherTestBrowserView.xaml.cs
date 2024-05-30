using System.Windows.Controls;
using UI.ViewModels;

namespace UI.Views;

public partial class TeacherTestBrowserView : UserControl
{
    public TeacherTestBrowserView()
    {
        DataContext = new TeacherTestBrowserViewModel(SessionContextProvider.SessionContext, ServiceProvider.UserService, ServiceProvider.TestService, ServiceProvider.QuestionService, ServiceProvider.AnswerOptionService, ServiceProvider.UserAnswerService, ServiceProvider.TestAttemptService, ServiceProvider.StudentGroupService);
        InitializeComponent();
    }
}
