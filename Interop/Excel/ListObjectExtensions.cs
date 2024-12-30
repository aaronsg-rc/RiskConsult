using Microsoft.Office.Interop.Excel;
using RiskConsult.Extensions;
using Range = Microsoft.Office.Interop.Excel.Range;

namespace RiskConsult.Interop.Excel;

public static class ListObjectExtensions
{
#warning hacer pruebas de todas las inserciones de tabla y ver si se pueden armar un array para los métodos append

	public enum TableImportType
	{
		/// <summary>
		///<para> Se asume que la primer fila del array es de encabezados, el resto de filas son datos.</para>
		///<para> Se reemplaza todo el contenido de la tabla y se reasignan encabezados</para>
		/// </summary>
		ReplaceAllColumns,

		/// <summary>
		///<para> Se reinicia la tabla y se borran todos los contenidos</para>
		///<para> Se reemplaza el contenido de la tabla por el del array </para>
		/// </summary>
		ReplaceBody,

		/// <summary>
		///<para> Se eliminan las filas excedentes de la tabla según el número de filas de los datos</para>
		///<para> Se asume que la primer fila del array es de encabezados, el resto de filas son datos.</para>
		///<para> Las filas de datos reemplazan las columnas que coincidan por el nombre de encabezados </para>
		/// </summary>
		ReplaceMatchColumns,

		/// <summary>
		/// <para>Se valida que los datos tengan la misma cantidad de columnas.</para>
		///<para> Se asume que la primer fila del array es de encabezados, el resto de filas son datos.</para>
		///<para> Si existe al menos una columna coincidente se agregan tantas filas datos tenga el array</para>
		///<para> Las filas de datos se agregan en las columnas que coincidan por el nombre de encabezados </para>
		/// </summary>
		AppendBody,

		/// <summary>
		///<para> Se asume que la primer fila del array es de encabezados, el resto de filas son datos.</para>
		///<para> Si existe al menos una columna coincidente se agregan tantas filas datos tenga el array</para>
		///<para> Las filas de datos se agregan en las columnas que coincidan por el nombre de encabezados </para>
		/// </summary>
		AppendMatchColumns,

		/// <summary>
		///<para> Se asume que la primer fila del array es de encabezados, el resto de filas son datos.</para>
		///<para> Si existe al menos una columna nueva o coincidente se agregan tantas filas datos tenga el array</para>
		///<para> Las filas de datos se agregan en las columnas que coincidan por el nombre de encabezados </para>
		///<para> Si no existe la columna la crea y asigna el nombre de encabezado</para>
		/// </summary>
		AppendAllColumns
	}

	public static void Clear( this ListObject table )
	{
		ArgumentNullException.ThrowIfNull( table );

		// Valido que tenga datos
		if ( table.DataBodyRange is null )
		{
			_ = table.ListRows.Add();
		}

		// Quito filtro
		if ( table.ShowAutoFilter )
		{
			table.AutoFilter.ShowAllData();
		}

		//Borro excedente de filas
		if ( table.DataBodyRange?.Rows.Count > 1 )
		{
			( ( Range ) table.DataBodyRange.Rows[ "2:" + table.ListRows.Count ] ).Delete();
		}

		//Borro contenido sin borrar fórmulas
		table.DataBodyRange?.Rows.SpecialCells( XlCellType.xlCellTypeConstants ).ClearContents();
	}

	public static bool ContainsAnyHeader( this ListObject table, IEnumerable<string> headers )
	{
		foreach ( ListColumn col in table.ListColumns )
		{
			if ( headers.Contains( col.Name ) )
			{
				return true;
			}
		}

		return false;
	}

	public static int GetColumnIndex( this ListObject table, string name )
	{
		foreach ( ListColumn column in table.ListColumns )
		{
			if ( column.Name == name )
			{
				return column.Index;
			}
		}

		return -1;
	}

	public static void ImportData( this ListObject table, object[,] data, TableImportType importType )
	{
		XlCalculation initialCalculation = table.Application.Calculation;
		var initialScreenUpdating = table.Application.ScreenUpdating;

		try
		{
			table.Application.Calculation = XlCalculation.xlCalculationManual;
			table.Application.ScreenUpdating = false;

			if ( importType is TableImportType.ReplaceBody )
			{
				table.ReplaceBody( data );
			}
			else if ( importType is TableImportType.ReplaceAllColumns )
			{
				table.ReplaceAllColumns( data );
			}
			else if ( importType is TableImportType.ReplaceMatchColumns )
			{
				table.ReplaceMatchColumns( data );
			}
			else if ( importType is TableImportType.AppendBody )
			{
				table.AppendBody( data );
			}
			else if ( importType is TableImportType.AppendAllColumns )
			{
				table.AppendAllColumns( data );
			}
			else if ( importType is TableImportType.AppendMatchColumns )
			{
				table.AppendMatchColumns( data );
			}
		}
		catch ( Exception ) { throw; }
		finally
		{
			table.Application.Calculation = initialCalculation;
			table.Application.ScreenUpdating = initialScreenUpdating;
		}
	}

	public static void ResizeDataBodyRange( this ListObject table, int rows, int cols )
	{
		Range newRng = ( ( Range ) table.HeaderRowRange[ 1, 1 ] ).Resize[ rows + 1, cols ];
		table.Resize( newRng );
	}

	public static void SetHeadersValues<T>( this ListObject table, IEnumerable<T> headers )
	{
		// Convertir el IEnumerable a una lista para facilitar el acceso por índice
		var headerNamesList = headers.Select( v => v?.ToString() ?? string.Empty ).ToArray();
		if ( table.ListColumns.Count != headerNamesList.Length )
		{
			throw new ArgumentException( $"Number of columns doesn't match with number of headers" );
		}

		// Asignar los nombres de encabezados
		for ( var i = 0; i < table.ListColumns.Count; i++ )
		{
			table.ListColumns[ i + 1 ].Name = headerNamesList[ i ];
		}
	}

	private static void AppendAllColumns( this ListObject table, object[,] data )
	{
		// Valido que tengan datos para importar
		var dataRows = data.GetLength( 0 );
		var dataCols = data.GetLength( 1 );
		if ( dataRows <= 1 || dataCols <= 0 )
		{
			return;
		}

		// Redimensiono tabla
		var tableLastRow = table.ListRows.Count;
		table.ResizeDataBodyRange( tableLastRow + dataRows, table.ListColumns.Count );

		// Agrego filas de datos
		var lbRows = data.GetLowerBound( 0 );
		var lbCols = data.GetLowerBound( 1 );
		for ( var dataCol = lbCols; dataCol <= data.GetUpperBound( 1 ); dataCol++ )
		{
			// Obtengo índice de la columna en la tabla
			var header = data[ lbRows, dataCol ].ToString() ?? string.Empty;
			var tableCol = table.GetColumnIndex( header );
			if ( tableCol == -1 )
			{
				ListColumn tableNewCol = table.ListColumns.Add();
				tableNewCol.Name = header;
				tableCol = tableNewCol.Index;
			}

			// Agrego datos
			var tableStartCell = ( Range ) table.DataBodyRange[ tableLastRow + 1, tableCol ];
			tableStartCell.Resize[ dataRows - 1, 1 ].Value = data.GetSection( lbRows + 1, dataRows - 1, dataCol, 1 );
		}
	}

	private static void AppendBody( this ListObject table, object[,] data )
	{
		var dataRows = data.GetLength( 0 );
		var dataCols = data.GetLength( 1 );
		if ( dataCols != table.ListColumns.Count )
		{
			throw new Exception( "Invalid number of columns on data" );
		}

		var tableLastRow = table.ListRows.Count;
		table.ResizeDataBodyRange( tableLastRow + dataRows, dataCols );
		var tableStartNewRow = ( Range ) table.ListRows[ tableLastRow + 1 ].Range[ 1, 1 ];
		tableStartNewRow.Resize[ dataRows, dataCols ].Value = data;
	}

	private static void AppendMatchColumns( this ListObject table, object[,] data )
	{
		// Verifica si hay suficientes filas en la tabla para acomodar los datos, se asume que la primera fila es de encabezados
		var dataRows = data.GetLength( 0 );
		var dataCols = data.GetLength( 1 );
		if ( dataRows <= 1 || dataCols <= 0 )
		{
			return;
		}

		// Valido que exista al menos un encabezado válido
		var lbRows = data.GetLowerBound( 0 );
		var lbCols = data.GetLowerBound( 1 );
		var ubCols = data.GetUpperBound( 1 );
		var tableHasMatchs = false;
		for ( var dataCol = lbCols; dataCol <= ubCols; dataCol++ )
		{
			var header = data[ lbRows, dataCol ].ToString() ?? string.Empty;
			var tableCol = table.GetColumnIndex( header );
			if ( tableCol != -1 )
			{
				tableHasMatchs = true;
			}
		}

		if ( !tableHasMatchs )
		{
			return;
		}

		// Redimensiono tabla
		var tableLastRow = table.ListRows.Count;
		table.ResizeDataBodyRange( tableLastRow + dataRows, table.ListColumns.Count );

		// Agrego filas de datos
		for ( var dataCol = lbCols; dataCol <= ubCols; dataCol++ )
		{
			// Obtengo índice de la columna en la tabla
			var header = data[ lbRows, dataCol ].ToString() ?? string.Empty;
			var tableCol = table.GetColumnIndex( header );
			if ( tableCol == -1 )
			{
				continue;
			}

			// Agrego datos
			var tableStartCell = ( Range ) table.DataBodyRange[ tableLastRow + 1, tableCol ];
			tableStartCell.Resize[ dataRows - 1, 1 ].Value = data.GetSection( lbRows + 1, dataRows - 1, dataCol, 1 );
		}
	}

	private static void ReplaceAllColumns( this ListObject table, object[,] data )
	{
		var rows = data.GetLength( 0 );
		var cols = data.GetLength( 1 );
		var lb_0 = data.GetLowerBound( 0 );
		table.ResizeDataBodyRange( rows, cols );
		table.SetHeadersValues( data.GetRow( lb_0 ) );
		table.DataBodyRange.Value = data.GetRows( lb_0 + 1, rows - 1 );
	}

	private static void ReplaceBody( this ListObject table, object[,] data )
	{
		var rows = data.GetLength( 0 );
		var cols = data.GetLength( 1 );
		table.ResizeDataBodyRange( rows, cols );
		table.DataBodyRange.Value = data;
	}

	private static void ReplaceMatchColumns( this ListObject table, object[,] data )
	{
		// Verifica si hay suficientes filas en la tabla para acomodar los datos, se asume que la primera fila es de encabezados
		var dataRows = data.GetLength( 0 );
		var dataCols = data.GetLength( 1 );
		if ( dataRows <= 1 || dataCols <= 0 )
		{
			return;
		}

		// Valido que exista al menos un encabezado válido
		var lbRows = data.GetLowerBound( 0 );
		var lbCols = data.GetLowerBound( 1 );
		var ubCols = data.GetUpperBound( 1 );
		var tableHasMatchs = false;
		for ( var dataCol = lbCols; dataCol <= ubCols; dataCol++ )
		{
			var header = data[ lbRows, dataCol ].ToString() ?? string.Empty;
			var tableCol = table.GetColumnIndex( header );
			if ( tableCol != -1 )
			{
				tableHasMatchs = true;
			}
		}

		if ( !tableHasMatchs )
		{
			return;
		}

#warning VALIDAR QUE SE borren las filas excedentes al reducir
		// Redimensiono tabla
		table.ResizeDataBodyRange( dataRows, table.ListColumns.Count );

		// Agrego filas de datos
		for ( var dataCol = lbCols; dataCol <= ubCols; dataCol++ )
		{
			// Obtengo índice de la columna en la tabla
			var header = data[ lbRows, dataCol ].ToString() ?? string.Empty;
			var tableCol = table.GetColumnIndex( header );
			if ( tableCol == -1 )
			{
				continue;
			}

			// Agrego datos
			table.ListColumns[ tableCol ].Range.Value = data.GetSection( lbRows + 1, dataRows - 1, dataCol, 1 );
		}
	}
}
