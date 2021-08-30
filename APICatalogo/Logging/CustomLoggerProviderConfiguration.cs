using Microsoft.Extensions.Logging;

namespace APICatalogo.Controllers.Logging
{
    public class CustomLoggerProviderConfiguration
    {
        public LogLevel LogLevel { get; set; }
        public int EventId { get; set; } = 0;
    }
}
