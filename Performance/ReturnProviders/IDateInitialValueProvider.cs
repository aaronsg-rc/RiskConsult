namespace RiskConsult.Performance.ReturnProviders;

public interface IDateInitialValueProvider
{
	/// <summary> Valor inicial en la fecha proporcionada </summary>
	double GetInitialValue( DateTime date );
}
