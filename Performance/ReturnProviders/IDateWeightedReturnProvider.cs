namespace RiskConsult.Performance.ReturnProviders;

public interface IDateWeightedReturnProvider
{
	double GetWeightedReturn( DateTime date );
}
