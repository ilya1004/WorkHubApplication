using Serilog.Events;
using Serilog.Formatting;

namespace ChatService.Infrastructure.Services.LogstashHelpers;

public class LogstashTextFormatter : ITextFormatter
{
    public void Format(LogEvent logEvent, TextWriter output)
    {
        output.Write($"[{logEvent.Timestamp:HH:mm:ss} {logEvent.Level} ChatService] ");
        logEvent.RenderMessage(output);
        output.WriteLine();
        if (logEvent.Exception != null)
        {
            output.WriteLine($"Exception: {logEvent.Exception}");
        }
    }
}