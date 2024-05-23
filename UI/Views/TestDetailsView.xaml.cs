using System.Windows.Controls;
using UI.ViewModels;

namespace UI.Views;

public partial class TestDetailsView : UserControl
{
    public TestDetailsView()
    {
        DataContext = new TestDetailsViewModel();
        InitializeComponent();
    }
    //private readonly MainWindow _mainWindow;
    //private readonly TestAttempt _testAttemptToContinue;

    //public TestDetails(MainWindow mainWindow)
    //{
    //    _mainWindow = mainWindow;
    //    InitializeComponent();

    //    if (AppState.CurrentTest == null)
    //    {
    //        throw new NullReferenceException(nameof(AppState.CurrentTest));
    //    }

    //    var currentUserAttempts = AppState.CurrentTest.TestAttempts.Where(a => a.User.Id == AppState.CurrentTest.Id);

    //    if (!currentUserAttempts.Any())
    //    {
    //        ContinueButton.Visibility = Visibility.Collapsed;
    //        StartButton.Visibility = Visibility.Visible;
    //        return;
    //    }

    //    var lastAttempt = currentUserAttempts.OrderByDescending(a => a.StartedAt).First();

    //    if (lastAttempt.Status == Core.Enums.TestAttemptStatus.InProcess)
    //    {
    //        StartButton.Visibility = Visibility.Collapsed;
    //        ContinueButton.Visibility = Visibility.Visible;
    //        _testAttemptToContinue = lastAttempt;
    //        return;
    //    }

    //    var maxAttempts = AppState.CurrentTest.MaxAttempts;
    //    var attemptsCount = currentUserAttempts.Count();

    //    if (attemptsCount < maxAttempts)
    //    {
    //        StartButton.Visibility = Visibility.Visible;
    //        return;
    //    }
    //}

    //private void Back_Click(object sender, RoutedEventArgs e)
    //{
    //    _mainWindow.GoTo(AppState.StudentTestBrowser);
    //}

    //private void Start_Click(object sender, RoutedEventArgs e)
    //{
    //    var newAttempt = new TestAttempt()
    //    {
    //        User = AppState.CurrentUser,
    //        Test = AppState.CurrentTest,
    //    };
    //    var createNewAttempt = ServiceProvider.TestAttemptService.Add(newAttempt);
    //    if (!createNewAttempt.IsSuccess)
    //    {
    //        MessageBox.Show(createNewAttempt.ErrorMessage);
    //    }
    //    AppState.CurrentTestAttempt = newAttempt;
    //    _mainWindow.GoTo(AppState.TestAttempt);
    //}

    //private void Continue_Click(object sender, RoutedEventArgs e)
    //{
    //    AppState.CurrentTestAttempt = _testAttemptToContinue;
    //    _mainWindow.GoTo(AppState.TestAttempt);
    //}
}
