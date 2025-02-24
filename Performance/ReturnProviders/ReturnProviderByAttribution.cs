namespace RiskConsult.Performance.ReturnProviders;

public class ReturnProviderByAttribution( IReturnProvider originProvider, IReturnProvider totalProvider, IEnumerable<IReturnProvider> contributionProviders ) : ReturnProvider
{
	protected override IReturnData CalculateReturn( DateTime date )
	{
		IReturnData totalReturn = totalProvider.GetReturn( date );
		IReturnData originReturn = originProvider.GetReturn( date );
		var sumReturnContributions = contributionProviders.Sum( provider => provider.GetReturn( date ).ReturnPercent );
		var proportion = totalReturn.ReturnPercent / sumReturnContributions;

		return new ReturnData( date,
			originReturn.InitialValue,
			originReturn.FinalValue,
			originReturn.ReturnPercent,
			originReturn.ReturnPercent * totalReturn.InitialValue * proportion );
	}
}
