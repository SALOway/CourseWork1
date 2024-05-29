using System.Windows.Controls;
using System.Windows;
using UI.ObservableModels;
using Core.Enums;

namespace UI.Selectors;

public class AnswerOptionTemplateSelector : DataTemplateSelector
{
    public DataTemplate SingleOptionQuestionOptions { get; set; }
    public DataTemplate MultipleOptionQuestionOptions { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        if (item is ObservableQuestion question)
        {
            return question.QuestionType == QuestionType.SingleChoice ? SingleOptionQuestionOptions : MultipleOptionQuestionOptions;
        }
        else if (item is QuestionType questionType)
        {
            return questionType == QuestionType.SingleChoice ? SingleOptionQuestionOptions : MultipleOptionQuestionOptions;
        }
        return base.SelectTemplate(item, container);
    }
}