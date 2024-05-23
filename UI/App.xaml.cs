using System.Windows;

namespace UI;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        ServiceProvider.StupidFunction();
        base.OnStartup(e);
    }
}
