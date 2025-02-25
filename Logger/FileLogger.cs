using Microsoft.Extensions.Logging;

namespace RiskConsult.Logger;

public class FileLogger : BaseLogger, ILogger
{
	private readonly string _logPath;

	protected override string CategoryName { get; }

	protected override LogLevel LogLevel { get; }

	private readonly object _lock = new object();

	public FileLogger( string categoryName, string filePath, LogLevel logLevel )
	{
		_logPath = filePath ?? throw new ArgumentNullException( nameof( filePath ) );
		CategoryName = categoryName ?? throw new ArgumentNullException( nameof( categoryName ) );
		LogLevel = logLevel;
	}

	public override void LogLevelAction( LogLevel logLevel, string message )
	{
		if ( logLevel == LogLevel.None || string.IsNullOrWhiteSpace( message ) )
		{
			return;
		}

		lock ( _lock ) // Bloqueo para evitar concurrencia en escritura
		{
			File.AppendAllText( _logPath, message + Environment.NewLine );
		}
	}
}
