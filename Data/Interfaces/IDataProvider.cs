namespace RiskConsult.Data.Interfaces;

public interface IDataProvider<T>
{
	T GetData();
}
