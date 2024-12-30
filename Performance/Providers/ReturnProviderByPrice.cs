using RiskConsult.Core;
using RiskConsult.Enumerators;
using RiskConsult.Extensions;

namespace RiskConsult.Performance.Providers;

public class ReturnProviderByPrice : ReturnProvider
{
	public IHoldingTerms Holding;
	public PriceSourceId SourceID;

	private static readonly Dictionary<(int, PriceSourceId), ReturnProviderByPrice> _providers = [];

	private ReturnProviderByPrice( IHoldingTerms holding, PriceSourceId SourceId )
	{
		Holding = holding;
		SourceID = SourceId;
	}

	public static ReturnProviderByPrice GetProvider( IHoldingTerms holding, PriceSourceId sourceId )
	{
		if ( _providers.TryGetValue( (holding.HoldingId, sourceId), out ReturnProviderByPrice? provider ) )
		{
			return provider;
		}

		provider = new ReturnProviderByPrice( holding, sourceId );
		_providers[ (holding.HoldingId, sourceId) ] = provider;

		return provider;
	}

	protected override IReturnData CalculateReturn( DateTime date )
	{
		DateTime prevDate = date.GetBusinessDateAdd( DateUnit.Day, -1 );
		return new ReturnData( date,
			Holding.GetCleanPrice( prevDate, SourceID ),
			Holding.GetDirtyPrice( date, SourceID, out _, out _, out _ ) );
	}
}
