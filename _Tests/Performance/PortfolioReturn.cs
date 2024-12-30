using RiskConsult.Core;
using RiskConsult.Data;
using RiskConsult.Enumerators;
using RiskConsult.Extensions;
using System.Text;

namespace RiskConsult._Tests.Performance;

public class PortfolioReturn : IReturnData, IPrintableReport
{
	public DateTime FinalDate { get; }
	public double FinalValue { get; }
	public CurrencyId FxCurrency { get; }
	public HoldingReturn[] HoldingsReturns { get; }
	public DateTime InitialDate { get; }
	public double InitialValue { get; }
	public string Name { get; }
	public double Return { get; }
	public PriceSourceId SourceID { get; }

	public PortfolioReturn( string portfolio, DateTime date,
		CurrencyId fxCurrency = CurrencyId.MXN, PriceSourceId sourceID = PriceSourceId.PiP_MD )
	{
		Name = portfolio;
		InitialDate = date.GetBusinessDateAdd( DateUnit.Day, -1 );
		FinalDate = date;
		FxCurrency = fxCurrency;
		SourceID = sourceID;

		// Obtengo la composición del portafolio en t-1
		IPortfolio port =
			DbZeus.Db.User?.Portfolios.GetPortfolio( InitialDate, portfolio ) ??
			DbZeus.Db.Portfolios.GetPortfolio( InitialDate, portfolio ) ??
			throw new Exception( $"Invalid portfolio {portfolio}" );

		// Calculo rendimientos
		HoldingsReturns = new HoldingReturn[ port.Holdings.Count ];
		for ( var i = 0; i < port.Holdings.Count; i++ )
		{
			IHolding holding = port.Holdings[ i ];
			var amount = holding.Amount;
			IHoldingTerms terms = holding.Terms;
			var priceReturn = PriceReturn.GetHoldingReturn( terms, date, sourceID );
			var fxReturn = FxReturn.GetFxReturn( date, terms.CurrencyId, fxCurrency );
			var fxPriceReturn = new FxPriceReturn( priceReturn, fxReturn );
			var holdReturn = new HoldingReturn( fxPriceReturn, amount );

			HoldingsReturns[ i ] = holdReturn;
			InitialValue += holdReturn.InitialValue;
			FinalValue += holdReturn.FinalValue;
		}

		for ( var i = 0; i < HoldingsReturns.Length; i++ )
		{
			HoldingsReturns[ i ].Weight = HoldingsReturns[ i ].InitialValue / InitialValue;
		}

		Return = InitialValue == 0 || FinalValue == 0 ? 0 : ( FinalValue / InitialValue ) - 1;
	}

	public static string GetReportHeaders()
	{
		return
			$"----- Portfolio Return Report-----\n" +
			$"Portfolio,Fx Currency,Price Source,Holdings," +
			$"Date_0,Date_1," +
			$"Value_0,Value_1,Return [bps],Return [$]";
	}

	public string GetReportLine()
	{
		var portData = $"{Name},{FxCurrency},{SourceID},{HoldingsReturns.Length}";
		var dateRange = $"{InitialDate.ToShortDateString()},{FinalDate.ToShortDateString()}";
		var portReturn = $"{InitialValue},{FinalValue},{Return * 10000},{FinalValue - InitialValue}";

		return $"{portData},{dateRange},{portReturn}";
	}

	public void PrintReport( string filePath )
	{
		StringBuilder text = new StringBuilder()
			.AppendLine( GetReportHeaders() )
			.AppendLine( GetReportLine() );
		File.WriteAllText( filePath, text.ToString() );
	}

	public override string ToString() => $"{FinalDate.ToShortDateString()}|{Return * 10000:F2} bps|{Name}";
}
