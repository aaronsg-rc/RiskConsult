using RiskConsult.Core;
using RiskConsult.Data.Repositories;
using RiskConsult.Enumerators;
using RiskConsult.Extensions;

namespace RiskConsult.Data.Services;

public interface IFloaterResetService
{
	double GetFloaterReset( int holdingId, DateTime date );

	Dictionary<DateTime, double> GetFloaterResetCalendar( int holdingId );

	public double GetPayoutByCoupon( IHoldingTerms tycs, DateTime date );
}

internal class FloaterResetService( IFloaterResetRepository floaterResetRepository ) : IFloaterResetService
{
	private readonly Dictionary<int, Dictionary<DateTime, double>> _cache = [];

	/// <summary> Limpia datos almacenados en cache </summary>
	public void ClearCache() => _cache.Clear();

	/// <summary> Obtiene la entidad para el ID y fecha solicitado </summary>
	/// <param name="holdingId"> ID del instrumento </param>
	/// <param name="date"> Fecha de la entidad </param>
	/// <returns> Entidad encontrada o nulo si no existe </returns>
	public double GetFloaterReset( int holdingId, DateTime date )
	{
		return GetFloaterResetCalendar( holdingId ).GetValueOrDefault( date );
	}

	public Dictionary<DateTime, double> GetFloaterResetCalendar( int holdingId )
	{
		if ( _cache.TryGetValue( holdingId, out Dictionary<DateTime, double>? calendar ) )
		{
			return calendar;
		}

		return _cache[ holdingId ] = floaterResetRepository
			.GetFloaterResetEntities( holdingId )
			.ToDictionary( entity => entity.Date, entity => entity.Value );
	}

	public double GetPayoutByCoupon( IHoldingTerms tycs, DateTime date )
	{
		if ( tycs.HoldingId >= 2000000 || ( tycs.ModuleId != ModuleId.Fixed && tycs.ModuleId != ModuleId.Float ) )
		{
			return 0;
		}

		var coupon = DbZeus.Db.FloaterResets.GetFloaterReset( tycs.HoldingId, date );
		if ( coupon == 0 && tycs.ModuleId == ModuleId.Fixed )
		{
			coupon = tycs.GetPaymentCalendar( date ).Contains( date ) ? tycs.CouponRate : 0;
		}

		if ( coupon == 0 )
		{
			return 0;
		}

		// Último cupón que se pago
		var days = tycs.PeriodId is PeriodId.Daily ? 1 : tycs.PeriodId is PeriodId.Weekly ? 7 : 30;
		DateTime dteLastPay = date.GetBusinessDateAdd( DateUnit.Day, -tycs.PayFrequency * days );
		var intDays = date.Subtract( dteLastPay ).Days;

		// Obtengo la amorización acumulada
		var currentNominal = tycs.GetAmortizedNominalAt( date );
		return currentNominal * coupon * intDays / 360;
	}
}
