using System.Windows.Controls;
using UI.ViewModels;

namespace UI.Views;

public partial class TestAttemptView : UserControl
{
    public TestAttemptView()
    {
        DataContext = new TestAttemptViewModel(SessionContextProvider.SessionContext, ServiceProvider.UserService, ServiceProvider.TestService, ServiceProvider.QuestionService, ServiceProvider.AnswerOptionService, ServiceProvider.UserAnswerService, ServiceProvider.TestAttemptService);
        InitializeComponent();
    }
}
