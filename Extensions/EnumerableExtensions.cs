using System.Numerics;

namespace RiskConsult.Extensions;

public static class EnumerableExtensions
{
	public static IEnumerable<double> AsDoubles<T>( this IEnumerable<T> values )
		where T : struct, INumberBase<T>
	{
		foreach ( T item in values )
		{
			yield return item is double value ? value : Convert.ToDouble( item );
		}
	}

	/// <summary> Se omiten los elementos antes del índice de inicio y se toman solo el número de elementos especificado </summary>
	/// <param name="values"> enumerable del que se quieren extraer valores </param>
	/// <param name="startIndex"> Índice del primer valor a extraer </param>
	/// <param name="count"> Número de elementos que se van a extraer </param>
	public static IEnumerable<T> GetRange<T>( this IEnumerable<T> values, int startIndex, int count )
		=> values.Skip( startIndex ).Take( count );

	/// <summary> Se toman todos los elementos a partir del índice inicial </summary>
	/// <param name="values"> enumerable del que se quieren extraer valores </param>
	/// <param name="startIndex"> Índice del primer valor a extraer </param>
	public static IEnumerable<T> GetRange<T>( this IEnumerable<T> values, int startIndex )
		=> GetRange( values, startIndex, values.Count() - startIndex );

	/// <summary> Obtiene un enumerable de los índices correspondientes a los elementos ordenados </summary>
	/// <param name="descending"> Si es true el orden será descendente, false ascendente </param>
	public static IEnumerable<int> GetSortedIndexesBy<TSource, TKey>( this IEnumerable<TSource> values, Func<TSource, TKey> keySelector, bool descending = false )
	{
		IEnumerable<int> sortedIndexes = values
			.Select( ( value, index ) => new { Value = value, Index = index } )
			.OrderBy( a => keySelector( a.Value ) )
			.Select( x => x.Index );

		return descending ? sortedIndexes.Reverse() : sortedIndexes;
	}

	public static T Product<T>( this IEnumerable<T> source ) where T : struct, IConvertible => source.Aggregate( ( acc, value ) => ( dynamic ) acc * value );

	/// <summary>
	/// Separa los elementos de un enumerable por un separador dado en una matriz en la orientacion indicada tendra n tantas columnas como separadores
	/// tenga la primer linea del array
	/// </summary>
	/// <param name="values"> elementos a separar </param>
	/// <param name="separator"> separador de elementos </param>
	public static string[,] Split( this IEnumerable<string> values, string separator )
	{
		ArgumentNullException.ThrowIfNull( values );
		if ( string.IsNullOrEmpty( separator ) )
		{
			throw new ArgumentNullException( nameof( separator ) );
		}

		var iRow = 0;
		var rowsCount = values.Count();
		var colsCount = values.First().Split( separator.ToCharArray() ).Length;
		var arrRes = new string[ rowsCount, colsCount ];
		foreach ( var line in values )
		{
			var colValues = line.Split( separator.ToCharArray() );
			for ( var j = 0; j < colsCount; j++ )
			{
				arrRes[ iRow, j ] = colValues[ j ];
			}

			iRow++;
		}

		return arrRes;
	}

	public static T[,] ToArrayFromColumns<T>( this IEnumerable<IEnumerable<T>> values )
	{
		ArgumentNullException.ThrowIfNull( values, nameof( values ) );

		var cols = values.Count();
		if ( cols == 0 )
		{
			throw new ArgumentException( "Sequence contains no elements.", nameof( values ) );
		}

		var rows = values.First().Count();
		var results = new T[ rows, cols ];
		var j = 0;
		foreach ( IEnumerable<T> col in values )
		{
			var i = 0;
			foreach ( T? value in col )
			{
				results[ i++, j ] = value;
			}

			j++;
		}

		return results;
	}

	public static T[,] ToArrayFromRows<T>( this IEnumerable<IEnumerable<T>> values )
	{
		ArgumentNullException.ThrowIfNull( values, nameof( values ) );

		var rows = values.Count();
		if ( rows == 0 )
		{
			throw new ArgumentException( "Sequence contains no elements.", nameof( values ) );
		}

		var cols = values.First().Count();
		var results = new T[ rows, cols ];
		var i = 0;
		foreach ( IEnumerable<T> row in values )
		{
			var j = 0;
			foreach ( T? value in row )
			{
				results[ i, j++ ] = value;

				if ( j == cols )
				{
					break;
				}
			}

			i++;
		}

		return results;
	}

	public static T[,] ToColumnArray<T>( this IEnumerable<T> source )
	{
		var i = 0;
		var result = new T[ source.Count(), 1 ];
		foreach ( T? value in source )
		{
			result[ i++, 0 ] = value;
		}

		return result;
	}

	public static T[,] ToRowArray<T>( this IEnumerable<T> source )
	{
		var i = 0;
		var result = new T[ 1, source.Count() ];
		foreach ( T? value in source )
		{
			result[ 0, i++ ] = value;
		}

		return result;
	}
}
