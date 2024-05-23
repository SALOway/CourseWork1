using System.Windows.Controls;
using System.Windows;
using UI.ViewModel;

namespace UI.DataTemplates;

public class AnswerOptionTemplateSelector : DataTemplateSelector
{
    public DataTemplate RadioButtonTemplate { get; set; }
    public DataTemplate CheckBoxTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (item is AnswerOptionViewModel answerOptionViewModel)
        {
            return answerOptionViewModel.Type == Core.Enums.QuestionType.SingleChoice ? RadioButtonTemplate : CheckBoxTemplate;
        }
        return base.SelectTemplate(item, container);
    }
}