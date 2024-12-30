namespace RiskConsult.FileManager;

public class IniLine
{
	public string? Comment { get; set; }
	public string? Key { get; set; }
	public LineType LineType { get; set; }
	public string? SectionName { get; set; }
	public char? Separator { get; set; }
	public string? Value { get; set; }

	public IniLine( string line, char? separator = null )
	{
		// Obtengo tipo de linea
		var lineClean = line.Trim();
		LineType = GetLineType( lineClean, separator, out var ixComment, out var ixSeparator );
		if ( string.IsNullOrEmpty( lineClean ) )
		{
			throw new ArgumentNullException( nameof( line ) );
		}

		// Si existe comentario lo agrego
		if ( ixComment != -1 )
		{
			Comment = lineClean[ ( ixComment + 1 ).. ].Trim();
		}

		// Nombre de la sección
		if ( LineType == LineType.Section )
		{
			SectionName =
				ixComment == -1
				? lineClean.Replace( "[", "" ).Replace( "]", "" ).ToUpper().Trim()
				: lineClean[ ..ixComment ].Replace( "[", "" ).Replace( "]", "" ).ToUpper().Trim();
		}

		// Variable y valor
		if ( LineType == LineType.Param )
		{
			// Obtengo key
			Key = lineClean[ ..ixSeparator ].Trim();
			if ( string.IsNullOrEmpty( Key.Trim() ) )
			{
				throw new ArgumentException( $"Uneable to find key on line '{line}'" );
			}

			// Obtengo valor hasta el primer comentario si es que existe
			Value =
				ixComment == -1
				? lineClean[ ( ixSeparator + 1 ).. ].Trim()
				: lineClean.Substring( ixSeparator + 1, lineClean.IndexOf( ';', ixSeparator + 1 ) - ixSeparator - 1 ).Trim();

			Separator = separator;
		}
	}

	/// <summary> Obtiene el tipo de linea con el que se trabaja </summary>
	/// <param name="strLine"> Texto contenido en la linea </param>
	/// <param name="separator"> Separador de llave y valor de variables </param>
	/// <returns> Si es un encabezado, una variable o una linea de comentario </returns>
	public static LineType GetLineType( string line, char? separator, out int ixComment, out int ixSeparator )
	{
		ixComment = line.IndexOf( ';' );
		ixSeparator = separator == null ? -1 : line.IndexOf( separator.Value );

		return string.IsNullOrEmpty( line )
			? LineType.Invalid
			: line[ 0 ] == '[' && line.Any( c => c == ']' )
			? LineType.Section
			: line[ 0 ] == ';' ? LineType.Comment : ixSeparator > 0 ? LineType.Param : LineType.Invalid;
	}

	public override string ToString()
	{
		var text = string.Empty;
		if ( LineType == LineType.Section )
		{
			text = $"[ {SectionName} ]";
		}
		else if ( LineType == LineType.Param )
		{
			text = $"{Key} {Separator} {Value}";
		}

		if ( Comment != null )
		{
			text += $"\t;{Comment}";
		}

		return text.Trim();
	}
}