using RiskConsult.Performance.Providers;

namespace RiskConsult.Performance;

public class WeightProvider( IDateValueProvider portion, IDateValueProvider total ) : IDateWeightProvider
{
	public double GetWeight( DateTime date ) => portion.GetValue( date ) / total.GetValue( date );
}
