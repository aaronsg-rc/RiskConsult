using Microsoft.Extensions.Logging;

namespace RiskConsult.Logger;

public class ConsoleLogger : BaseLogger, ILogger
{
	protected override string CategoryName { get; }
	protected override LogLevel LogLevel { get; }

	public ConsoleLogger( string categoryName, LogLevel logLevel )
	{
		LogLevel = logLevel;
		CategoryName = categoryName;
	}

	public override void LogLevelAction( LogLevel logLevel, string message )
	{
		switch ( logLevel )
		{
			case LogLevel.Trace:
				LogMessage( message, ConsoleColor.Gray, ConsoleColor.Black );
				break;

			case LogLevel.Debug:
				LogMessage( message, ConsoleColor.DarkCyan, ConsoleColor.Black );
				break;

			case LogLevel.Information:
				LogMessage( message, ConsoleColor.White, ConsoleColor.Black );
				break;

			case LogLevel.Warning:
				LogMessage( message, ConsoleColor.DarkYellow, ConsoleColor.Black );
				break;

			case LogLevel.Error:
				LogMessage( message, ConsoleColor.DarkRed, ConsoleColor.Black );
				break;

			case LogLevel.Critical:
				LogMessage( message, ConsoleColor.White, ConsoleColor.DarkRed );
				break;

			case LogLevel.None:
			default:
				break;
		}
	}

	private static void LogMessage( string message, ConsoleColor foregroundColor, ConsoleColor backgroundColor )
	{
		ConsoleColor initialForegroundColor = Console.ForegroundColor;
		ConsoleColor initialBackgroundColor = Console.BackgroundColor;

		Console.ForegroundColor = foregroundColor;
		Console.BackgroundColor = backgroundColor;
		Console.WriteLine( message );

		Console.ForegroundColor = initialForegroundColor;
		Console.BackgroundColor = initialBackgroundColor;
	}
}
