using System.Data;

namespace RiskConsult.Data;

public interface IUnitOfWork : IDisposable
{
	void BeginTransaction();

	void Commit();

	IDbCommand CreateCommand();

	void Rollback();
}

public class UnitOfWork : IUnitOfWork
{
	private readonly IDbConnection _connection;
	private IDbTransaction? _transaction;

	public UnitOfWork( IDbConnection connection )
	{
		_connection = connection ?? throw new ArgumentNullException( nameof( connection ) );
		if ( _connection.State != ConnectionState.Open )
		{
			_connection.Open();
		}
	}

	public void BeginTransaction()
	{
		_transaction = _connection.BeginTransaction();
	}

	public void Commit()
	{
		try
		{
			_transaction?.Commit();
		}
		catch
		{
			_transaction?.Rollback();
			throw;
		}
		finally
		{
			_transaction?.Dispose();
			_transaction = null;
		}
	}

	public IDbCommand CreateCommand()
	{
		IDbCommand command = _connection.CreateCommand();
		command.Transaction = _transaction;
		return command;
	}

	public void Dispose()
	{
		_transaction?.Dispose();
		_connection.Dispose();
		GC.SuppressFinalize( this );
	}

	public void Rollback()
	{
		_transaction?.Rollback();
		_transaction?.Dispose();
		_transaction = null;
	}
}
