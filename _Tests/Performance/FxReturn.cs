using RiskConsult.Enumerators;
using RiskConsult.Extensions;
using System.Text;

namespace RiskConsult._Tests.Performance;

public class FxReturn : IReturnData, IPrintableReport
{
	private static readonly Dictionary<(DateTime, CurrencyId, CurrencyId), FxReturn> _cache = [];
	public DateTime FinalDate { get; }
	public double FinalValue { get; }
	public CurrencyId FromCurrency { get; }
	public DateTime InitialDate { get; }
	public double InitialValue { get; }
	public double Return { get; }
	public CurrencyId ToCurrency { get; }

	private FxReturn( DateTime date, CurrencyId fromCurrency, CurrencyId toCurrency )
	{
		FromCurrency = fromCurrency;
		ToCurrency = toCurrency;
		InitialDate = date.GetBusinessDateAdd( DateUnit.Day, -1 );
		InitialValue = fromCurrency.ConvertToCurrency( toCurrency, InitialDate );
		FinalDate = date;
		FinalValue = fromCurrency.ConvertToCurrency( toCurrency, date );
		Return = InitialValue == 0 || FinalValue == 0 ? 0 : ( FinalValue / InitialValue ) - 1;
	}

	public static FxReturn GetFxReturn( DateTime date, CurrencyId fromCurrency, CurrencyId toCurrency )
	{
		if ( _cache.TryGetValue( (date, fromCurrency, toCurrency), out FxReturn? dateReturn ) )
		{
			return dateReturn;
		}

		dateReturn = new FxReturn( date, fromCurrency, toCurrency );

		return _cache[ (date, fromCurrency, toCurrency) ] = dateReturn;
	}

	public static string GetReportHeaders()
	{
		return
			$"----- Fx Return Report-----\n" +
			$"Base,Quote," +
			$"Date_0,Date_1," +
			$"Fx_0,Fx_1,Fx Return [bps],Fx Return [$]";
	}

	public string GetReportLine()
	{
		var baseQuote = $"{ToCurrency},{FromCurrency}";
		var dateRange = $"{InitialDate.ToShortDateString()},{FinalDate.ToShortDateString()}";
		var fxReturn = $"{InitialValue},{FinalValue},{Return * 10000},{FinalValue - InitialValue}";

		return $"{baseQuote},{dateRange},{fxReturn}";
	}

	public void PrintReport( string filePath )
	{
		StringBuilder text = new StringBuilder()
			.AppendLine( GetReportHeaders() )
			.AppendLine( GetReportLine() );
		File.WriteAllText( filePath, text.ToString() );
	}

	/// <summary> Rendimiento de la fecha y nombre del instrumento </summary>
	public override string ToString() => $"{FinalDate.ToShortDateString()}|{Return * 10000:F2} bps]|{ToCurrency}/{FromCurrency}";
}
