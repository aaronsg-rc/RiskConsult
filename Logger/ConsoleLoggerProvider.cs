using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace RiskConsult.Logger;

public class ConsoleLoggerProvider : ILoggerProvider
{
	private readonly LogLevel _logLevel;

	public ConsoleLoggerProvider( IConfiguration configuration )
	{
		_logLevel = Enum.TryParse( configuration[ "ConsoleLogger:LogLevel" ], out LogLevel result ) ? result : LogLevel.Information;
	}

	public ConsoleLoggerProvider( LogLevel logLevel ) => _logLevel = logLevel;

	public ILogger CreateLogger( string categoryName ) => new ConsoleLogger( categoryName, _logLevel );

	public void Dispose() => GC.SuppressFinalize( this );
}
