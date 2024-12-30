using RiskConsult.Extensions;
using Range = Microsoft.Office.Interop.Excel.Range;

namespace RiskConsult.Interop.Excel;

public static class RangeExtensions
{
	public static void SetValues<T>( this Range range, T[,] values )
		=> SetValues( range, values.ToObjectArray() );

	public static void SetValues( this Range range, object?[,] values )
	{
		ArgumentNullException.ThrowIfNull( range );

		// Asignar los valores al rango
		range.Value = values ?? throw new ArgumentNullException( nameof( values ) );
	}

	public static object[,] ToArray( this Range range )
	{
		ArgumentNullException.ThrowIfNull( range );

		var arrRng = ( object[,] ) range.Value;
		var rows = arrRng.GetLength( 0 );
		var cols = arrRng.GetLength( 1 );
		var arr = new object[ rows, cols ];
		for ( var i = 0; i < rows; i++ )
		{
			for ( var j = 0; j < cols; j++ )
			{
				arr[ i, j ] = arrRng[ i + 1, j + 1 ];
			}
		}

		return arr;
	}
}
