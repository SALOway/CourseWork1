using Core.Enums;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using UI.Commands;
using UI.Windows;

namespace UI.ViewModel;

public class StudentTestBrowserViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    private ObservableCollection<TestViewModel> _tests = [];
    private ObservableCollection<TestViewModel> _filteredTests = [];
    private string _searchText = string.Empty;
    private TestViewModel? _selectedTest;

    public int MyProperty { get; set; } = 5;

    public StudentTestBrowserViewModel()
    {
        LoadTests();
        FilteredTests = new ObservableCollection<TestViewModel>(Tests);
        SearchCommand = new RelayCommand(Search);
    }

    public ObservableCollection<TestViewModel> Tests
    {
        get => _tests;
        set { _tests = value; OnPropertyChanged(nameof(Tests)); }
    }

    public ObservableCollection<TestViewModel> FilteredTests
    {
        get => _filteredTests;
        set { _filteredTests = value; OnPropertyChanged(nameof(FilteredTests)); }
    }

    public string SearchText
    {
        get => _searchText;
        set { _searchText = value; OnPropertyChanged(nameof(SearchText)); }
    }

    public TestViewModel? SelectedTest
    {
        get => _selectedTest;
        set { _selectedTest = value; OnPropertyChanged(nameof(SelectedTest)); }
    }

    public ICommand SearchCommand { get; }

    private void LoadTests()
    {
        if (AppState.CurrentUser == null)
        {
            MessageBox.Show("Current user is null");
            throw new NullReferenceException(nameof(AppState.CurrentUser));
        }
        else if (AppState.CurrentUser.StudentGroup == null)
        {
            MessageBox.Show("User group is null");
            throw new NullReferenceException();
        }

        var getTestResult = ServiceProvider.TestService
            .Get(t => t.StudentGroup.Id == AppState.CurrentUser.StudentGroup.Id && t.Status == TestStatus.Public);
        if (!getTestResult.IsSuccess)
        {
            MessageBox.Show(getTestResult.ErrorMessage);
            return;
        }

        var tests = getTestResult.Value;

        Tests = new ObservableCollection<TestViewModel>(tests.Select(t => new TestViewModel(t)));
    }

    public void Search()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            FilteredTests = new ObservableCollection<TestViewModel>(Tests);
        }
        else
        {
            FilteredTests = new ObservableCollection<TestViewModel>(Tests.Where(t => t.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
        }
    }

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
