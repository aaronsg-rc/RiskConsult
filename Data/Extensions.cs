using System.Data;

namespace RiskConsult.Data;

public static class Extensions
{
	public static void AddParameters( this IDbCommand command, IPropertyMap[] properties )
	{
		foreach ( IPropertyMap prop in properties )
		{
			IDbDataParameter parameter = command.CreateParameter();
			parameter.ParameterName = $"@{prop.ColumnName}";
			command.Parameters.Add( parameter );
		}
	}

	public static void ExecuteForEach<TEntity>( this IDbCommand command, IPropertyMap[] properties, params TEntity[] entities )
	{
		foreach ( TEntity entity in entities )
		{
			command.SetParametersValues( properties, entity );
			command.ExecuteNonQuery();
		}
	}

	public static TEntity[] GetEntities<TEntity>( this IDbCommand command, IPropertyMap[] properties ) where TEntity : new()
	{
		var entities = new List<TEntity>();
		using IDataReader reader = command.ExecuteReader();
		while ( reader.Read() )
		{
			entities.Add( reader.GetEntity<TEntity>( properties ) );
		}

		return [ .. entities ];
	}

	public static TEntity? GetEntity<TEntity>( this IDbCommand command, IPropertyMap[] properties ) where TEntity : new()
	{
		using IDataReader reader = command.ExecuteReader();
		if ( reader.Read() )
		{
			return reader.GetEntity<TEntity>( properties );
		}

		return default;
	}

	public static TEntity GetEntity<TEntity>( this IDataReader reader, IPropertyMap[] properties ) where TEntity : new()
	{
		var entity = new TEntity();
		foreach ( IPropertyMap property in properties )
		{
			var value = reader[ property.ColumnName ];
			property.PropertyInfo.SetValue( entity, value == DBNull.Value ? null : value );
		}

		return entity;
	}

	public static void SetParametersValues<TEntity>( this IDbCommand command, IPropertyMap[] properties, TEntity entity )
	{
		foreach ( IPropertyMap property in properties )
		{
			var param = ( IDbDataParameter ) command.Parameters[ $"@{property.ColumnName}" ];
			param.Value = property.PropertyInfo.GetValue( entity ) ?? DBNull.Value;
		}
	}
}
