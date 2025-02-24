using RiskConsult.Reporting;

namespace RiskConsult.Performance;

public interface IReturnData : IReportModel
{
	/// <summary> Fecha en que se presenta el rendimiento </summary>
	DateTime Date { get; }

	/// <summary> Valor final </summary>
	double FinalValue { get; }

	/// <summary> Valor inicial </summary>
	double InitialValue { get; }

	/// <summary> Rendimiento porcentual entre el valor inicial y el final </summary>
	double ReturnPercent { get; }

	/// <summary> Rendimiento originado por el cambio de valor (unidades del valor) </summary>
	double ReturnValue { get; }
}

public class ReturnData : IReturnData
{
	public DateTime Date { get; set; }

	public double FinalValue { get; set; }

	public double InitialValue { get; set; }

	public double ReturnPercent { get; set; }

	public double ReturnValue { get; set; }

	public ReturnData()
	{ }

	public ReturnData( DateTime date, double initialValue, double finalValue )
	{
		Date = date;
		InitialValue = initialValue;
		FinalValue = finalValue;

		if ( initialValue == 0 || finalValue == 0 )
		{
			ReturnPercent = double.NaN;
			ReturnValue = 0;
		}
		else
		{
			ReturnPercent = ( finalValue / initialValue ) - 1;
			ReturnValue = finalValue - initialValue;
		}
	}

	public ReturnData( DateTime date, double initialValue, double finalValue, double returnPercent, double returnValue )
	{
		Date = date;
		InitialValue = initialValue;
		FinalValue = finalValue;
		ReturnPercent = returnPercent;
		ReturnValue = returnValue;
	}

	public string GetHeaders() => string.Join( ',', nameof( Date ), nameof( InitialValue ), nameof( FinalValue ), "Return %", "Return $" );

	public string GetLine() => string.Join( ',', Date, InitialValue, FinalValue, ReturnPercent, ReturnValue );

	public override string ToString() => $"{ReturnPercent * 10000:F2} bps | {ReturnValue:F2}";
}
