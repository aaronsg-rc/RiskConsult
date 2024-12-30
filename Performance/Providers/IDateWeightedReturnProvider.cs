namespace RiskConsult.Performance.Providers;

public interface IDateWeightedReturnProvider
{
	double GetWeightedReturn( DateTime date );
}
