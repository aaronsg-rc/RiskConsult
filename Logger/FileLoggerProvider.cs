using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RiskConsult.Extensions;

namespace RiskConsult.Logger;

public class FileLoggerProvider : ILoggerProvider
{
	private readonly LogLevel _logLevel;
	private readonly string _logPath;

	public FileLoggerProvider( IConfiguration configuration )
	{
		_logLevel = Enum.TryParse( configuration[ "FileLogger:LogLevel" ], out LogLevel result ) ? result : LogLevel.Information;
		_logPath = configuration[ "FileLogger:Path" ]?.DateFormat( DateTime.Today, "<>" ) ?? throw new Exception( "Missing configuration key: FileLogger:Path" );
	}

	public FileLoggerProvider( string filePath, LogLevel level )
	{
		_logPath = filePath;
		_logLevel = level;
	}

	public ILogger CreateLogger( string categoryName ) => new FileLogger( categoryName, _logPath, _logLevel );

	public void Dispose() => GC.SuppressFinalize( this );
}
