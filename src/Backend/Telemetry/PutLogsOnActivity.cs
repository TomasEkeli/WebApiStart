using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Logs;

namespace Backend.Telemetry;

public class PutLogsOnActivity : BaseProcessor<LogRecord>
{
    public override void OnEnd(LogRecord data)
    {
        if (data is null)
        {
            return;
        }

        Activity.Current?.AddEvent(data.ToActivityEvent());
    }
}
