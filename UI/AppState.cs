using Core.Models;

namespace UI;

public class AppState
{
    public static User? CurrentUser { get; set; }
    public static Test? CurrentTest { get; set; }
    public static TestAttempt? CurrentTestAttempt { get; set; }
}
