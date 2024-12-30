using System.Data;

namespace RiskConsult.Data.Interfaces;

public interface IUnitOfWork : IDisposable
{
	void BeginTransaction();

	void Commit();

	IDbCommand CreateCommand();

	T[] GetCommandEntities<T>( IDbCommand command, IEnumerable<IPropertyMap> properties ) where T : new();

	T? GetCommandEntity<T>( IDbCommand command, IEnumerable<IPropertyMap> properties ) where T : new();

	void ExecuteNonQuery( IDbCommand command );

	void ExecuteCommandForEach<T>( string query, IEnumerable<IPropertyMap> properties, params T[] entities );

	void Rollback();
}
