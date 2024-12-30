namespace RiskConsult.Performance.Providers;

public class WeightedReturnProvider( IDateWeightProvider weightProvider, IDateReturnPercentProvider returnProvider ) : IDateWeightedReturnProvider
{
	public double GetWeightedReturn( DateTime date ) => weightProvider.GetWeight( date ) * returnProvider.GetReturnPercent( date );
}
