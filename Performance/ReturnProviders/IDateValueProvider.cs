namespace RiskConsult.Performance.ReturnProviders;

public interface IDateValueProvider
{
	/// <summary> Valor en la fecha proporcionada </summary>
	double GetValue( DateTime date );
}
