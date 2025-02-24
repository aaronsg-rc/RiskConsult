using RiskConsult.Core;
using RiskConsult.Data;
using RiskConsult.Data.Interfaces;
using RiskConsult.Enumerators;

namespace RiskConsult.Extensions;

/// <summary> Obtiene el precio de un instrumento para una fecha y tipo de precio dado </summary>
/// <param name="holding"> Instrumento del que se requiere el precio </param>
/// <param name="date"> Fecha del precio </param>
/// <param name="priceID"> Tipo de precio </param>
/// <returns> El precio del instrumento o 0 si no es valido </returns>
public static class PriceExtensions
{
	/// <summary> Obtiene el precio limpio de un instrumento para una fecha y tipo de precio dado </summary>
	/// <param name="holdingId"> Instrumento del que se requiere el precio </param>
	/// <param name="date"> Fecha del precio </param>
	/// <param name="priceSourceId"> Tipo de precio </param>
	/// <returns> El precio del instrumento o 0 si no es valido </returns>
	public static double GetCleanPrice( this IHoldingIdProperty holdingId, DateTime date, PriceSourceId priceSourceId )
	{
		var price = 0.0;
		var terms = holdingId as IHoldingTerms ?? holdingId.GetHoldingTerms();

		if ( date < terms.Issue || date > terms.Maturity )
		{
			return 0;
		}

		price = terms.TypeId == TypeId.Cash ? 1 : GetDbPrice( terms.HoldingId, date, priceSourceId );
		if ( price is 0 )
		{
			if ( terms.ClassId is ClassId.Acciones or ClassId.Fondos or ClassId.Derivados || ( int ) terms.ClassId == 101 )
			{
				return 0;
			}

			if ( terms.Maturity == date && terms.Nominal != 0 )
			{
				price = terms.Nominal;
			}
			else
			{
				price = terms.GetPresentValue( date );
			}
		}

		if ( terms.CurrencyId == CurrencyId.UDI )
		{
			price /= CurrencyId.UDI.ConvertToCurrency( CurrencyId.MXN, date );
		}

		return price;
	}

	public static double GetDirtyPrice( this IHoldingIdProperty holdingId, DateTime date, PriceSourceId priceID,
		out double amortizationPayout, out double dividendPayout, out double couponPayout )
	{
		IHoldingTerms terms = holdingId as IHoldingTerms ?? holdingId.GetHoldingTerms();
		var cleanPrice = terms.GetCleanPrice( date, priceID );
		amortizationPayout = terms.GetPayoutByAmortization( date );
		dividendPayout = terms.GetPayoutByEvents( date, cleanPrice );
		couponPayout = terms.GetPayoutByCoupon( date );

		return cleanPrice + amortizationPayout + dividendPayout + couponPayout;
	}

	public static double GetMarketPrice( this IHoldingIdProperty holdingId, DateTime date, PriceSourceId priceID )
	{
		IHoldingTerms terms = holdingId as IHoldingTerms ?? holdingId.GetHoldingTerms();
		if ( terms.TypeId == TypeId.Cash )
		{
			return 1;
		}
		else if ( date < terms.Issue || date > terms.Maturity )
		{
			return 0;
		}

		var price = GetDbPrice( terms.HoldingId, date, priceID );
		if ( price == 0 )
		{
			if ( terms.ClassId is ClassId.Acciones or ClassId.Fondos or ClassId.Derivados || ( int ) terms.ClassId == 101 )
			{
				return 0;
			}

			price = terms.GetPresentValue( date );
		}

		if ( terms.CurrencyId == CurrencyId.UDI )
		{
			price /= CurrencyId.UDI.ConvertToCurrency( CurrencyId.MXN, date );
		}

		return price;
	}

	public static void LoadPrice( this IHolding holding, DateTime date, PriceSourceId sourceID = PriceSourceId.PiP_MD, CurrencyId fxCurrency = CurrencyId.Invalid )
	{
		fxCurrency = fxCurrency == CurrencyId.Invalid ? holding.CurrencyId : fxCurrency;
		holding.Price = holding.CurrencyId.ConvertToCurrency( fxCurrency, date, holding.Terms.GetCleanPrice( date, sourceID ) );
		holding.Date = date;
		holding.PriceSourceId = sourceID;
		holding.Value = holding.Price * holding.Amount;
	}

	public static void LoadPrices( this IPortfolio portfolio, DateTime date, PriceSourceId sourceId = PriceSourceId.PiP_MD, CurrencyId fxCurrency = CurrencyId.Invalid )
	{
		portfolio.Date = date;
		portfolio.PriceSourceId = sourceId;
		portfolio.CurrencyId = fxCurrency;
		foreach ( IHolding holding in portfolio.Holdings )
		{
			holding.LoadPrice( date, sourceId, fxCurrency );
		}
	}

	private static double GetDbPrice( int holdingId, DateTime date, PriceSourceId priceID )
	{
		if ( holdingId >= 2000000 || priceID is PriceSourceId.Valmer_MD or PriceSourceId.Valmer_24h or PriceSourceId.User )
		{
			return DbZeus.Db.User?.Prices.GetPrice( date, priceID, holdingId ) ?? 0;
		}

		return DbZeus.Db.Prices.GetPrice( date, priceID, holdingId );
	}
}
