using System.ComponentModel;

namespace PowerScheme.Utility;

public static class Invoker
{
    public static void InvokeIfRequired(this ISynchronizeInvoke obj, System.Windows.Forms.MethodInvoker action)
    {
        if (obj.InvokeRequired)
        {
            var args = new object[0];
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