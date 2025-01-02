using Microsoft.Office.Interop.Excel;
using RiskConsult.Data;
using RiskConsult.Data.Interfaces;
using RiskConsult.Interop.Excel;
using Range = Microsoft.Office.Interop.Excel.Range;

namespace RiskConsult._Tests;

public abstract class ExcelRepository<T>( ListObject table, IPropertyMap[] properties ) : IRepository<T>, ITableMap where T : new()
{
	public readonly ListObject ListObject = table;
	private readonly IPropertyMap[] _pks = [ .. properties.Where( p => p.IsPrimaryKey ) ];
	public IPropertyMap[] Properties { get; } = properties;
	public string TableName { get; } = table.Name;
	public IUnitOfWork UnitOfWork { get; }

	public void Delete( params T[] entities )
	{
		var entitiesPks = entities.Select( e => GetPrimaryKey( e, _pks ) ).ToHashSet();
		for ( var i = ListObject.ListRows.Count; i > 0; i-- )
		{
			ListRow row = ListObject.ListRows.Item[ i ];
			var rowPk = GetPrimaryKey( row, _pks );
			if ( entitiesPks.Contains( rowPk ) )
			{
				row.Delete();
			}
		}
	}

	public K[] GetAll<K>() where K : T, new()
	{
		var entities = new List<K>();
		var values = ListObject.DataBodyRange.ToArray();
		for ( var i = 0; i < values.GetLength( 0 ); i++ )
		{
			K entity = GetEntity<K>( values, i, Properties );
			entities.Add( entity );
		}

		return entities.ToArray();
	}

	public K? GetById<K>( string id ) where K : new()
	{
		foreach ( ListRow row in ListObject.ListRows )
		{
			var rowPk = GetPrimaryKey( row, _pks );
			if ( id == rowPk )
			{
				K entity = GetEntity<K>( row, Properties );
				return entity;
			}
		}

		return default;
	}

	public T? GetById( object id ) => GetById( id.ToString() ?? string.Empty );

	public void Insert( params T[] entities )
	{
		try
		{
			// Crear una matriz bidimensional para almacenar los valores de las celdas
			var rowCount = entities.Length;
			var columnCount = ListObject.ListColumns.Count;
			var values = new object[ rowCount, columnCount ];

			// Rellenar la matriz de valores de las celdas con los valores de las propiedades de las entidades
			var rowIx = 0;
			foreach ( T? entity in entities )
			{
				foreach ( IPropertyMap prop in Properties )
				{
					var value = prop.PropertyInfo.GetValue( entity );
					if ( value != null )
					{
						values[ rowIx, prop.ColumnIndex ] = value;
					}
				}

				rowIx++;
			}

			// Obtener el rango de celdas donde se y escribo los valores
			Range startCell = ListObject.ListRows.AddEx().Range;
			Range endCell = startCell.Offset[ rowCount - 1, columnCount - 1 ];
			Range writeRange = ListObject.Parent.Range[ startCell, endCell ];
			writeRange.Value = values;
		}
		catch ( Exception ex )
		{
			Console.WriteLine( $"Error al agregar las entidades al ListObject: {ex.Message}" );
			throw;
		}
	}

	public void Update( params T[] entities )
	{
		var entitiesDic = entities.ToDictionary( e => GetPrimaryKey( e, _pks ) );
		var values = ListObject.DataBodyRange.ToArray();
		for ( var i = 0; i < ListObject.ListRows.Count; i++ )
		{
			var rowPk = GetPrimaryKey( values, i, _pks );
			if ( entitiesDic.TryGetValue( rowPk, out T? entity ) )
			{
				foreach ( IPropertyMap prop in Properties )
				{
					values[ i, prop.ColumnIndex ] = prop.PropertyInfo.GetValue( entity ) ?? string.Empty;
				}
			}
		}

		ListObject.DataBodyRange.Value = values;
	}

	private static K GetEntity<K>( ListRow row, IEnumerable<IPropertyMap> columns ) where K : new()
	{
		var entity = new K();
		foreach ( IPropertyMap col in columns )
		{
			dynamic value = row.Range.Cells[ 1, col.ColumnIndex + 1 ].Value;
			col.PropertyInfo.SetValue( entity, value );
		}

		return entity;
	}

	private static K GetEntity<K>( object[,] values, int row, IEnumerable<IPropertyMap> columns ) where K : new()
	{
		var entity = new K();
		foreach ( IPropertyMap col in columns )
		{
			col.PropertyInfo.SetValue( entity, values[ row, col.ColumnIndex ] );
		}

		return entity;
	}

	private static string GetPrimaryKey( T entity, IEnumerable<IPropertyMap> pks ) => string.Join( ',', pks.Select( pk => pk.PropertyInfo.GetValue( entity ) ) );

	private static string GetPrimaryKey( object[,] values, int row, IEnumerable<IPropertyMap> pks ) => string.Join( ',', pks.Select( pk => values[ row, pk.ColumnIndex ] ) );

	private static string GetPrimaryKey( ListRow row, IEnumerable<IPropertyMap> pks ) => string.Join( ',', pks.Select( pk => row.Range[ 1, pk.ColumnIndex + 1 ].Value ) );
}
