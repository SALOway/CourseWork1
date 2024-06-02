using CommunityToolkit.Mvvm.ComponentModel;
using Core.Models;
using System.ComponentModel;
using UI.Enums;

namespace UI.Interfaces;

public interface ISessionContext : INotifyPropertyChanged
{
    Guid? CurrentUserId { get; set; }
    Guid? CurrentTestId { get; set; }
    Guid? CurrentTestAttemptId { get; set; }
    AppState CurrentState { get; set; }
}
