using System.Windows.Controls;
using UI.ViewModels;

namespace UI.Views;

public partial class TestDetailsView : UserControl
{
    public TestDetailsView()
    {
        DataContext = new TestDetailsViewModel(SessionContextProvider.SessionContext, ServiceProvider.UserService, ServiceProvider.TestService, ServiceProvider.QuestionService, ServiceProvider.AnswerOptionService, ServiceProvider.UserAnswerService, ServiceProvider.TestAttemptService);
        InitializeComponent();
    }
}
