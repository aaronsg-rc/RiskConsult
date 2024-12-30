using RiskConsult.Data.Interfaces;
using System.Data;

namespace RiskConsult.Data;

public class UnitOfWork( IDbConnection connection ) : IUnitOfWork
{
	private readonly IDbConnection _connection = connection;
	private IDbTransaction? _transaction;
	public bool IsWorking { get; private set; }

	public void BeginTransaction() => _transaction ??= GetConnection().BeginTransaction();

	public void Commit()
	{
		try
		{
			_transaction?.Commit();
		}
		catch ( Exception )
		{
			_transaction?.Rollback();
			throw;
		}
		finally
		{
			Cleanup();
		}
	}

	public IDbCommand CreateCommand() => _connection.CreateCommand();

	public void Dispose()
	{
		_transaction?.Dispose();
		if ( _connection.State != ConnectionState.Closed )
		{
			_connection.Close();
		}

		_connection.Dispose();
		GC.SuppressFinalize( this );
	}

	public void ExecuteCommandForEach<T>( string query, IEnumerable<IPropertyMap> properties, params T[] entities )
	{
		var alreadyWorking = IsWorking;
		if ( !IsWorking )
		{
			BeginTransaction();
		}

		using IDbCommand command = _connection.CreateCommand();
		command.CommandText = query;
		command.Transaction = _transaction;

		try
		{
			foreach ( T? entity in entities )
			{
				command.Parameters.Clear();
				foreach ( IPropertyMap prop in properties )
				{
					var value = prop.PropertyInfo.GetValue( entity ) ?? DBNull.Value;
					IDbDataParameter parameter = command.CreateParameter();
					parameter.ParameterName = $"@{prop.ColumnName}";
					parameter.Value = value;
					command.Parameters.Add( parameter );
				}

				command.ExecuteNonQuery();
			}

			if ( !alreadyWorking )
			{
				Commit();
			}
		}
		catch ( Exception )
		{
			Rollback();
			throw;
		}
	}

	public void ExecuteNonQuery( IDbCommand command )
	{
		var alreadyWorking = IsWorking;
		if ( !IsWorking )
		{
			BeginTransaction();
		}

		try
		{
			command.Transaction = _transaction;
			command.ExecuteNonQuery();

			if ( !alreadyWorking )
			{
				Commit();
			}
		}
		catch ( Exception )
		{
			Rollback();
			throw;
		}
	}

	public T[] GetCommandEntities<T>( IDbCommand command, IEnumerable<IPropertyMap> properties ) where T : new()
	{
		var entities = new List<T>();
		try
		{
			GetConnection();
			using IDataReader reader = command.ExecuteReader();
			while ( reader.Read() )
			{
				T entity = new();
				foreach ( IPropertyMap prop in properties )
				{
					var value = reader[ prop.ColumnName ];
					if ( value != null )
					{
						prop.PropertyInfo.SetValue( entity, value == DBNull.Value ? default : value );
					}
				}

				entities.Add( entity );
			}

			return [ .. entities ];
		}
		catch ( Exception )
		{
			throw;
		}
		finally
		{
			Cleanup();
		}
	}

	public T? GetCommandEntity<T>( IDbCommand command, IEnumerable<IPropertyMap> properties ) where T : new()
	{
		return GetCommandEntities<T>( command, properties ).FirstOrDefault();
	}

	public void Rollback()
	{
		try
		{
			_transaction?.Rollback();
		}
		finally
		{
			Cleanup();
		}
	}

	private void Cleanup()
	{
		_connection?.Close();
		_transaction?.Dispose();
		_transaction = null;
		IsWorking = false;
	}

	private IDbConnection GetConnection()
	{
		if ( _connection.State != ConnectionState.Open )
		{
			_connection.Open();
		}

		IsWorking = true;
		return _connection;
	}
}
