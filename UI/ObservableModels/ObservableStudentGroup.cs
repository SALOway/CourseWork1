using CommunityToolkit.Mvvm.ComponentModel;
using Core.Models;

namespace UI.ObservableModels;

public partial class ObservableStudentGroup : ObservableObject
{
    [ObservableProperty]
    private Guid _studentGroupId;

    [ObservableProperty]
    private string _name;

    public ObservableStudentGroup(StudentGroup studentGroup)
    {
        _studentGroupId = studentGroup.Id;
        _name = studentGroup.Name;
    }
}
