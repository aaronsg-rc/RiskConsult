using Microsoft.Office.Interop.Excel;
using RiskConsult.Extensions;
using RiskConsult.Interop.Excel;
using System.Runtime.InteropServices;
using Range = Microsoft.Office.Interop.Excel.Range;

namespace RiskConsult.Interop;

public static class ExcelExtensions
{
	private static XlCalculation _calculationState;
	private static bool _eventsState;
	private static bool _isOtherMethod;
	private static bool _screenUpdatingState;
	private static bool _stateIsSaved;

	public static void ExportToListObject<T>( this T[,] arr, ListObject table )
		=> arr.ExportToListObject( table, false );

	public static void ExportToListObject<T>( this T[,] arr, ListObject table, bool arrHasHeaders )
	{
		ArgumentNullException.ThrowIfNull( arr );
		ArgumentNullException.ThrowIfNull( table );

		try
		{
			SaveAppState( table );

			// Reinicio tabla
			table.Clear();

			// si no tiene encabezados
			if ( !arrHasHeaders )
			{
				table.ResizeDataBodyRange( arr.GetLength( 0 ), arr.GetLength( 1 ) );
				table.DataBodyRange.Value = arr;
				return;
			}

			// Asigno cuepo del array
			IEnumerable<T> arrHeader = arr.GetRow( 0 );
			T[,] arrBody = arr.GetRows( 1, arr.GetLength( 0 ) - 1 );

			// Redimensiono tabla
			Range rng = table.HeaderRowRange;
			table.Resize( rng.Resize[ arrBody.GetLength( 0 ) + 1, arrBody.GetLength( 1 ) ] );

			// Paso contenido del array
			table.HeaderRowRange.Value = arrHeader;
			table.DataBodyRange.Value = arrBody;
		}
		catch ( Exception )
		{
			throw;
		}
		finally
		{
			LoadAppState( table );
		}
	}

	public static void ExportToRange<T>( this T[,] arrData, Range rngCell )
	{
		ArgumentNullException.ThrowIfNull( arrData );
		ArgumentNullException.ThrowIfNull( rngCell );

		try
		{
			SaveAppState( rngCell );

			// Asigno contenido de array a rango
			rngCell.Resize[ arrData.GetLength( 0 ), arrData.GetLength( 1 ) ].Value = arrData;
		}
		catch ( Exception )
		{
			throw;
		}
		finally
		{
			LoadAppState( rngCell );
		}
	}

	public static void ExportToRangeAsColumn<T>( this IEnumerable<T> enumerable, Range range )
		=> enumerable.ToColumnArray().ExportToRange( range );

	public static void ExportToRangeAsRow<T>( this IEnumerable<T> enumerable, Range range )
		=> enumerable.ToRowArray().ExportToRange( range );

	public static object[,] GetArray( this Worksheet worksheet )
	{
		return worksheet is null
			? throw new ArgumentNullException( nameof( worksheet ) )
			: worksheet.UsedRange.ToArray();
	}

	public static object[,] GetArray( this Worksheet worksheet,
			int initialRow = -1, int initialColumn = -1, int finalRow = -1, int finalColumn = -1 )
	{
		ArgumentNullException.ThrowIfNull( worksheet );

		// Obtengo el rango de la última celda
		Range rngLast = worksheet.Cells.SpecialCells( XlCellType.xlCellTypeLastCell );

		// Obtengo rango que se va a extraer
		initialRow = initialRow <= 0 ? 1 : Math.Max( initialRow, 1 );
		initialColumn = initialColumn <= 0 ? 1 : Math.Max( initialColumn, 1 );
		finalRow = finalRow <= 0 ? rngLast.Row : Math.Min( finalRow, rngLast.Row );
		finalColumn = finalColumn <= 0 ? rngLast.Column : Math.Min( finalColumn, rngLast.Column );
		Range range = worksheet.Range[
			worksheet.Cells[ initialRow, initialColumn ],
			worksheet.Cells[ finalRow, finalColumn ] ];

		return range.ToArray();
	}

	/// <summary> Obtengo el contenido de una pestaña en excel </summary>
	/// <param name="workbookPath"> Dirección completa del libro de excel </param>
	/// <param name="worksheetName"> Nombre de la pestaña, si no se proporciona nombre tomará la pestaña en que abra </param>
	/// <param name="preserveWorkbook"> Decide si se mantiene abierto el libro después de extraer el array </param>
	/// <param name="preserveApplication"> Decide si se mantiene abierto excel después de extraer el array </param>
	/// <param name="initialRow"> Fila inicial base 1 </param>
	/// <param name="initialColumn"> &gt;Columna inicial base 1 </param>
	/// <param name="finalRow"> &gt;Fila final base 1 </param>
	/// <param name="finalColumn"> &gt;Columna final base 1 </param>
	/// <returns> Array con el contenido de la pestaña </returns>
	public static object[,] GetArrayFromWorksheet(
		string workbookPath, string worksheetName = "",
		bool preserveWorkbook = false, bool preserveApplication = true,
		int initialRow = 1, int initialColumn = 1, int finalRow = -1, int finalColumn = -1 )
	{
		Application? excel = null;
		Workbook? workbook = null;

		try
		{
			// Creo enlace a la aplicación
			excel = new Application();
			workbook = excel.GetWorkbook( workbookPath, true, true );
			Worksheet sheet = string.IsNullOrEmpty( worksheetName )
		? ( Worksheet ) workbook.ActiveSheet
		: workbook.GetWorksheet( worksheetName );

			// Obtengo el rango de la última celda
			return sheet.GetArray( initialRow, initialColumn, finalRow, finalColumn );
		}
		catch ( Exception )
		{
			throw;
		}
		finally
		{
			// Verifico si se requiere que se mantenga instanciado
			//if ( sheet != null )
			//{
			//	_ = Marshal.ReleaseComObject( sheet );
			//}

			if ( preserveWorkbook == false )
			{
				workbook?.Close();
			}

			if ( preserveApplication == false )
			{
				excel?.Quit();
				if ( excel != null )
				{
					_ = Marshal.ReleaseComObject( excel );
				}
			}
		}
	}

	public static ListObject GetTable( this Worksheet worksheet, string tableName )
	{
		return worksheet == null
			? throw new ArgumentNullException( nameof( worksheet ) )
			: worksheet.TableExists( tableName )
			? worksheet.ListObjects[ tableName ]
			: throw new Exception( $"{tableName} doesn't exists on {worksheet.Name}" );
	}

	public static Workbook GetWorkbook( this Application application, string filePath,
			bool readOnly = true, bool ignoreReadOnlyRecommended = true )
	{
		ArgumentNullException.ThrowIfNull( application );

		if ( !File.Exists( filePath ) )
		{
			throw new FileNotFoundException( filePath );
		}

		var wbName = Path.GetFileName( filePath );
		Workbook wb = application.IsWorkbookOpen( wbName )
			? application.Workbooks[ wbName ]
			: application.Workbooks.Open( filePath, ReadOnly: readOnly, IgnoreReadOnlyRecommended: ignoreReadOnlyRecommended );

		return wb;
	}

	public static Worksheet GetWorksheet( this Workbook workbook, string worksheetName )
	{
		return workbook == null
			? throw new ArgumentNullException( nameof( workbook ) )
			: workbook.WorksheetExists( worksheetName )
			? ( Worksheet ) workbook.Worksheets[ worksheetName ]
			: throw new Exception( $"{worksheetName} doesn't exists on {workbook.Name}" );
	}

	public static bool IsWorkbookOpen( this Application application, string name )
	{
		ArgumentNullException.ThrowIfNull( application );

		foreach ( Workbook wb in application.Workbooks )
		{
			if ( wb.Name == name )
			{
				return true;
			}
		}

		return false;
	}

	public static bool TableExists( this Worksheet worksheet, string tableName )
	{
		ArgumentNullException.ThrowIfNull( worksheet );

		foreach ( ListObject table in worksheet.ListObjects )
		{
			if ( table.Name == tableName )
			{
				return true;
			}
		}

		return false;
	}

	public static bool WorksheetExists( this Workbook workbook, string sheetName )
	{
		ArgumentNullException.ThrowIfNull( workbook );

		foreach ( Worksheet ws in workbook.Worksheets )
		{
			if ( ws.Name == sheetName )
			{
				return true;
			}
		}

		return false;
	}

	private static void LoadAppState( dynamic excelObject )
	{
		if ( _isOtherMethod )
		{
			_isOtherMethod = false;
		}

		var appExc = ( Application ) excelObject.Application;
		appExc.ScreenUpdating = _screenUpdatingState;
		appExc.Calculation = _calculationState;
		appExc.EnableEvents = _eventsState;
	}

	private static void SaveAppState( dynamic excelObject )
	{
		var appExc = ( Application ) excelObject.Application;
		_screenUpdatingState = appExc.ScreenUpdating;
		_calculationState = appExc.Calculation;
		_eventsState = appExc.EnableEvents;
		_stateIsSaved = true;

		appExc.ScreenUpdating = false;
		appExc.Calculation = XlCalculation.xlCalculationManual;
		appExc.EnableEvents = false;
	}
}
