namespace RiskConsult.Performance.Providers;

public interface IDateAmountProvider
{
	/// <summary> Cantidad en la fecha proporcionada </summary>
	double GetAmount( DateTime date );
}
