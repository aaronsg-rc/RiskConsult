using System.Numerics;

namespace RiskConsult.Extensions;

public static class ArrayExtensions
{
	/// <summary> Agrega encabezados a una matriz a partir de un ienumerable </summary>
	/// <typeparam name="T"> Tipo de dato contenido en la matriz </typeparam>
	/// <param name="values"> Matriz de valores </param>
	/// <param name="headers"> Enumerable de encabezados </param>
	/// <returns> Matriz de strings donde su primer fila sera de encabezados </returns>
	public static string[,] AddHeaders<T>( this T[,] values, IEnumerable<string> headers )
	{
		var rows = values.GetLength( 0 );
		var cols = values.GetLength( 1 );
		var result = new string[ rows + 1, cols ];
		var headerIndex = 0;

		foreach ( var header in headers.Take( cols ) )
		{
			result[ 0, headerIndex++ ] = header;
		}

		for ( var i = 0; i < rows; i++ )
		{
			for ( var j = 0; j < cols; j++ )
			{
				result[ i + 1, j ] = values[ i, j ]?.ToString() ?? string.Empty;
			}
		}

		return result;
	}

	/// <summary> Agrega encabezados y primera fila a una matriz a partir de un ienumerable de encabezados y de primera columna </summary>
	/// <typeparam name="T"> Tipo de dato contenido en la matriz </typeparam>
	/// <param name="values"> Matriz de valores </param>
	/// <param name="headers"> Enumerable de encabezados </param>
	/// <returns> Matriz de strings donde su primer fila sera de encabezados y tendra una primer columna </returns>
	public static string[,] AddHeadersAndFirstColumn<T>( this T[,] values, IEnumerable<string> headers, IEnumerable<string> firstColumn )
	{
		var rows = values.GetLength( 0 );
		var cols = values.GetLength( 1 );
		var result = new string[ rows + 1, cols + 1 ];

		var headerIndex = 0;
		foreach ( var header in headers.Take( cols ) )
		{
			result[ 0, headerIndex + 1 ] = header;
			headerIndex++;
		}

		var columnIndex = 0;
		foreach ( var value in firstColumn.Take( rows ) )
		{
			result[ columnIndex + 1, 0 ] = value;
			columnIndex++;
		}

		for ( var i = 0; i < rows; i++ )
		{
			for ( var j = 0; j < cols; j++ )
			{
				result[ i + 1, j + 1 ] = values[ i, j ]?.ToString() ?? string.Empty;
			}
		}

		return result;
	}

	public static double[] AsDoublesArray<T>( this IEnumerable<T> values ) where T : struct, INumberBase<T>
	{
		if ( values is double[] results )
		{
			return results;
		}

		var size = values.Count();
		results = new double[ size ];
		var i = 0;
		foreach ( T value in values )
		{
			results[ i++ ] = Convert.ToDouble( value );
		}

		return results;
	}

	/// <summary> Valida si el valor buscado existe dentro de la matriz </summary>
	/// <typeparam name="T"> El tipo de elementos contenidos en la matriz </typeparam>
	/// <param name="values"> La matriz de origen. </param>
	/// <param name="searchedValue"> Valor buscado en la matriz </param>
	public static bool Contains<T>( this T[,] values, T searchedValue ) where T : IEquatable<T>
	{
		foreach ( T value in values )
		{
			if ( value.Equals( searchedValue ) )
			{
				return true;
			}
		}

		return false;
	}

	/// <summary> Obtiene una columna de una matriz multidimensional a partir del índice especificado. </summary>
	/// <typeparam name="T"> El tipo de elementos contenidos en la matriz. </typeparam>
	/// <param name="values"> La matriz de origen. </param>
	/// <param name="index"> El índice de la columna. </param>
	/// <returns> Una nueva matriz que contiene la sección especificada. </returns>
	public static IEnumerable<T> GetColumn<T>( this T[,] values, int index )
	{
		var rows = values.GetLength( 0 );
		for ( var i = 0; i < rows; i++ )
		{
			yield return values[ i, index ];
		}
	}

	/// <summary> Obtiene un enumerable que representa las columnas del array </summary>
	/// <typeparam name="T"> El tipo de elementos contenidos en la matriz </typeparam>
	/// <param name="values"> La matriz de origen </param>
	/// <returns> Enumerable de enumerables que representan las columnas del array </returns>
	public static IEnumerable<IEnumerable<T>> GetColumns<T>( this T[,] values )
	{
		var cols = values.GetLength( 1 );
		for ( var j = 0; j < cols; j++ )
		{
			yield return values.GetColumn( j );
		}
	}

	/// <summary> Obtiene una sección rectangular de una matriz multidimensional a partir de los índices de inicio y el número columnas especificados. </summary>
	/// <typeparam name="T"> El tipo de elementos contenidos en la matriz. </typeparam>
	/// <param name="values"> La matriz de origen. </param>
	/// <param name="indexInitialColumn"> El índice de inicio de la columna. </param>
	/// <param name="columnsCount"> El número de columnas a incluir. </param>
	/// <returns> Una nueva matriz que contiene la sección especificada. </returns>
	public static T[,] GetColumns<T>( this T[,] values, int indexInitialColumn, int columnsCount )
		=> values.GetSection( 0, values.GetLength( 0 ), indexInitialColumn, columnsCount );

	public static IEnumerable<IEnumerable<T>> GetColumns<T>( this T[,] values, IEnumerable<int> indexes )
	{
		foreach ( var index in indexes )
		{
			yield return values.GetColumn( index );
		}
	}

	/// <summary> </summary>
	/// <typeparam name="T"> </typeparam>
	/// <param name="values"> </param>
	/// <param name="value"> </param>
	/// <param name="count"> </param>
	/// <returns> </returns>
	public static IEnumerable<T> GetLastElementsBefore<T>( this IEnumerable<T> values, T value, int count ) where T : IEquatable<T>
	{
		var index = -1;
		var i = 0;
		foreach ( T element in values )
		{
			if ( element.Equals( value ) )
			{
				index = i;
				break;
			}

			i++;
		}

		return index == -1 || index < count ? ( IEnumerable<T> ) ( [] ) : values.Skip( index - count + 1 ).Take( count );
	}

	/// <summary> Obtiene una fila de una matriz multidimensional a partir del índice especificado. </summary>
	/// <typeparam name="T"> El tipo de elementos contenidos en la matriz. </typeparam>
	/// <param name="values"> La matriz de origen. </param>
	/// <param name="index"> El índice de la fila. </param>
	/// <returns> Una nueva matriz que contiene la sección especificada. </returns>
	public static IEnumerable<T> GetRow<T>( this T[,] values, int index )
	{
		var cols = values.GetLength( 1 );
		for ( var j = 0; j < cols; j++ )
		{
			yield return values[ index, j ];
		}
	}

	/// <summary> Obtiene un enumerable que representa las filas del array </summary>
	/// <typeparam name="T"> El tipo de elementos contenidos en la matriz </typeparam>
	/// <param name="values"> La matriz de origen </param>
	/// <returns> Enumerable de enumerables que representan las filas del array </returns>
	public static IEnumerable<IEnumerable<T>> GetRows<T>( this T[,] values )
	{
		var rows = values.GetLength( 0 );
		for ( var i = 0; i < rows; i++ )
		{
			yield return values.GetRow( i );
		}
	}

	public static IEnumerable<IEnumerable<T>> GetRows<T>( this T[,] values, IEnumerable<int> indexes )
	{
		foreach ( var index in indexes )
		{
			yield return values.GetRow( index );
		}
	}

	/// <summary> Obtiene una sección rectangular de una matriz multidimensional a partir de los índices de inicio y el número de filas especificados. </summary>
	/// <typeparam name="T"> El tipo de elementos contenidos en la matriz. </typeparam>
	/// <param name="values"> La matriz de origen. </param>
	/// <param name="indexInitialRow"> El índice de inicio de la fila. </param>
	/// <param name="rowsCount"> El número de filas a incluir. </param>
	/// <returns> Una nueva matriz que contiene la sección especificada. </returns>
	public static T[,] GetRows<T>( this T[,] values, int indexInitialRow, int rowsCount )
		=> values.GetSection( indexInitialRow, rowsCount, 0, values.GetLength( 1 ) );

	/// <summary>
	/// Obtiene una sección rectangular de una matriz multidimensional a partir de los índices de inicio y el número de filas y columnas especificados.
	/// </summary>
	/// <typeparam name="T"> El tipo de elementos contenidos en la matriz. </typeparam>
	/// <param name="values"> La matriz de origen. </param>
	/// <param name="indexInitialRow"> El índice de inicio de la fila. </param>
	/// <param name="rowsCount"> El número de filas a incluir. </param>
	/// <param name="indexInitialCol"> El índice de inicio de la columna. </param>
	/// <param name="colsCount"> El número de columnas a incluir. </param>
	/// <returns> Una nueva matriz que contiene la sección especificada. </returns>
	public static T[,] GetSection<T>( this T[,] values, int indexInitialRow, int rowsCount, int indexInitialCol, int colsCount )
	{
		ArgumentNullException.ThrowIfNull( values );
		ArgumentOutOfRangeException.ThrowIfGreaterThan( indexInitialRow + rowsCount, values.GetLength( 0 ), nameof( indexInitialRow ) );
		ArgumentOutOfRangeException.ThrowIfGreaterThan( indexInitialCol + colsCount, values.GetLength( 1 ), nameof( indexInitialCol ) );

		var arrRes = new T[ rowsCount, colsCount ];
		for ( var i = 0; i < rowsCount; i++ )
		{
			for ( var j = 0; j < colsCount; j++ )
			{
				arrRes[ i, j ] = values[ indexInitialRow + i, indexInitialCol + j ];
			}
		}

		return arrRes;
	}

	/// <summary> Ordena la matriz en función de una columna </summary>
	/// <param name="columnIndex"> Índice de la columna que va a ordenar la matriz </param>
	/// <param name="descending"> Determina si el orden debe ser descentente </param>
	public static void OrderBy<T>( this T[,] values, int columnIndex, bool descending = false )
		where T : IComparable<T>
	{
		// Obtengo vector de índices que corresponden al orden de la columna
		var orderedIndex = values.GetColumn( columnIndex ).GetSortedIndexesBy( v => v, descending ).ToArray();

		// Clono matriz y reasigno valores
		var mTmp = ( T[,] ) values.Clone();
		for ( var i = 0; i < values.GetLength( 0 ); i++ )
		{
			for ( var j = 0; j < values.GetLength( 1 ); j++ )
			{
				values[ i, j ] = mTmp[ orderedIndex[ i ], j ];
			}
		}
	}

	/// <summary> Es la matriz que queda después de borrar el renglón i y la columna j </summary>
	/// <param name="values"> Matriz </param>
	/// <param name="i"> Índice de fila </param>
	/// <param name="j"> Índice de columna </param>
	/// <returns> Matriz sin fila i y sin columna j </returns>
	public static T[,] SubMatrix<T>( this T[,] values, int i, int j )
	{
		ArgumentNullException.ThrowIfNull( values );

		var rows = values.GetLength( 0 );
		var cols = values.GetLength( 1 );
		var results = new T[ rows - 1, cols - 1 ];
		var resRow = 0;
		for ( var iRow = 0; iRow < rows; iRow++ )
		{
			if ( iRow == i )
			{
				continue;
			}

			var resCol = 0;
			for ( var iCol = 0; iCol < cols; iCol++ )
			{
				if ( iCol == j )
				{
					continue;
				}

				results[ resRow, resCol++ ] = values[ iRow, iCol ];
			}

			resRow++;
		}

		return results;
	}

	/// <summary> Intercambia posición de dos columnas </summary>
	/// <param name="values"> Matriz que intercambiara sus colummnas </param>
	/// <param name="iCol_1"> Índice de columna a intercambiar </param>
	/// <param name="iCol_2"> Índice de columna a intercambiar </param>
	public static void SwapColumn<T>( this T[,] values, int iCol_1, int iCol_2 )
	{
		ArgumentNullException.ThrowIfNull( values );
		for ( var iRow = 0; iRow < values.GetLength( 0 ); iRow++ )
		{
			(values[ iRow, iCol_1 ], values[ iRow, iCol_2 ]) = (values[ iRow, iCol_2 ], values[ iRow, iCol_1 ]);
		}
	}

	/// <summary> Intercambia posición de dos filas </summary>
	/// <param name="values"> Matriz que intercambiara sus filas </param>
	/// <param name="iRow_1"> Índice de fila a intercambiar </param>
	/// <param name="iRow_2"> Índice de fila a intercambiar </param>
	public static void SwapRow<T>( this T[,] values, int iRow_1, int iRow_2 )
	{
		ArgumentNullException.ThrowIfNull( values );
		for ( var iCol = 0; iCol < values.GetLength( 1 ); iCol++ )
		{
			(values[ iRow_1, iCol ], values[ iRow_2, iCol ]) = (values[ iRow_2, iCol ], values[ iRow_1, iCol ]);
		}
	}

	public static void ToCsvFile<T>( this T[,] values, string filePath )
	{
		using var writer = new StreamWriter( filePath );
		var rows = values.GetLength( 0 );
		for ( var i = 0; i < rows; i++ )
		{
			var line = string.Join( ',', values.GetRow( i ) );
			writer.WriteLine( line );
		}
	}

	public static object?[,] ToObjectArray<T>( this T[,] values )
	{
		ArgumentNullException.ThrowIfNull( values );

		var rows = values.GetLength( 0 );
		var cols = values.GetLength( 1 );

		var results = new object?[ rows, cols ];
		for ( var i = 0; i < rows; i++ )
		{
			for ( var j = 0; j < cols; j++ )
			{
				results[ i, j ] = values[ i, j ];
			}
		}

		return results;
	}

	/// <summary> Transpone un array de 2 dimensiones </summary>
	/// <param name="values"> Array de 2 dimensiones </param>
	/// <returns> Devuelve array de 2 dimensiones transpuesto </returns>
	public static T[,] Transpose<T>( this T[,] values )
	{
		ArgumentNullException.ThrowIfNull( values );

		// Dimensiones de input
		var rows = values.GetLength( 0 );
		var cols = values.GetLength( 1 );
		var lbRows = values.GetLowerBound( 0 );
		var lbCols = values.GetLowerBound( 1 );
		var ubRows = values.GetUpperBound( 0 );
		var ubCols = values.GetUpperBound( 1 );

		// Defino mi array resultado
		var results = new T[ cols, rows ];
		for ( var i = lbRows; i <= ubRows; i++ )
		{
			for ( var j = lbCols; j <= ubCols; j++ )
			{
				results[ j + lbCols, i + lbRows ] = values[ i, j ];
			}
		}

		return results;
	}
}
