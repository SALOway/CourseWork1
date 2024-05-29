using CommunityToolkit.Mvvm.ComponentModel;
using Core.Models;
using UI.Enums;

namespace UI.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    public const AppState DefaultState = AppState.LogIn; 

    [ObservableProperty]
    private User? _currentUser;

    [ObservableProperty]
    private Test? _currentTest;

    [ObservableProperty]
    private TestAttempt? _currentTestAttempt;

    [ObservableProperty]
    private AppState _currentState = AppState.LogIn;
}
