using System.Text;
using System.Windows.Forms;

namespace RiskConsult.Utilities;

public static class Utilities
{
	public static string CreateFormattedTitle( string title, char charTitleLine, int lineSize )
	{
		var cleanTitle = title.Trim();
		var line = new string( charTitleLine, lineSize );

		if ( cleanTitle.Length >= lineSize )
		{
			return $"{line}{Environment.NewLine}{cleanTitle[ ..lineSize ]}{Environment.NewLine}{line}";
		}

		var separatorLength = ( lineSize - cleanTitle.Length - 2 ) / 2;
		var leftSeparator = new string( charTitleLine, separatorLength );
		var rightSeparator = new string( charTitleLine, lineSize - cleanTitle.Length - separatorLength - 2 );
		var fullTitle = $"{leftSeparator} {cleanTitle} {rightSeparator}";

		if ( fullTitle.Length < lineSize )
		{
			fullTitle += charTitleLine;
		}

		return $"{line}{Environment.NewLine}{fullTitle}{Environment.NewLine}{line}";
	}

	/// <summary> Se lee el contenido del portapapeles y lo transforma en un array de 2 dimensiones </summary>
	/// <param name="rowSeparator"> Carácter por el cual deben separarse las filas </param>
	/// <param name="colSeparator"> Carácter por el cual deben separarse las columnas </param>
	/// <returns> Array de 2 dimensiones contenido en el portapapeles, en error devuelve Nothing </returns>
	public static string[,]? GetArrayFromClipboard( string rowSeparator, string colSeparator, TextDataFormat dataFormat = TextDataFormat.Text )
	{
		string[,] arrRes;
		try
		{
			// Obtengo filas de texto
			var arrRows = GetLinesFromClipboard( rowSeparator, dataFormat );

			// Obtengo número de columnas del output
			var intCols = arrRows[ 0 ].Split( colSeparator.ToCharArray() ).Length;

			// Defino mi resultado
			arrRes = new string[ arrRows.Length, intCols ];

			// Recorro cada fila obtenida
			var intResRow = arrRes.GetLowerBound( 0 );
			int intResCol;
			for ( var intRow = arrRows.GetLowerBound( 0 ); intRow < arrRows.Length; intRow++ )
			{
				// Obtengo mi array de columna desde la fila
				var arrCol = arrRows[ intRow ].Split( colSeparator.ToCharArray() );
				intResCol = arrRes.GetLowerBound( 1 );

				// Agrego mi columna a mi resultado
				for ( var intCol = arrCol.GetLowerBound( 0 ); intCol < arrCol.Length; intCol++ )
				{
					arrRes[ intResRow, intResCol ] = arrCol[ intCol ];
					intResCol++;
				}

				intResRow++;
			}

			return arrRes;
		}
		catch ( Exception )
		{
			return null;
		}
	}

	/// <summary> Convierte el contenido de texto del portapapeles en un vector separado por <paramref name="lineSeparator" /> </summary>
	public static string[] GetLinesFromClipboard( string lineSeparator, TextDataFormat dataFormat = TextDataFormat.Text )
	{
		// Valido si el portapapeles tiene contenido
		var content = Clipboard.GetText( dataFormat );
		if ( string.IsNullOrEmpty( content ) )
		{
			return [];
		}

		// Paso contenido del portapapeles a un string
		return content.Split( lineSeparator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries );
	}

	public static double[,] GetMatrix<TRow, TCol>( IEnumerable<TRow> rows, IEnumerable<TCol> cols, Func<TRow, TCol, double> valueFunction )
	{
		IList<TCol> colList = cols as IList<TCol> ?? cols.ToArray();
		IList<TRow> rowList = rows as IList<TRow> ?? rows.ToArray();

		var matrix = new double[ rowList.Count, colList.Count ];
		for ( var i = 0; i < rowList.Count; i++ )
		{
			for ( var j = 0; j < colList.Count; j++ )
			{
				matrix[ i, j ] = valueFunction( rowList[ i ], colList[ j ] );
			}
		}

		return matrix;
	}

	/// <summary>
	/// Sustituyo los formatos de fechas por su valor de aquellas coincidencias entre los carácteres ingresados, por ejemplo "Prueba@yyyyMMdd@" --&gt; "Prueba20201205"
	/// </summary>
	/// <param name="value"> cadena que se va a formatear </param>
	/// <param name="date"> fecha con la que se va a aplicar formato </param>
	/// <param name="LChar"> Carácter izquierdo del formato </param>
	/// <param name="RChar"> Carácter izquderechoierdo del formato </param>
	/// <returns> String con formato de fecha donde sea indicado </returns>
	public static string GetStringDateFormated( string value, DateTime date, char LChar, char RChar )
	{
		var result = value;
		var LBetween = false;
		var format = string.Empty;
		foreach ( var c in value )
		{
			if ( LBetween == false && c == LChar )
			{
				LBetween = true;
			}
			else if ( LBetween && c == RChar && format != string.Empty )
			{
				result = result.Replace( $"{LChar}{format}{RChar}", date.ToString( format ) );
				format = string.Empty;
				LBetween = false;
			}
			else if ( LBetween )
			{
				format += c;
			}
		}

		return result;
	}

	public static string RemoveSpecialCharacters( this string word )
	{
		var sb = new StringBuilder();
		foreach ( var c in word )
		{
			if ( c is ( >= '0' and <= '9' ) or ( >= 'A' and <= 'Z' ) or ( >= 'a' and <= 'z' ) or '.' or '_' )
			{
				_ = sb.Append( c );
			}
		}

		return sb.ToString();
	}

	/// <summary> Genera vector de una secuencia numérica </summary>
	/// <param name="lenght"> Longitud del vector </param>
	/// <param name="start"> número en que se inicia la secuencia </param>
	/// <param name="dblStep"> número incremental a la secuencia </param>
	public static double[] Sequence( int lenght, double start = 1d, double dblStep = 1d )
	{
		if ( lenght < 1 )
		{
			throw new Exception( "lenght must be greater than zero" );
		}

		var vRes = new double[ lenght ];
		var dblSum = start;
		for ( var i = 0; i < lenght; i++ )
		{
			vRes[ i ] = dblSum;
			dblSum += dblStep;
		}

		return vRes;
	}

	/// <summary> Valida el valor e intenta realizar una conversión al tipo de dato solicitado </summary>
	/// <param name="objValue"> Valor </param>
	/// <param name="dataType"> Tipo de dato del valor </param>
	/// <param name="valueOnError"> Valor en caso de fallar la conversión </param>
	/// <param name="arraySeparator"> Separador para el caso de arrays desde una cadena de texto </param>
	/// <returns> El <![CDATA[value]]> convertido a <![CDATA[datatype]]> , en error devuelve <![CDATA[valueOnError]]> </returns>
	public static object? Verify( object? objValue, string dataType = "STR", object? valueOnError = null, string? arraySeparator = null )
	{
		if ( objValue == null || objValue == DBNull.Value )
		{
			return valueOnError;
		}

		dataType = dataType.ToUpperInvariant().Trim();
		var value = objValue.ToString() ?? string.Empty;
		return dataType switch
		{
			"DBL" => double.TryParse( value, out var res ) ? res : valueOnError,
			"DOUBLE" => double.TryParse( value, out var res ) ? res : valueOnError,
			"BOO" => bool.TryParse( value, out var res ) ? res : valueOnError,
			"BOOLEAN" => bool.TryParse( value, out var res ) ? res : valueOnError,
			"BOI" => bool.TryParse( value, out var res ) ? res : valueOnError,
			"BOL" => bool.TryParse( value, out var res ) ? res : valueOnError,
			"DTE" => DateTime.TryParse( value, out DateTime res ) ? res : valueOnError,
			"DATE" => DateTime.TryParse( value, out DateTime res ) ? res : valueOnError,
			"TME" => DateTime.TryParse( value, out DateTime res ) ? res : valueOnError,
			"TIME" => DateTime.TryParse( value, out DateTime res ) ? res : valueOnError,
			"STR" => value,
			"STRING" => value,
			"FTR" => value.Trim(),
			"UTR" => value.ToUpperInvariant(),
			"LTR" => value.ToLowerInvariant(),
			"INT" => int.TryParse( value, out var res ) ? res : valueOnError,
			"INTEGER" => int.TryParse( value, out var res ) ? res : valueOnError,
			"INC" => int.TryParse( value, out var res ) ? res : valueOnError,
			"ARRAY" => value.Split( new[] { arraySeparator ?? "," }, StringSplitOptions.RemoveEmptyEntries ).Select( s => s.Trim() ).ToArray(),
			"ARR" => value.Split( new[] { arraySeparator ?? "," }, StringSplitOptions.RemoveEmptyEntries ).Select( s => s.Trim() ).ToArray(),
			_ => default
		};
	}
}
