using System.ComponentModel;
using System.Windows.Forms;
using static System.Array;

namespace PowerScheme.Utility;

public static class Invoker
{
    public static void InvokeIfRequired(this ISynchronizeInvoke obj, MethodInvoker action)
    {
        if (obj.InvokeRequired)
        {
            var args = Empty<object>();
            try
            {
                obj.Invoke(action, args);
            }
            catch
            {
                // ignored
            }
        }
        else
        {
            action();
        }
    }
}
