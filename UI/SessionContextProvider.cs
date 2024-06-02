using UI.Interfaces;
using UI.ObservableModels;

namespace UI;

public static class SessionContextProvider
{
    public static readonly ISessionContext SessionContext;

    static SessionContextProvider()
    {
        SessionContext = new SessionContext();
    }
}
