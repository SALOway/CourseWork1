using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using UI.Enums;
using UI.ObservableModels;

namespace UI.ViewModels;

public partial class StudentTestBrowserViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<ObservableTest> _tests = [];

    [ObservableProperty]
    private ObservableCollection<ObservableTest> _filteredTests = [];

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private ObservableTest? _selectedTest;

    public StudentTestBrowserViewModel()
    {
        LoadTests();
    }

    [RelayCommand]
    private void Search()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            FilteredTests = new ObservableCollection<ObservableTest>(Tests);
        }
        else
        {
            FilteredTests = new ObservableCollection<ObservableTest>(Tests.Where(t => t.Model.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
        }
    }

    [RelayCommand]
    private static void LogOut()
    {
        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;

        context.CurrentUser = null;
        context.CurrentState = AppState.LogIn;
    }

    [RelayCommand]
    private void OpenDetails()
    {
        if (SelectedTest == null)
        {
            return;
        }

        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;

        context.CurrentTest = SelectedTest.Model;
        context.CurrentState = AppState.TestDetails;
    }

    partial void OnSelectedTestChanged(ObservableTest? value)
    {
        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;

        context.CurrentTest = value?.Model;
    }

    private void LoadTests()
    {
        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;
        var user = context.CurrentUser;

        var getTestsResult = ServiceProvider.TestService.GetTestsByGroup(user!.StudentGroup!);
        if (!getTestsResult.IsSuccess)
        {
            MessageBox.Show(getTestsResult.ErrorMessage);
            return;
        }

        var tests = getTestsResult.Value;

        Tests = new ObservableCollection<ObservableTest>(tests.Select(t => new ObservableTest(t)));
        FilteredTests = new ObservableCollection<ObservableTest>(Tests);
    }
}
