using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace woodgrovedemo.Helpers
{
    public class AppInsights
    {
        
        public static void TrackException(TelemetryClient telemetry, Exception exception, string functionName)
        {
            ExceptionTelemetry exceptionTelemetry = new ExceptionTelemetry(exception);
            exceptionTelemetry.Properties.Add("Type", "AppError");
            exceptionTelemetry.Properties.Add("Page", "Profile");
            exceptionTelemetry.Properties.Add("Function", functionName);
            exceptionTelemetry.Properties.Add("ExceptionType", exception.GetType().ToString());
            telemetry.TrackException(exceptionTelemetry);
        }

    }
}