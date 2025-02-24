using RiskConsult.Core;
using RiskConsult.Enumerators;

namespace RiskConsult.Performance.ReturnProviders;

public class ReturnProviderByFxPrice : ReturnProvider
{
	public readonly ReturnProviderByFx FxReturnProvider;
	public readonly ReturnProviderByPrice PriceReturnProvider;
	private static readonly Dictionary<(int, CurrencyId, PriceSourceId), ReturnProviderByFxPrice> _providers = [];

	public CurrencyId FxCurrency => FxReturnProvider.ToCurrency;
	public IHoldingTerms Holding => PriceReturnProvider.Holding;
	public PriceSourceId SourceID => PriceReturnProvider.SourceID;

	private ReturnProviderByFxPrice( ReturnProviderByPrice priceReturnProvider, ReturnProviderByFx fxReturnProvider )
	{
		FxReturnProvider = fxReturnProvider;
		PriceReturnProvider = priceReturnProvider;
	}

	public static ReturnProviderByFxPrice GetProvider( IHoldingTerms holding, CurrencyId fxCurrency, PriceSourceId sourceID )
	{
		if ( _providers.TryGetValue( (holding.HoldingId, fxCurrency, sourceID), out ReturnProviderByFxPrice? provider ) )
		{
			return provider;
		}

		var priceReturnProvider = ReturnProviderByPrice.GetProvider( holding, sourceID );
		var fxReturnProvider = ReturnProviderByFx.GetProvider( holding.CurrencyId, fxCurrency );
		provider = new ReturnProviderByFxPrice( priceReturnProvider, fxReturnProvider );

		return _providers[ (holding.HoldingId, fxCurrency, sourceID) ] = provider;
	}

	public override string ToString() => $"{PriceReturnProvider}|{FxReturnProvider}";

	protected override IReturnData CalculateReturn( DateTime date )
	{
		IReturnData priceReturn = PriceReturnProvider.GetReturn( date );
		IReturnData fxReturn = FxReturnProvider.GetReturn( date );
		return new ReturnData( date,
			priceReturn.InitialValue * fxReturn.InitialValue,
			priceReturn.FinalValue * fxReturn.FinalValue );
	}
}
