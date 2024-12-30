using RiskConsult.Data.Interfaces;
using System.Data;

namespace RiskConsult.Data;

public abstract class DbRepository<T>( IUnitOfWork unitOfWork ) : IRepository<T>, ITableMap
{
	protected IUnitOfWork UnitOfWork { get; } = unitOfWork;
	public abstract IPropertyMap[] Properties { get; }
	public abstract string TableName { get; }

	public void Delete( params T[] entities )
	{
		if ( entities.Length == 0 )
		{
			return;
		}

		try
		{
			var pkConditions = string.Join( " AND ", Properties
				.Where( p => p.IsPrimaryKey )
				.Select( p => $"{p.ColumnName} = @{p.ColumnName}" ) );

			UnitOfWork.ExecuteCommandForEach( $"DELETE FROM {TableName} WHERE {pkConditions};", Properties, entities );
		}
		catch ( Exception e )
		{
			throw new Exception( $"Error deleting: {e.Message}", e );
		}
	}

	public K[] GetAll<K>() where K : T, new()
	{
		try
		{
			var columns = string.Join( ", ", Properties.Select( p => p.ColumnName ) );
			using IDbCommand command = UnitOfWork.CreateCommand();
			command.CommandText = $"SELECT {columns} FROM {TableName};";

			return UnitOfWork.GetCommandEntities<K>( command, Properties );
		}
		catch ( Exception e )
		{
			throw new Exception( $"Error getting entities: {e.Message}", e );
		}
	}

	public void Insert( params T[] entities )
	{
		if ( entities.Length == 0 )
		{
			return;
		}

		try
		{
			var propNames = string.Join( ", ", Properties.Select( p => p.ColumnName ) );
			var propParams = string.Join( ", ", Properties.Select( p => $"@{p.ColumnName}" ) );

			UnitOfWork.ExecuteCommandForEach( $"INSERT INTO {TableName}({propNames}) VALUES ({propParams});", Properties, entities );
		}
		catch ( Exception e )
		{
			throw new Exception( $"Error inserting: {e.Message}", e );
		}
	}

	public void Update( params T[] entities )
	{
		if ( entities.Length == 0 )
		{
			return;
		}

		try
		{
			var valuesToSet = string.Join( ", ", Properties.Where( p => p.IsPrimaryKey == false ).Select( p => $"{p.ColumnName} = @{p.ColumnName}" ) );
			var pkConditions = string.Join( " AND ", Properties.Where( p => p.IsPrimaryKey ).Select( p => $"{p.ColumnName} = @{p.ColumnName}" ) );
			UnitOfWork.ExecuteCommandForEach( $"UPDATE {TableName} SET {valuesToSet} WHERE {pkConditions};", Properties, entities );
		}
		catch ( Exception e )
		{
			throw new Exception( $"Error updating: {e.Message}", e );
		}
	}
}
