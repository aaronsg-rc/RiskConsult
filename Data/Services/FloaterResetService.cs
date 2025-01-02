using RiskConsult.Core;
using RiskConsult.Data.Entities;
using RiskConsult.Data.Interfaces;
using RiskConsult.Data.Repositories;
using RiskConsult.Enumerators;
using RiskConsult.Extensions;

namespace RiskConsult.Data.Services;

public interface IFloaterResetService
{
	void ClearCache();

	double GetFloaterReset( IHoldingIdProperty holdingId, DateTime date );

	Dictionary<DateTime, double> GetFloaterResetCalendar( IHoldingIdProperty holdingId );

	double GetPayoutByCoupon( IHoldingIdProperty holdingId, DateTime date );
}

internal class FloaterResetService( IFloaterResetRepository floaterResetRepository ) : IFloaterResetService
{
	private readonly Dictionary<int, IFloaterResetEntity[]> _cache = [];

	/// <summary> Limpia datos almacenados en cache </summary>
	public void ClearCache() => _cache.Clear();

	public double GetFloaterReset( IHoldingIdProperty holdingId, DateTime date )
	{
		return GetFloaterResetEntities( holdingId.HoldingId ).FirstOrDefault( e => e.Date == date )?.Value ?? 0;
	}

	public Dictionary<DateTime, double> GetFloaterResetCalendar( IHoldingIdProperty holdingId )
	{
		return GetFloaterResetEntities( holdingId.HoldingId ).ToDictionary( entity => entity.Date, entity => entity.Value );
	}

	public double GetPayoutByCoupon( IHoldingIdProperty holdingId, DateTime date )
	{
		IHoldingTerms tycs = holdingId as IHoldingTerms ?? holdingId.GetHoldingTerms();
		if ( tycs.ModuleId is not ModuleId.Fixed and not ModuleId.Float )
		{
			return 0;
		}

		var coupon = GetFloaterReset( tycs, date );
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

	private IFloaterResetEntity[] GetFloaterResetEntities( int holdingId )
	{
		if ( _cache.TryGetValue( holdingId, out IFloaterResetEntity[]? entities ) )
		{
			return entities;
		}

		return _cache[ holdingId ] = floaterResetRepository.GetFloaterResetEntities( holdingId );
	}
}
