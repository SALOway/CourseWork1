using System.Windows;
using System.Windows.Controls;
using UI.Enums;
using UI.Windows;

namespace UI.Controls;

public partial class TestDetails : UserControl
{
    private readonly MainWindow _mainWindow;

    public TestDetails(MainWindow mainWindow)
    {
        _mainWindow = mainWindow;
        InitializeComponent();

        if (AppState.CurrentTest == null)
        {
            throw new NullReferenceException(nameof(AppState.CurrentTest));
        }

        var currentUserAttempts = AppState.CurrentTest.TestAttempts.Where(a => a.User.Id == AppState.CurrentTest.Id);

        if (!currentUserAttempts.Any())
        {
            StartButton.Visibility = Visibility.Visible;
            return;
        }

        var lastAttemptStatus = currentUserAttempts.OrderByDescending(a => a.StartedAt).First();

        if (lastAttemptStatus.Status == Core.Enums.TestAttemptStatus.InProcess)
        {
            ContinueButton.Visibility = Visibility.Visible;
            return;
        }

        var maxAttempts = AppState.CurrentTest.MaxAttempts;
        var attemptsCount = currentUserAttempts.Count();

        if (attemptsCount < maxAttempts)
        {
            StartButton.Visibility = Visibility.Visible;
            return;
        }
    }

    private void Back_Click(object sender, RoutedEventArgs e)
    {
        _mainWindow.GoTo(Menus.StudentTestBrowser);
    }

    private void Start_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("А дзуськи тобі");
    }

    private void Continue_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("А дзуськи тобі");
    }
}
