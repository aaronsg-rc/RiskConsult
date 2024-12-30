using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

namespace RiskConsult.Logger;

public abstract class BaseLogger : ILogger
{
	protected abstract string CategoryName { get; }
	protected abstract LogLevel LogLevel { get; }

	public IDisposable? BeginScope<TState>( TState state ) where TState : notnull => null;

	public bool IsEnabled( LogLevel logLevel ) => logLevel >= LogLevel;

	public virtual void Log<TState>( LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter )
	{
		if ( !IsEnabled( logLevel ) || logLevel is LogLevel.None )
		{
			return;
		}

		var message = formatter( state, exception );

		// Regex para validar y extraer datos del formato [TITLE#N], donde # es cualquier carácter y N es un número
		var titleRegex = new Regex( @"\[TITLE(?<char>.)\s*(?<len>\d+)\]" );
		var match = titleRegex.Match( message );

		// Flags
		bool isTitle = match.Success;
		bool omitDate = isTitle || message.Contains( "[OMIT_DATE]" );
		bool isRaw = message.Contains( "[RAW]" ); // Nuevo flag para mostrar tal cual

		// Si el mensaje debe ser "tal cual" (RAW), limpiamos y lo devolvemos sin prefijo ni fecha
		if ( isRaw )
		{
			message = message.Replace( "[RAW]", string.Empty );
			LogLevelAction( logLevel, message );
			return;
		}

		// Genero bloque de fecha
		var blockDate = omitDate ?
			string.Empty :
			$"[{DateTime.Now}] - ";

		// Genero bloque de prefijo
		var blockPrefix = LogLevel is LogLevel.Trace or LogLevel.Debug && !isTitle && omitDate ?
			$"[{logLevel}|{CategoryName}] - " :
			string.Empty;

		// Genero bloque de mensaje
		var blockMessage = message;
		if ( omitDate )
		{
			blockMessage = blockMessage.Replace( "[OMIT_DATE]", string.Empty );
		}

		if ( isTitle )
		{
			char titleChar = match.Groups[ "char" ].Value[ 0 ];
			int titleLen = int.Parse( match.Groups[ "len" ].Value );

			var title = titleRegex.Replace( blockMessage, string.Empty );
			blockMessage = Utilities.Utilities.CreateFormattedTitle( title, titleChar, titleLen );
		}

		message = $"{blockDate}{blockPrefix}{blockMessage}";

		LogLevelAction( logLevel, message );
	}

	public abstract void LogLevelAction( LogLevel logLevel, string message );
}
