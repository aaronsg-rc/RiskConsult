namespace RiskConsult.Data.Interfaces;

public interface ITableMap : IEntityMap
{
	string TableName { get; }
}
