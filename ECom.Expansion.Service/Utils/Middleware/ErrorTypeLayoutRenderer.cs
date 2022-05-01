using NLog;
using NLog.LayoutRenderers;
using System.Text;

namespace ECom.Expansion.Service.Utils
{
    [LayoutRenderer("exception-type")]
    public class ErrorTypeLayoutRenderer : LayoutRenderer
    {
        public ErrorTypeLayoutRenderer()
        {
        }

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            if (logEvent.Level == NLog.LogLevel.Error)
            {
                var errorType = "Logic";
                var ex = logEvent.Exception;
                if (ex != null && ex.GetType().Assembly.GetName().Name != "ECom.Types")
                {
                    errorType = "Code";
                }
                builder.Append(errorType);
            }
        }
    }
}
