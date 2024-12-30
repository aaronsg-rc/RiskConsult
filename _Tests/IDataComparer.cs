namespace RiskConsult._Tests;

public interface IDataComparer<T1, T2, TOut>
{
	TOut CompareData( T1 value, T2 other );
}
