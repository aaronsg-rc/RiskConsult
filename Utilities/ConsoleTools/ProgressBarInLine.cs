namespace RiskConsult.Utilities.ConsoleTools;

public static class ProgressBarInLine
{
	public static int ProgressBarLenght { get; set; } = 50;

	public static void StartProgress( int total, string message = "element" )
	{
		WriteProgress( 0, total, message );
		ClearCurrentConsoleLine();
	}

	public static void UpdateProgress( Action action, int index, int total, string message = "element" )
	{
		ClearCurrentConsoleLine();

		try
		{
			action.Invoke();
		}
		catch ( Exception ex )
		{
			Console.WriteLine( $"ERROR: {ex.Message}" );
		}

		WriteProgress( index, total, message );
	}

	public static async Task UpdateProgressAsync( Func<Task> action, int index, int total, string message = "element" )
	{
		ClearCurrentConsoleLine();

		try
		{
			await action.Invoke();
		}
		catch ( Exception ex )
		{
			Console.WriteLine( $"ERROR: {ex.Message}" );
		}

		WriteProgress( index, total, message );
	}

	private static void ClearCurrentConsoleLine()
	{
		//Console.Write( new string( ' ', progress.Length ) + "\r" );
		var currentLineCursor = Console.CursorTop;
		Console.SetCursorPosition( 0, Console.CursorTop - 1 );
		Console.Write( new string( ' ', Console.WindowWidth ) );
		Console.SetCursorPosition( 0, currentLineCursor - 1 );
	}

	private static void WriteProgress( int index, int total, string message )
	{
		string progressChars = new( '|', Convert.ToInt32( ProgressBarLenght * index / total ) );
		string emptyChars = new( ' ', ProgressBarLenght - progressChars.Length );
		var progress = $"[{progressChars}{emptyChars}] {index + 1:N0} of {total:N0} {message}";
		Console.Write( progress );
	}
}
