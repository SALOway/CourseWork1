using BLL.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using UI.Enums;
using UI.Interfaces;

namespace UI.ViewModels;

public partial class LogInViewModel : ObservableValidator
{
    private readonly IUserService _userService;
    private readonly ISessionContext _sessionContext;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Введіть логін")]
    [StringLength(16, MinimumLength = 2, ErrorMessage = "Логін користувача має бути більше 2, та менше 16 символів")]
    private string? _username = string.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Введіть пароль")]
    [StringLength(16, MinimumLength = 2, ErrorMessage = "Пароль користувача має бути більше 2, та менше 16 символів")]
    private string? _password = string.Empty;

    public LogInViewModel(ISessionContext sessionContext, IUserService userService)
    {
        _userService = userService;
        _sessionContext = sessionContext;
    }

    [RelayCommand]
    private void LogIn()
    {
        ValidateAllProperties();
        if (HasErrors || Username == null || Password == null)
        {
            return;
        }

        var authResult = _userService.Authenticate(Username, Password);
        if (authResult.IsSuccess)
        {
            var user = authResult.Value;
            _sessionContext.CurrentUserId = user.Id;
            switch (user.Role)
            {
                case UserRole.Teacher:
                    _sessionContext.CurrentState = AppState.TeacherMenu;
                    break;
                case UserRole.Student:
                    _sessionContext.CurrentState = AppState.StudentTestBrowser;
                    break;
                case UserRole.None:
                default:
                    MessageBox.Show("Даного юзера неможливо авторизувати.", "Помилка авторизації", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
            }
        }
        else
        {
            MessageBox.Show("Виникла критична помилка.\n" + authResult.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    partial void OnPasswordChanging(string? value)
    {
        ClearErrors(nameof(Password));
    }

    partial void OnUsernameChanging(string? value)
    {
        ClearErrors(nameof(Username));
    }
}
