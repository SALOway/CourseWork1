using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using UI.ObservableModels;

namespace UI.ViewModels;

public partial class TestEditorViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<ObservableQuestion> _questions;
    [ObservableProperty]
    private ObservableQuestion _selectedQuestion;

    [RelayCommand]
    private void RemoveQuestion()
    {
        throw new NotImplementedException();
    }

    [RelayCommand]
    private void AddQuestion()
    {
        throw new NotImplementedException();
    }
}
