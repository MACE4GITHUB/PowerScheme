using System.Collections.Generic;
using System.Windows.Forms;
using Languages;

namespace MessageForm;

internal class DialogResultLookup
{
    internal static readonly Dictionary<DialogResult, string> DialogResultScheme
        = new()
        {
            {DialogResult.None, Language.Current.None},
            {DialogResult.OK, Language.Current.Ok},
            {DialogResult.Cancel, Language.Current.Cancel},
            {DialogResult.Abort, Language.Current.Abort},
            {DialogResult.Retry, Language.Current.Retry},
            {DialogResult.Ignore, Language.Current.Ignore},
            {DialogResult.Yes, Language.Current.Yes},
            {DialogResult.No, Language.Current.No}
        };
}