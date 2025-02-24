namespace RiskConsult.Performance.ReturnProviders;

public class WeightedReturnProvider( IDateWeightProvider weightProvider, IDateReturnPercentProvider returnProvider ) : IDateWeightedReturnProvider
{
	public double GetWeightedReturn( DateTime date ) => weightProvider.GetWeight( date ) * returnProvider.GetReturnPercent( date );
}
