using RiskConsult.Data.Repositories;

namespace RiskConsult.Data.Services;

public interface IAmortizationService
{
	Dictionary<DateTime, double> GetAmortizationCalendar( int holdingId );

	double GetAmortizationPercent( int holdingId, DateTime date );
}

internal class AmortizationService( IAmortizationRepository amortizationRepository ) : IAmortizationService
{
	private readonly Dictionary<int, Dictionary<DateTime, double>> _cache = [];

	public void ClearCache()
	{
		_cache.Clear();
	}

	public Dictionary<DateTime, double> GetAmortizationCalendar( int holdingId )
	{
		if ( _cache.TryGetValue( holdingId, out Dictionary<DateTime, double>? calendar ) )
		{
			return calendar;
		}

		return _cache[ holdingId ] = amortizationRepository
			.GetAmortizationEntities( holdingId )
			.ToDictionary( e => e.Date, e => e.Value );
	}

	/// <summary> Obtiene la entidad de amortizacion para el id y fecha dada </summary>
	/// <returns> Entidad encontrada o nulo si no existe </returns>
	public double GetAmortizationPercent( int holdingId, DateTime date )
	{
		return GetAmortizationCalendar( holdingId ).GetValueOrDefault( date );
	}
}
