using System.Windows.Controls;
using UI.ViewModels;

namespace UI.Views;

public partial class StudentTestBrowserView : UserControl
{
    public StudentTestBrowserView()
    {
        DataContext = new StudentTestBrowserViewModel(SessionContextProvider.SessionContext, ServiceProvider.UserService, ServiceProvider.TestService, ServiceProvider.QuestionService, ServiceProvider.AnswerOptionService, ServiceProvider.UserAnswerService, ServiceProvider.TestAttemptService);
        InitializeComponent();
    }
}
