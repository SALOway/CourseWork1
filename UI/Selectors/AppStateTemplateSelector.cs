using System.Windows.Controls;
using System.Windows;
using UI.Enums;

namespace UI.Selectors;

public class AppStateTemplateSelector : DataTemplateSelector
{
    public DataTemplate? Placeholder { get; set; }
    public DataTemplate? LogInTemplate { get; set; }
    public DataTemplate? StudentTestBrowserTemplate { get; set; }
    public DataTemplate? TestDetailsTemplate { get; set; }
    public DataTemplate? TestAttemptsTemplate { get; set; }
    public DataTemplate? TeacherTestBrowserTemplate { get; set; }
    public DataTemplate? TestEditorTemplate { get; set; }

    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        if (item == null)
        {
            return Placeholder;
        }
        var appState = (AppState)item;
        return appState switch
        {
            AppState.LogIn => LogInTemplate,
            AppState.StudentTestBrowser => StudentTestBrowserTemplate,
            AppState.TestDetails => TestDetailsTemplate,
            AppState.TestAttempt => TestAttemptsTemplate,
            AppState.TeacherTestBrowser => TeacherTestBrowserTemplate,
            AppState.TestEditor => TestEditorTemplate,
            _ => Placeholder,
        };
    }
}
