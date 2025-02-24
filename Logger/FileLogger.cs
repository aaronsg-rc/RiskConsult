using Microsoft.Extensions.Logging;

namespace RiskConsult.Logger;

public class FileLogger : BaseLogger, ILogger
{
	private readonly string _logPath;

	protected override string CategoryName { get; }

	protected override LogLevel LogLevel { get; }

	private object? _lock;

	public FileLogger( string categoryName, string filePath, LogLevel logLevel )
	{
		CategoryName = categoryName;
		LogLevel = logLevel;
		_logPath = filePath;
	}

	public override void LogLevelAction( LogLevel logLevel, string message )
	{
		lock ( _lock! )
		{
			if ( logLevel != LogLevel.None )
			{
				File.AppendAllText( _logPath, message + Environment.NewLine );
			}
		}
	}
}
