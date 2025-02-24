using RiskConsult.Performance.ReturnProviders;

namespace RiskConsult.Performance;

public class HoldingProvider( ReturnProviderByFxPrice providerByFxPrice, ReturnProviderByPortfolio providerByPortfolio ) :
	ReturnProvider, IDateAmountProvider, IDateWeightProvider
{
	public double GetAmount( DateTime date ) => providerByPortfolio.GetComposition( date ).FirstOrDefault( e => e.HoldingId.Equals( providerByFxPrice.Holding.HoldingId ) )?.Amount ?? 0;

	public double GetWeight( DateTime date ) => GetValue( date ) / providerByPortfolio.GetValue( date );

	public double GetWeightedReturn( DateTime date ) => GetWeight( date ) * GetReturnPercent( date );

	protected override IReturnData CalculateReturn( DateTime date )
	{
		IReturnData holdingReturn = providerByFxPrice.GetReturn( date );
		var amount = GetAmount( date );
		return new ReturnData( date,
			holdingReturn.InitialValue * amount,
			holdingReturn.FinalValue * amount );
	}
}
