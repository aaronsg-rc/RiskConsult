namespace RiskConsult.Data.Interfaces;

public interface ICsvRepository<TEntity> : IRepository<TEntity> where TEntity : new()
{
}
