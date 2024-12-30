using RiskConsult.Data;
using RiskConsult.Enumerators;
using System.Text;

namespace RiskConsult._Tests.Performance;

public class PortfolioPeriodReturn : IReturnData, IPrintableReport
{
	public HoldingReturnContribution[] HoldingsContributions;
	public DateTime FinalDate { get; }
	public double FinalValue { get; }
	public CurrencyId FxCurrency { get; }
	public DateTime InitialDate { get; }
	public double InitialValue { get; }
	public string Name { get; }
	public PortfolioReturn[] PortfolioReturns { get; }
	public double Return { get; }
	public PriceSourceId SourceID { get; }

	public PortfolioPeriodReturn( string portfolio, DateTime initialDate, DateTime finalDate,
		CurrencyId fxCurrency = CurrencyId.MXN, PriceSourceId sourceID = PriceSourceId.PiP_MD )
	{
		Name = portfolio;
		InitialDate = initialDate;
		FinalDate = finalDate;
		FxCurrency = fxCurrency;
		SourceID = sourceID;

		DateTime[] dates = DbZeus.Db.Dates.GetBusinessPeriod( initialDate, finalDate ).ToArray();
		PortfolioReturns = new PortfolioReturn[ dates.Length ];
		Return = 1;
		for ( var i = 0; i < dates.Length; i++ )
		{
			Console.WriteLine( $"Portfolio Return for {Name} {dates[ i ].ToShortDateString()}" );
			var dateReturn = new PortfolioReturn( portfolio, dates[ i ], fxCurrency, sourceID );
			Return *= 1 + dateReturn.Return;
			PortfolioReturns[ i ] = dateReturn;
		}

		Return -= 1;
		InitialValue = PortfolioReturns.MinBy( r => r.InitialDate )?.InitialValue ?? 0;
		FinalValue = PortfolioReturns.MaxBy( r => r.FinalDate )?.FinalValue ?? 0;
		HoldingsContributions = [ .. HoldingReturnContribution.GetHoldingReturnContributions( PortfolioReturns ) ];
	}

	public static string GetReportHeaders()
	{
		return
			$"----- Portfolio Period Return Report-----\n" +
			$"Portfolio,Fx Currency,Price Source,Days," +
			$"Start Date,End Date," +
			$"Value_0,Value_1,Return [bps],Return [$]";
	}

	public string GetReportLine()
	{
		var portData = $"{Name},{FxCurrency},{SourceID},{PortfolioReturns.Length}";
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

	public override string ToString() => $"{InitialDate.ToShortDateString()}|{FinalDate.ToShortDateString()}|{Return * 10000:F2} bps|{Name}";
}
