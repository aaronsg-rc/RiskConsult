namespace RiskConsult.Data.Interfaces;

public interface IRepository<TEntity>
{
	void Delete( params TEntity[] entities );

	K[] GetAll<K>() where K : TEntity, new();

	void Insert( params TEntity[] entities );

	void Update( params TEntity[] entities );
}
