using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Core.Enums;
using Core.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using UI.Enums;
using UI.ObservableModels;

namespace UI.ViewModels;

public partial class TeacherTestBrowserViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<ObservableTest> _tests = [];

    [ObservableProperty]
    private ObservableCollection<ObservableTest> _filteredTests = [];

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private ObservableTest? _selectedTest;

    public TeacherTestBrowserViewModel()
    {
        LoadTests();
    }

    private void LoadTests()
    {
        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;
        var user = context.CurrentUser;

        var getTestsResult = ServiceProvider.TestService.Get();
        if (!getTestsResult.IsSuccess)
        {
            MessageBox.Show(getTestsResult.ErrorMessage);
            return;
        }

        var tests = getTestsResult.Value.ToList();

        Tests = new ObservableCollection<ObservableTest>(tests.Select(t => new ObservableTest(t)));
        FilteredTests = new ObservableCollection<ObservableTest>(Tests);
    }

    [RelayCommand]
    private static void LogOut()
    {
        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;

        context.CurrentUser = null;
        context.CurrentState = AppState.LogIn;
    }

    [RelayCommand]
    private void Back()
    {
        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;

        context.CurrentState = AppState.TeacherMenu;
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
    private void CreateNewTest()
    {
        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;

        context.CurrentTest = new Test()
        {
            Name = string.Empty,
            Creator = context.CurrentUser!,
            Status = TestStatus.Draft,
            MaxAttempts = 1,
        };

        context.CurrentState = AppState.TestEditor;
    }

    [RelayCommand]
    private void ModifyTest()
    {
        if (SelectedTest == null)
        {
            return;
        }

        var context = (MainWindowViewModel)Application.Current.MainWindow.DataContext;

        context.CurrentTest = SelectedTest.Model;
        context.CurrentState = AppState.TestEditor;
    }

    [RelayCommand]
    private void RemoveTest()
    {
        if (SelectedTest == null)
        {
            return;
        }

        var answer = MessageBox.Show("Ви впевнені що бажаєте видалити тест?", "Видалення тесу", MessageBoxButton.YesNo);
        if (answer != MessageBoxResult.Yes)
        {
            return;
        }

        var remove = ServiceProvider.TestService.Remove(t => t.Id == SelectedTest.Model.Id);
        if (!remove.IsSuccess)
        {
            MessageBox.Show("Щось пішло не так. Тест не видалено");
            Trace.WriteLine(remove.ErrorMessage);
            return;
        }

        Tests.Remove(SelectedTest);
        FilteredTests.Remove(SelectedTest);
        SelectedTest = FilteredTests.FirstOrDefault();
    }
}
