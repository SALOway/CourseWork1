using BLL.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Enums;
using Core.Models;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using UI.Enums;
using UI.Interfaces;

namespace UI.ViewModels;

public partial class LogInViewModel : ObservableValidator
{
    private readonly IUserService _userService;
    private readonly IStudentGroupService _studentGroupService;
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

    public LogInViewModel(ISessionContext sessionContext, IUserService userService, IStudentGroupService studentGroupService)
    {
        _userService = userService;
        _sessionContext = sessionContext;
        _studentGroupService = studentGroupService;
        BaseInit();
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
                    _sessionContext.CurrentState = AppState.TeacherTestBrowser;
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

    private void BaseInit()
    {
        var getGroup = _studentGroupService.Get();
        if (!getGroup.IsSuccess)
        {
            MessageBox.Show("Виникла критична помилка\n" + getGroup.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var group = getGroup.Value.FirstOrDefault();

        if (group == null)
        {
            group = new StudentGroup()
            {
                Name = "Група 1"
            };

            var addGroup = _studentGroupService.Add(group);
            if (!addGroup.IsSuccess)
            {
                MessageBox.Show("Виникла критична помилка\n" + addGroup.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        var get = _userService.Get();
        if (!get.IsSuccess)
        {
            MessageBox.Show("Виникла критична помилка\n" + get.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var users = get.Value;

        if (!users.Any(u => u.Role == UserRole.Teacher))
        {
            var teacher = new User()
            {
                Username = "tc1",
                Password = "ps1",
                FirstName = "",
                LastName = "",
                Role = UserRole.Teacher,
            };

            var add = _userService.Add(teacher);
            if (!add.IsSuccess)
            {
                MessageBox.Show("Виникла критична помилка\n" + add.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        if (!users.Any(u => u.Role == UserRole.Student))
        {
            var student = new User()
            {
                Username = "st1",
                Password = "ps1",
                FirstName = "",
                LastName = "",
                StudentGroup = group,
                Role = UserRole.Student,
            };

            var add = _userService.Add(student);
            if (!add.IsSuccess)
            {
                MessageBox.Show("Виникла критична помилка\n" + add.ErrorMessage, "Критична помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
    }
}
