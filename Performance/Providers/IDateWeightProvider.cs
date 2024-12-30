namespace RiskConsult.Performance.Providers;

public interface IDateWeightProvider
{
	double GetWeight( DateTime date );
}
