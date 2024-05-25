using System.Windows.Controls;
using System.Windows;
using UI.ObservableModels;
using Core.Enums;

namespace UI.Selectors;

public class AnswerOptionTemplateSelector : DataTemplateSelector
{
    public DataTemplate RadioButtonTemplate { get; set; }
    public DataTemplate CheckBoxTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (item is ObservableAnswerOption answerOption)
        {
            return answerOption.Model.Question.Type == QuestionType.SingleChoice ? RadioButtonTemplate : CheckBoxTemplate;
        }
        return base.SelectTemplate(item, container);
    }
}