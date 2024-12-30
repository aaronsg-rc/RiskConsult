namespace RiskConsult._Tests;

public interface IDataConverter<TInput, TOutput>
{
	public TOutput ConvertData( TInput inputs );
}
