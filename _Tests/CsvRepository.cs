using RiskConsult.Data;
using RiskConsult.Data.Interfaces;

namespace RiskConsult._Tests;

public class CsvRepository<T>( string filePath, IEnumerable<IPropertyMap> properties, bool hasHeaders ) : ICsvRepository<T> where T : new()
{
	private readonly string _filePath = filePath;
	private readonly bool _hasHeaders = hasHeaders;
	private readonly IPropertyMap[] _pks = [ .. properties.Where( p => p.IsPrimaryKey ) ];
	public IEnumerable<IPropertyMap> Properties { get; } = properties;

	public void Delete( params T[] entities )
	{
		try
		{
			var pkSet = entities.Select( entity => GetPrimaryKeyValue( entity, _pks ) ).ToHashSet();
			var tempFile = Path.GetTempFileName();
			using ( var reader = new StreamReader( _filePath ) )
			using ( var writer = new StreamWriter( tempFile ) )
			{
				if ( _hasHeaders )
				{
					_ = reader.ReadLine();
					WriteHeaders( writer, Properties );
				}

				while ( reader.EndOfStream == false )
				{
					var line = reader.ReadLine() ?? string.Empty;
					var lineValues = line.Split( ',' );
					var linePk = GetPrimaryKeyValue( lineValues, _pks );
					if ( pkSet.Contains( linePk ) )
					{
						continue;
					}

					writer.WriteLine( line );
				}
			}

			File.Delete( _filePath );
			File.Move( tempFile, _filePath );
		}
		catch ( Exception ex )
		{
			Console.WriteLine( $"Error al eliminar la línea del archivo CSV: {ex.Message}" );
			throw;
		}
	}

	public K[] GetAll<K>() where K : T, new()
	{
		var entities = new List<K>();
		using var reader = new StreamReader( _filePath );
		if ( _hasHeaders )
		{
			reader.ReadLine();
		}

		var line = string.Empty;
		while ( string.IsNullOrEmpty( line = reader.ReadLine() ) == false )
		{
			var lineValues = line.Split( ',' );
			K entity = GetEntity<K>( lineValues, Properties );
			entities.Add( entity );
		}

		return entities.ToArray();
	}

	public T? GetById( string id )
	{
		using var stream = new StreamReader( _filePath );
		while ( stream.EndOfStream == false )
		{
			var line = stream.ReadLine() ?? string.Empty;
			var lineValues = line.Split( ',' );
			var linePk = GetPrimaryKeyValue( lineValues, _pks );
			if ( linePk == id )
			{
				T entity = GetEntity<T>( lineValues, Properties );
				return entity;
			}
		}

		return default;
	}

	public T? GetById( object id ) => GetById( id.ToString() ?? string.Empty );

	public void Insert( params T[] entities )
	{
		using var writer = new StreamWriter( _filePath, true );
		foreach ( T? entity in entities )
		{
			var line = GetEntityLine( entity, Properties );
			writer.WriteLine( line );
		}
	}

	public void Update( params T[] entities )
	{
		try
		{
			var entitiesDic = entities.ToDictionary( entity => GetPrimaryKeyValue( entity, _pks ) );
			var tempFile = Path.GetTempFileName();
			using ( var writer = new StreamWriter( tempFile ) )
			using ( var reader = new StreamReader( _filePath ) )
			{
				if ( _hasHeaders )
				{
					_ = reader.ReadLine();
					WriteHeaders( writer, Properties );
				}

				while ( reader.EndOfStream == false )
				{
					var line = reader.ReadLine() ?? string.Empty;
					var lineValues = line.Split( ',' );
					var linePk = GetPrimaryKeyValue( lineValues, _pks );
					if ( entitiesDic.TryGetValue( linePk, out T? entityToUpdate ) )
					{
						line = GetEntityLine( entityToUpdate, Properties );
						_ = entitiesDic.Remove( linePk );
					}

					writer.WriteLine( line );
				}
			}

			File.Delete( _filePath );
			File.Move( tempFile, _filePath );
		}
		catch ( Exception ex )
		{
			Console.WriteLine( $"Error al actualizar la línea del archivo CSV: {ex.Message}" );
			throw;
		}
	}

	private static K GetEntity<K>( string[] lineValues, IEnumerable<IPropertyMap> columns ) where K : new()
	{
		var entity = new K();
		foreach ( IPropertyMap col in columns )
		{
			Type type = col.PropertyInfo.PropertyType;
			var value = Convert.ChangeType( lineValues[ col.ColumnIndex ], type );
			col.PropertyInfo.SetValue( entity, value );
		}

		return entity;
	}

	private static string GetEntityLine( T entity, IEnumerable<IPropertyMap> columns ) => string.Join( ',', columns.Select( prop => prop.PropertyInfo.GetValue( entity ) ) );

	private static string GetPrimaryKeyValue( string[] values, IEnumerable<IPropertyMap> pks ) => string.Join( ',', pks.Select( pk => values[ pk.ColumnIndex ] ) );

	private static string GetPrimaryKeyValue( T entity, IEnumerable<IPropertyMap> pks ) => string.Join( ',', pks.Select( pk => pk.PropertyInfo.GetValue( entity )?.ToString() ?? string.Empty ) );

	private static void WriteHeaders( StreamWriter writer, IEnumerable<IPropertyMap> columns )
	{
		var headers = string.Join( ',', columns.Select( c => c.ColumnName ) );
		writer.WriteLine( headers );
	}
}
