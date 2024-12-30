using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RiskConsult.Logger;

namespace RiskConsult.Extensions;

public static class LoggingExtensions
{
	public static ILoggingBuilder AddConsoleLogger( this ILoggingBuilder builder, IConfiguration configuration )
	{
		return builder.AddProvider( new ConsoleLoggerProvider( configuration ) );
	}

	public static ILoggingBuilder AddDualLogger( this ILoggingBuilder builder, IConfiguration configuration )
	{
		return builder.AddProvider( new DualLoggerProvider( configuration ) );
	}

	public static ILoggingBuilder AddFileLogger( this ILoggingBuilder builder, IConfiguration configuration )
	{
		return builder.AddProvider( new FileLoggerProvider( configuration ) );
	}
}
