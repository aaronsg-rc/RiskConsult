using Microsoft.Extensions.Logging;

namespace RiskConsult.Logger;

public class DualLogger : BaseLogger, ILogger
{
	private readonly ConsoleLogger _consoleLogger;
	private readonly FileLogger _fileLogger;

	protected override string CategoryName { get; }

	protected override LogLevel LogLevel { get; }

	public DualLogger( string categoryName, ConsoleLogger consoleLogger, FileLogger fileLogger )
	{
		CategoryName = categoryName;
		LogLevel = LogLevel.Trace;
		_consoleLogger = consoleLogger;
		_fileLogger = fileLogger;
	}

	public override void LogLevelAction( LogLevel logLevel, string message )
	{
		_consoleLogger.Log( logLevel, "[RAW]{msg}", message );
		_fileLogger.Log( logLevel, "[RAW]{msg}", message );
	}
}
