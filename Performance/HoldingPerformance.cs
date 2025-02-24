using RiskConsult.Core;
using RiskConsult.Enumerators;
using RiskConsult.Performance.ReturnProviders;

namespace RiskConsult.Performance;

public class HoldingPerformance( ReturnProviderByFxPrice providerByFxPrice, ReturnProviderByPortfolio providerByPortfolio )
{
	public readonly Performance FxContribution = new();
	public readonly Performance PriceContribution = new();
	public readonly Performance TotalReturns = new();
	private readonly HoldingProvider _holdingProvider = new( providerByFxPrice, providerByPortfolio );
	public CurrencyId FxCurrency => providerByFxPrice.FxCurrency;
	public IHoldingTerms Holding => providerByFxPrice.Holding;
	public PriceSourceId SourceID => providerByFxPrice.SourceID;

	public void Calculate( IEnumerable<DateTime> period )
	{
		var contributions = new IReturnProvider[] { providerByFxPrice.PriceReturnProvider, providerByFxPrice.FxReturnProvider };
		var priceAttribution = new ReturnProviderByAttribution( providerByFxPrice.PriceReturnProvider, _holdingProvider, contributions );
		var fxAttribution = new ReturnProviderByAttribution( providerByFxPrice.FxReturnProvider, _holdingProvider, contributions );
		TotalReturns.Calculate( period, providerByFxPrice );
		PriceContribution.Calculate( period, priceAttribution );
		FxContribution.Calculate( period, fxAttribution );
	}
}
