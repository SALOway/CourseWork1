using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UI.CustomControls;

public partial class IntegerInputBox : TextBox
{
    private static readonly Regex _regex = IntegerRegex();

    private static bool IsTextAllowed(string text)
    {
        return !_regex.IsMatch(text);
    }

    public IntegerInputBox()
    {
        DataObject.AddPastingHandler(this, TextBoxPasting);
    }

    protected override void OnPreviewTextInput(TextCompositionEventArgs e)
    {
        e.Handled = !IsTextAllowed(e.Text);
    }

    private void TextBoxPasting(object sender, DataObjectPastingEventArgs e)
    {
        if (e.DataObject.GetDataPresent(typeof(string)))
        {
            var text = (string)e.DataObject.GetData(typeof(string));
            if (!IsTextAllowed(text))
            {
                e.CancelCommand();
            }
        }
        else
        {
            e.CancelCommand();
        }
    }

    [GeneratedRegex("[^0-9-]+")]
    private static partial Regex IntegerRegex();
}
