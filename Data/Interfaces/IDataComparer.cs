namespace RiskConsult.Data.Interfaces;

public interface IDataComparer<T, Q, TResult>
{
	public string CompareData( T value, Q other );
}