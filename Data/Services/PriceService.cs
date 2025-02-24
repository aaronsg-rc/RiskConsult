using RiskConsult.Data.Entities;
using RiskConsult.Data.Interfaces;
using RiskConsult.Data.Repositories;
using RiskConsult.Enumerators;

namespace RiskConsult.Data.Services;

public interface IPriceService : ICachedService
{
	/// <summary> Obtiene el precio de un instrumento para una fecha y tipo de precio dado </summary>
	/// <param name="holdingId"> ID del instrumento del que se requiere el precio </param>
	/// <param name="date"> Fecha del precio </param>
	/// <param name="sourceId"> Tipo de precio </param>
	/// <returns> El precio del instrumento o 0 si no existe </returns>
	double GetPrice( DateTime date, PriceSourceId sourceId, int holdingId );

	/// <summary> Obtiene una lista de entidades de precio para una serie de fechas </summary>
	/// <param name="holdingId"> ID del instrumento </param>
	/// <param name="sourceId"> Tipo de precio </param>
	/// <returns> Lista de entidades para el rango de fechas </returns>
	List<double> GetPrices( IEnumerable<DateTime> period, PriceSourceId sourceId, int holdingId );
}

internal class PriceService( IPriceRepository priceRepository ) : IPriceService
{
	private readonly Dictionary<(DateTime, PriceSourceId), Dictionary<int, double>> _cache = [];

	public void ClearCache() => _cache.Clear();

	public double GetPrice( DateTime date, PriceSourceId sourceId, int holdingId )
	{
		Dictionary<int, double> datePrices = GetDatePricesDictionary( date, sourceId );
		return datePrices.TryGetValue( holdingId, out var price ) ? price : 0;
	}

	public List<double> GetPrices( IEnumerable<DateTime> period, PriceSourceId sourceId, int holdingId )
	{
		var prices = new List<double>();
		foreach ( DateTime date in period )
		{
			var price = GetPrice( date, sourceId, holdingId );
			prices.Add( price );
		}

		return prices;
	}

	private Dictionary<int, double> GetDatePricesDictionary( DateTime date, PriceSourceId sourceId )
	{
		if ( _cache.TryGetValue( (date, sourceId), out Dictionary<int, double>? datePrices ) )
		{
			return datePrices;
		}

		IPriceEntity[] priceEntities = priceRepository.GetPriceEntities( date, ( int ) sourceId );
		datePrices = priceEntities.ToDictionary( entity => entity.HoldingId, entity => entity.Value );

		return _cache[ (date, sourceId) ] = datePrices;
	}
}
