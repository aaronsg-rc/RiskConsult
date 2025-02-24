namespace RiskConsult.Performance.ReturnProviders;

public interface IDateAmountProvider
{
	/// <summary> Cantidad en la fecha proporcionada </summary>
	double GetAmount( DateTime date );
}
