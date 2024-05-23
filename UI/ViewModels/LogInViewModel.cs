using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using UI.Enums;

namespace UI.ViewModels;

public partial class LogInViewModel : ObservableValidator
{
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required]
    [StringLength(64, MinimumLength = 2)]
    private string _username = string.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required]
    [StringLength(64, MinimumLength = 2)]
    private string _password = string.Empty;

    [RelayCommand]
    private void LogIn()
    {
        ValidateAllProperties();
        if (HasErrors)
        {
            return;
        }

        var userService = ServiceProvider.UserService;
        var authResult = userService.Authenticate(Username, Password);
        if (authResult.IsSuccess)
        {
            var user = authResult.Value;
            var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;
            context.CurrentUser = user;
            switch (user.Role)
            {
                case UserRole.Teacher:
                    MessageBox.Show("Вчителя авторизовано");
                    break;
                case UserRole.Student:
                    context.CurrentState = AppState.TestBrowser;
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
        }
    }
}
