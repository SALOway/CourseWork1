using Core.Enums;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using UI.Enums;
using UI.Windows;


namespace UI.Controls;

public partial class LogInControl : UserControl
{
    private readonly Brush _borderBrush = new SolidColorBrush(Color.FromRgb(171, 173, 179));

    public LogInControl()
    {
        InitializeComponent();
    }

    private void LogIn_Click(object sender, RoutedEventArgs e)
    {
        var username = UsernameField.Text.Trim();
        var password = PasswordField.Password.Trim();
    
        var inputInvalid = string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password);
        if (inputInvalid)
        {
            MessageBox.Show("Будь-ласка, введіть корректні дані");
            return;
        }
    
        var authResult = MainWindow.Instance.UserService.Authenticate(username, password);
        if (authResult.IsSuccess)
        {
            var user = authResult.Value;
            MainWindow.CurrentUser = user;
            switch (user.Role)
            {
                case UserRole.Teacher:
                    MainWindow.Instance.GoTo(Menus.TeacherMenu);
                    break;
                case UserRole.Student:
                    MainWindow.Instance.GoTo(Menus.StudentTestBrowser);
                    break;
                case UserRole.None:
                default:
                    MessageBox.Show("Даного юзера неможливо авторизувати");
                    break;
            }
        }
        else
        {
            MessageBox.Show(authResult.ErrorMessage);
            ResetInputField(UsernameField, UsernameErrorMessage);
            ResetInputField(PasswordField, PasswordErrorMessage);
        }
    }

    private void UsernameField_LostFocus(object sender, RoutedEventArgs e)
    {
        var textbox = (TextBox)sender;

        var input = textbox.Text.Trim();
        Validate(input, textbox, UsernameErrorMessage);
    }
    private void PasswordField_LostFocus(object sender, RoutedEventArgs e)
    {
        var textbox = (PasswordBox)sender;

        var input = textbox.Password.Trim();
        Validate(input, textbox, PasswordErrorMessage);
    }

    private void Validate(string input, Control textBox, Label errorMessage)
    {
        if (string.IsNullOrEmpty(input))
        {
            errorMessage.Visibility = Visibility.Visible;
            textBox.BorderBrush = Brushes.Red;
        }
        else
        {
            textBox.BorderBrush = _borderBrush;
        }
    }

    private void UsernameField_GotFocus(object sender, RoutedEventArgs e)
    {
        var textbox = (TextBox)sender;

        ResetInputField(textbox, UsernameErrorMessage);
    }

    private void PasswordField_GotFocus(object sender, RoutedEventArgs e)
    {
        var passwordBox = (PasswordBox)sender;

        ResetInputField(passwordBox, PasswordErrorMessage);
    }

    private static void ResetInputField(Control textBox, Label errorMessage)
    {
        errorMessage.Visibility = Visibility.Hidden;
        textBox.BorderBrush = SystemColors.ActiveBorderBrush;
    }
}
