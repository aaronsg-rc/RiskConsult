using RiskConsult.Core;
using RiskConsult.Data.Interfaces;
using RiskConsult.Data.Repositories;
using RiskConsult.Enumerators;
using RiskConsult.Extensions;

namespace RiskConsult.Data.Services;

public interface IAmortizationService : ICachedService
{
	Dictionary<DateTime, double> GetAmortizationCalendar( IHoldingIdProperty holdingId );

	double GetAmortizationPercent( IHoldingIdProperty holdingId, DateTime date );

	double GetAmortizedNominalAt( IHoldingIdProperty holdingId, DateTime date );

	double GetPayoutByAmortization( IHoldingIdProperty holdingId, DateTime date );
}

internal class AmortizationService : IAmortizationService
{
	private readonly IAmortizationRepository _amortizationRepository;
	private readonly Dictionary<int, Dictionary<DateTime, double>> _cache = [];

	public AmortizationService( IAmortizationRepository amortizationRepository )
	{
		_amortizationRepository = amortizationRepository ?? throw new ArgumentNullException( nameof( amortizationRepository ) );
	}

	public void ClearCache()
	{
		_cache.Clear();
	}

	public Dictionary<DateTime, double> GetAmortizationCalendar( IHoldingIdProperty holdingId )
	{
		return GetAmortizationCalendar( holdingId.HoldingId ).ToDictionary();
	}

	/// <summary> Obtiene el porcentaje de amortización dado un HoldingID y fecha </summary>
	/// <returns> Entidad encontrada o nulo si no existe </returns>
	public double GetAmortizationPercent( IHoldingIdProperty holdingId, DateTime date )
	{
		return GetAmortizationCalendar( holdingId.HoldingId ).GetValueOrDefault( date );
	}

	public double GetAmortizedNominalAt( IHoldingIdProperty holdingId, DateTime date )
	{
		IHoldingTerms terms = holdingId as IHoldingTerms ?? holdingId.GetHoldingTerms();
		if ( terms.ModuleId is ModuleId.Fixed or ModuleId.Float or ModuleId.Float2 )
		{
			Dictionary<DateTime, double> calendar = GetAmortizationCalendar( terms.HoldingId );
			if ( calendar.Count > 0 )
			{
				return terms.Nominal * ( 1 - calendar.Where( kvp => kvp.Key < date ).Sum( kvp => kvp.Value ) );
			}
		}

		return terms.Nominal;
	}

	public double GetPayoutByAmortization( IHoldingIdProperty holdingId, DateTime date )
	{
		IHoldingTerms terms = holdingId as IHoldingTerms ?? holdingId.GetHoldingTerms();
		if ( terms.ModuleId is ModuleId.Fixed or ModuleId.Float or ModuleId.Float2 )
		{
			var amortization = GetAmortizationPercent( terms, date );
			return amortization * terms.Nominal;
		}

		return 0;
	}

	private Dictionary<DateTime, double> GetAmortizationCalendar( int holdingId )
	{
		if ( _cache.TryGetValue( holdingId, out Dictionary<DateTime, double>? calendar ) )
		{
			return calendar;
		}

		return _cache[ holdingId ] = _amortizationRepository.GetAmortizationEntities( holdingId ).ToDictionary( e => e.Date, e => e.Value );
	}
}
