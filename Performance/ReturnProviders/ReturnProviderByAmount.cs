namespace RiskConsult.Performance.ReturnProviders;

public class ReturnProviderByAmount( IReturnProvider returnProvider, IDateAmountProvider amountProvider ) : ReturnProvider
{
	protected override IReturnData CalculateReturn( DateTime date )
	{
		IReturnData holdingReturn = returnProvider.GetReturn( date );
		var amount = amountProvider.GetAmount( date );
		var initialValue = double.IsNaN( holdingReturn.InitialValue ) ? 0 : holdingReturn.InitialValue;
		var finalValue = double.IsNaN( holdingReturn.FinalValue ) ? 0 : holdingReturn.FinalValue;

		return new ReturnData( date, initialValue * amount, finalValue * amount );
	}
}
