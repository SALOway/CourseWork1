using System.Windows.Controls;
using UI.ViewModels;

namespace UI.Views;

public partial class TestEditorView : UserControl
{
    public TestEditorView()
    {
        DataContext = new TestEditorViewModel(SessionContextProvider.SessionContext, ServiceProvider.UserService, ServiceProvider.TestService, ServiceProvider.QuestionService, ServiceProvider.AnswerOptionService, ServiceProvider.UserAnswerService, ServiceProvider.TestAttemptService, ServiceProvider.StudentGroupService);
        InitializeComponent();
    }
}
