using System.Windows.Controls;
using System.Windows;
using UI.Enums;

namespace UI.Selectors;

public class AppStateTemplateSelector : DataTemplateSelector
{
    public DataTemplate? Placeholder { get; set; }
    public DataTemplate? LogInTemplate { get; set; }
    public DataTemplate? TestBrowserTemplate { get; set; }
    public DataTemplate? TestDetailsTemplate { get; set; }
    // Define other templates here

    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        var appState = (AppState)item;
        return appState switch
        {
            AppState.LogIn => LogInTemplate,
            AppState.TestBrowser => TestBrowserTemplate,
            AppState.TestDetails => TestDetailsTemplate,
            _ => Placeholder,
        };
    }
}
