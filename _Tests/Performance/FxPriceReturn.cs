using RiskConsult.Core;
using RiskConsult.Enumerators;
using System.Text;

namespace RiskConsult._Tests.Performance;

public class FxPriceReturn : IReturnData, IPrintableReport
{
	public DateTime FinalDate => PriceReturn.FinalDate;
	public double FinalValue { get; }
	public CurrencyId FxCurrency => FxReturn.ToCurrency;
	public FxReturn FxReturn { get; }
	public DateTime InitialDate => PriceReturn.InitialDate;
	public double InitialValue { get; }
	public PriceReturn PriceReturn { get; }
	public double Return { get; }
	public PriceSourceId SourceID => PriceReturn.SourceID;
	public IHoldingTerms Terms => PriceReturn.Terms;

	public FxPriceReturn( PriceReturn priceReturn, FxReturn fxReturn )
	{
		PriceReturn = priceReturn;
		FxReturn = fxReturn;
		InitialValue = fxReturn.InitialValue * priceReturn.InitialValue;
		FinalValue = fxReturn.FinalValue * priceReturn.FinalValue;
		Return = InitialValue == 0 || FinalValue == 0 ? 0 : ( FinalValue / InitialValue ) - 1;
	}

	public static string GetReportHeaders()
	{
		return
			$"----- Fx Price Return Report-----\n" +
			$"Description,Price Currency,Fx Currency," +
			$"Date_0,Date_1," +
			$"FxPrice_0,FxPrice_1,FxPrice Return [bps],FxPrice Return [$]," +
			$"Price_0,Price_1,Price Return [bps],Price Return [$]," +
			$"Fx_0,Fx_1,Fx Return [bps],Fx Return [$]";
	}

	public string GetReportLine()
	{
		var holdingData = $"{Terms.Description},{Terms.CurrencyId},{FxCurrency}";
		var dateRange = $"{InitialDate.ToShortDateString()},{FinalDate.ToShortDateString()}";
		var fxPriceReturn = $"{InitialValue},{FinalValue},{Return * 10000},{FinalValue - InitialValue}";
		var priceReturn = $"{PriceReturn.InitialValue},{PriceReturn.FinalValue},{PriceReturn.Return * 10000},{( PriceReturn.FinalValue - PriceReturn.InitialValue ) * FxReturn.FinalValue}";
		var fxReturn = $"{FxReturn.InitialValue},{FxReturn.FinalValue},{FxReturn.Return * 10000},{( FxReturn.FinalValue - FxReturn.InitialValue ) * PriceReturn.InitialValue}";

		return $"{holdingData},{dateRange},{fxPriceReturn},{priceReturn},{fxReturn}";
	}

	public void PrintReport( string filePath )
	{
		StringBuilder text = new StringBuilder()
			.AppendLine( GetReportHeaders() )
			.AppendLine( GetReportLine() );
		File.WriteAllText( filePath, text.ToString() );
	}

	public override string ToString() => $"{FinalDate.ToShortDateString()}|{Return * 10000:F2} bps]|{Terms.Description}";
}
