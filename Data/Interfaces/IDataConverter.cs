namespace RiskConsult.Data.Interfaces;

public interface IDataConverter<TSource, TDestiny>
{
	TDestiny ConvertData( TSource source );
}
