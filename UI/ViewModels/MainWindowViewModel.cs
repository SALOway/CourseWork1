using CommunityToolkit.Mvvm.ComponentModel;
using UI.Enums;

namespace UI.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private AppState _currentState;
    
    public MainWindowViewModel()
    {
        SessionContextProvider.SessionContext.PropertyChanged += (s, e) => {
            CurrentState = SessionContextProvider.SessionContext.CurrentState;
        };
    }
}
