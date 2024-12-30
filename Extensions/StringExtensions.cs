namespace RiskConsult.Extensions;

public static class StringExtensions
{
	public static string DateFormat( this string value, DateTime? date, string betweenChars )
	{
		if ( date is null )
		{
			return value;
		}

		var LChar = betweenChars.First();
		var RChar = betweenChars.Last();
		var result = value;
		do
		{
			var format = result.FirstBetween( LChar, RChar );
			if ( string.IsNullOrEmpty( format ) )
			{
				return result;
			}

			result = result.Replace( $"{LChar}{format}{RChar}", date?.ToString( format.Trim() ) ?? string.Empty );
		} while ( true );
	}

	public static string FirstBetween( this string value, char leftChar, char rightChar )
	{
		var startIndex = value.IndexOf( leftChar );
		if ( startIndex == -1 )
		{
			return string.Empty;
		}

		var finalIndex = value.IndexOf( rightChar, startIndex + 1 );
		if ( finalIndex == -1 )
		{
			return string.Empty;
		}

		var length = finalIndex - startIndex - 1;
		return length <= 0 ? string.Empty : value.Substring( startIndex + 1, length );
	}
}
