using RiskConsult.Enumerators;
using RiskConsult.Extensions;
using System.Text;

namespace RiskConsult.Core.Performance;

public class PortfolioDateResult
{
	public double BpsReturn { get; }
	public double CashReturn { get; }
	public DateTime Date { get; }
	public CurrencyId FxCurrencyId { get; }
	public List<HoldingDateResult> Holdings { get; }
	public string PortfolioID { get; }
	public PriceSourceId SourceID { get; }
	public double Value { get; }
	public double ValueClose { get; }

	public PortfolioDateResult( string portfolio, DateTime date, PriceSourceId sourceID, CurrencyId fxCurrency )
	{
		PortfolioID = portfolio;
		Date = date;
		SourceID = sourceID;
		FxCurrencyId = fxCurrency;
		Holdings = [];

		DateTime previous = date.GetBusinessDateAdd( DateUnit.Day, -1 );
		IPortfolio port = portfolio.GetPortfolio( previous );
		foreach ( IHolding holding in port.Holdings )
		{
			var holdReturn = HoldingDateReturn.GetHoldingDateReturn( holding.Terms, date, sourceID );
			var holdResult = new HoldingDateResult( holdReturn, holding.Amount, fxCurrency );
			Value += holdResult.Value;
			ValueClose += holdResult.ValueFinal;
			Holdings.Add( holdResult );
		}

		// Asigno ponderaciones
		foreach ( HoldingDateResult holdResult in Holdings )
		{
			holdResult.Weight = holdResult.Value / Value;
		}

		CashReturn = ValueClose - Value;
		BpsReturn = Value == 0 || ValueClose == 0 ? 0 :
			( ( ValueClose / Value ) - 1 ) * 10000;
	}

	public static string GetReportHeaders()
	{
		return "----- Portfolio Date Return Report -----\n" +
			"Date," +
			"Portfolio," +
			"FxCurrency," +
			"Return [bps]," +
			"Return [$]," +
			"Value_0," +
			"Value_1,";
	}

	public string GetReportLine()
	{
		return
			$"{Date.ToShortDateString()}," +
			$"{PortfolioID}," +
			$"{FxCurrencyId}," +
			$"{BpsReturn}," +
			$"{CashReturn}," +
			$"{Value}," +
			$"{ValueClose},";
	}

	public void PrintReport( string filePath )
	{
		StringBuilder str = new StringBuilder()
			.AppendLine( GetReportHeaders() )
			.AppendLine( GetReportLine() )
			.AppendLine()
			.AppendLine( HoldingDateResult.GetReportHeaders() );
		foreach ( HoldingDateResult holdRet in Holdings )
		{
			_ = str.AppendLine( holdRet.GetReportLine() );
		}

		File.WriteAllText( filePath, str.ToString() );
	}

	public override string ToString() => $"{Date.ToShortDateString()}|{BpsReturn:F2} bps|{PortfolioID}";
}
