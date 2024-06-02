using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using UI.Enums;
using UI.Interfaces;

namespace UI.ViewModels;

public partial class TeacherMenuViewModel : ObservableObject
{
    private readonly ISessionContext _sessionContext;

    public TeacherMenuViewModel(ISessionContext sessionContext)
    {
        _sessionContext = sessionContext;
    }

    [RelayCommand]
    private void LogOut()
    {
        _sessionContext.CurrentUserId = null;
        _sessionContext.CurrentState = AppState.LogIn;
    }

    [RelayCommand]
    private void OpenTestBrowser()
    {
        _sessionContext.CurrentState = AppState.TeacherTestBrowser;
    }
}
