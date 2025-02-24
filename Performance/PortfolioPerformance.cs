using RiskConsult.Core;
using RiskConsult.Enumerators;
using RiskConsult.Performance.ReturnProviders;

namespace RiskConsult.Performance;

public class PortfolioPerformance( ReturnProviderByPortfolio providerByPortfolio )
{
	public List<HoldingPerformance> Holdings = [];
	public Performance Performance = new();
	public CurrencyId FxCurrency => providerByPortfolio.FxCurrency;
	public string Name => providerByPortfolio.Name;
	public PriceSourceId SourceID => providerByPortfolio.SourceID;

	public void Calculate( IEnumerable<DateTime> period )
	{
		Performance.Calculate( period, providerByPortfolio );

		IHoldingTerms[] holdings = period
			.SelectMany( providerByPortfolio.GetComposition )
			.DistinctBy( e => e.HoldingId )
			.Select( e => e.Terms )
			.ToArray();

		foreach ( IHoldingTerms hold in holdings )
		{
			var provFxPrice = ReturnProviderByFxPrice.GetProvider( hold, FxCurrency, SourceID );
			var holdPerfat = new HoldingPerformance( provFxPrice, providerByPortfolio );
			holdPerfat.Calculate( period );

			Holdings.Add( holdPerfat );
		}
	}
}
