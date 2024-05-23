using Core.Enums;
using Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace UI.ViewModel;

public class AnswerOptionViewModel(AnswerOption answerOption, bool isChecked = false) : INotifyPropertyChanged
{
    private AnswerOption _answerOption = answerOption;
    private bool _isChecked = false;

    public string Content => _answerOption.Content;
    public QuestionType Type => _answerOption.Question.Type;

    public bool IsChecked
    {
        get => _isChecked;
        set
        {
            _isChecked = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

