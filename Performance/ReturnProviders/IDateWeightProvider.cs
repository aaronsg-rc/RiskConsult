namespace RiskConsult.Performance.ReturnProviders;

public interface IDateWeightProvider
{
	double GetWeight( DateTime date );
}
