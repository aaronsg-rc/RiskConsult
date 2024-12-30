using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RiskConsult.Extensions;

namespace RiskConsult.Logger;

public class DualLoggerProvider : ILoggerProvider
{
	private readonly LogLevel _logConsoleLevel;
	private readonly LogLevel _logDualLevel;
	private readonly LogLevel _logFileLevel;
	private readonly string _logPath;

	public DualLoggerProvider( IConfiguration configuration )
	{
		_logDualLevel = Enum.TryParse( configuration[ "DualLogger:LogLevel" ], out LogLevel level ) ? level : LogLevel.Trace;
		_logFileLevel = Enum.TryParse( configuration[ "FileLogger:LogLevel" ], out level ) ? level : _logDualLevel;
		_logConsoleLevel = Enum.TryParse( configuration[ "ConsoleLogger:LogLevel" ], out level ) ? level : _logDualLevel;

		var path = configuration[ "FileLogger:Path" ] ?? configuration[ "DualLogger:Path" ] ?? throw new Exception( "Missing configuration key: DualLogger:Path " );
		_logPath = path.DateFormat( DateTime.Today, "<>" );
	}

	public ILogger CreateLogger( string categoryName )
	{
		var logConsole = new ConsoleLogger( categoryName, _logConsoleLevel );
		var logFile = new FileLogger( categoryName, _logPath, _logFileLevel );
		return new DualLogger( categoryName, logConsole, logFile );
	}

	public void Dispose() => GC.SuppressFinalize( this );
}
