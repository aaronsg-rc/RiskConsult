using RiskConsult.Data.Repositories;
using RiskConsult.Enumerators;

namespace RiskConsult.Data.Services;

public interface IPriceService
{
	/// <summary> Obtiene el precio de un instrumento para una fecha y tipo de precio dado </summary>
	/// <param name="holdingId"> ID del instrumento del que se requiere el precio </param>
	/// <param name="date"> Fecha del precio </param>
	/// <param name="sourceId"> Tipo de precio </param>
	/// <returns> El precio del instrumento o <see langword="double" />.NaN si no es valido </returns>
	double GetPrice( DateTime date, PriceSourceId sourceId, int holdingId );

	/// <summary> Obtiene una lista de entidades de precio para una serie de fechas </summary>
	/// <param name="holdingId"> ID del instrumento </param>
	/// <param name="sourceId"> Tipo de precio </param>
	/// <returns> Lista de entidades para el rango de fechas </returns>
	IEnumerable<double> GetPrices( IEnumerable<DateTime> period, PriceSourceId sourceId, int holdingId );
}

internal class PriceService( IPriceRepository priceRepository ) : IPriceService
{
	private readonly Dictionary<(DateTime, PriceSourceId), Dictionary<int, double>> _cache = [];

	/// <summary> Limpia datos almacenados en el cache </summary>
	public void ClearCache() => _cache.Clear();

	public double GetPrice( DateTime date, PriceSourceId sourceId, int holdingId )
	{
		return GetDatePrices( date, sourceId ).TryGetValue( holdingId, out var price ) ? price : double.NaN;
	}

	public IEnumerable<double> GetPrices( IEnumerable<DateTime> period, PriceSourceId sourceId, int holdingId )
	{
		foreach ( DateTime date in period )
		{
			yield return GetPrice( date, sourceId, holdingId );
		}
	}

	private Dictionary<int, double> GetDatePrices( DateTime date, PriceSourceId sourceId )
	{
		if ( _cache.TryGetValue( (date, sourceId), out Dictionary<int, double>? datePrices ) )
		{
			return datePrices;
		}

		return _cache[ (date, sourceId) ] = priceRepository
			.GetPriceEntities( date, ( int ) sourceId )
			.ToDictionary( entity => entity.HoldingId, entity => entity.Value );
	}
}
