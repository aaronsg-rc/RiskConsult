using RiskConsult.Core;
using RiskConsult.Data.Interfaces;
using RiskConsult.Enumerators;

namespace RiskConsult.Extensions;

public static class HoldingExtensions
{
	public static IHolding GetHolding( this IHoldingIdProperty holdingId, double amount = 0 )
	{
		return holdingId.GetHoldingTerms().GetHolding( amount );
	}

	public static IHolding GetHolding( this IHoldingIdProperty holdingId, DateTime date, PriceSourceId sourceId, CurrencyId fxCurrency, double amount = 0 )
	{
		return holdingId.GetHoldingTerms().GetHolding( date, sourceId, fxCurrency, amount );
	}

	public static IHolding GetHolding( this IHoldingTerms terms, double amount = 0 )
	{
		return new Holding
		{
			Terms = terms,
			HoldingId = terms.HoldingId,
			CurrencyId = terms.CurrencyId,
			Amount = amount
		};
	}

	public static IHolding GetHolding( this IHoldingTerms terms, DateTime date, PriceSourceId sourceId, CurrencyId fxCurrency, double amount = 0 )
	{
		var holding = new Holding
		{
			Terms = terms,
			HoldingId = terms.HoldingId,
			CurrencyId = terms.CurrencyId,
			Amount = amount
		};

		holding.LoadPrice( date, sourceId, fxCurrency );

		return holding;
	}
}
