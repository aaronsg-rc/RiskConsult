using RiskConsult.Data.Interfaces;
using System.Data;

namespace RiskConsult.Data;

public interface IRepository<TEntity>
{
	IUnitOfWork UnitOfWork { get; }

	void Delete( params TEntity[] entities );

	TImplementation[] GetAll<TImplementation>() where TImplementation : TEntity, new();

	void Insert( params TEntity[] entities );

	void Update( params TEntity[] entities );
}

public abstract class DbRepository<TEntity> : IRepository<TEntity>, ITableMap
{
	public abstract IPropertyMap[] Properties { get; }
	public abstract string TableName { get; }
	public IUnitOfWork UnitOfWork { get; }

	public DbRepository( IUnitOfWork unitOfWork )
	{
		UnitOfWork = unitOfWork ?? throw new ArgumentNullException( nameof( unitOfWork ) );
	}

	public void Delete( params TEntity[] entities )
	{
		if ( entities.Length == 0 )
		{
			return;
		}

		IPropertyMap[] pkProps = Properties.Where( p => p.IsPrimaryKey ).ToArray();
		var pkConditions = string.Join( " AND ", pkProps.Select( p => $"{p.ColumnName} = @{p.ColumnName}" ) );

		try
		{
			using IDbCommand command = UnitOfWork.CreateCommand();
			command.CommandText = $"DELETE FROM {TableName} WHERE {pkConditions};";
			command.AddParameters( pkProps );
			command.ExecuteForEach( pkProps, entities );
		}
		catch ( Exception e )
		{
			throw new Exception( $"Error deleting: {e.Message}", e );
		}
	}

	public TImplementation[] GetAll<TImplementation>() where TImplementation : TEntity, new()
	{
		var properties = string.Join( ", ", Properties.Select( p => p.ColumnName ) );

		using IDbCommand command = UnitOfWork.CreateCommand();
		command.CommandText = $"SELECT {properties} FROM {TableName};";

		using IDataReader reader = command.ExecuteReader();
		var entities = new List<TImplementation>();
		while ( reader.Read() )
		{
			TImplementation? entity = reader.GetEntity<TImplementation>( Properties );
			entities.Add( entity );
		}

		return [ .. entities ];
	}

	public void Insert( params TEntity[] entities )
	{
		if ( entities.Length == 0 )
		{
			return;
		}

		var propNames = string.Join( ", ", Properties.Select( p => p.ColumnName ) );
		var propParams = string.Join( ", ", Properties.Select( p => $"@{p.ColumnName}" ) );

		try
		{
			using IDbCommand command = UnitOfWork.CreateCommand();
			command.CommandText = $"INSERT INTO {TableName}({propNames}) VALUES ({propParams});";
			command.AddParameters( Properties );
			command.ExecuteForEach( Properties, entities );
		}
		catch ( Exception e )
		{
			throw new Exception( $"Error inserting: {e.Message}", e );
		}
	}

	public void Update( params TEntity[] entities )
	{
		if ( entities.Length == 0 )
		{
			return;
		}

		var valuesToSet = string.Join( ", ", Properties.Where( p => p.IsPrimaryKey == false ).Select( p => $"{p.ColumnName} = @{p.ColumnName}" ) );
		var pkConditions = string.Join( " AND ", Properties.Where( p => p.IsPrimaryKey ).Select( p => $"{p.ColumnName} = @{p.ColumnName}" ) );

		try
		{
			using IDbCommand command = UnitOfWork.CreateCommand();
			command.CommandText = $"UPDATE {TableName} SET {valuesToSet} WHERE {pkConditions};";
			command.AddParameters( Properties );
			command.ExecuteForEach( Properties, entities );
		}
		catch ( Exception e )
		{
			throw new Exception( $"Error updating: {e.Message}", e );
		}
	}
}
