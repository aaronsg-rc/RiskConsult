namespace RiskConsult.Performance.Providers;

public class ReturnProviderByAmount( IReturnProvider returnProvider, IDateAmountProvider amountProvider ) : ReturnProvider
{
	protected override IReturnData CalculateReturn( DateTime date )
	{
		IReturnData holdingReturn = returnProvider.GetReturn( date );
		var amount = amountProvider.GetAmount( date );
		return new ReturnData( date,
			holdingReturn.InitialValue * amount,
			holdingReturn.FinalValue * amount );
	}
}
