using CommunityToolkit.Mvvm.ComponentModel;
using UI.Enums;
using UI.Interfaces;

namespace UI.ObservableModels;

public partial class SessionContext : ObservableObject, ISessionContext
{
    public const AppState DefaultState = AppState.LogIn;

    [ObservableProperty]
    private Guid? _currentUserId;

    [ObservableProperty]
    private Guid? _currentTestId;

    [ObservableProperty]
    private Guid? _currentTestAttemptId;

    [ObservableProperty]
    private AppState _currentState = DefaultState;
}
