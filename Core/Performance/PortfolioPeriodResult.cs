using RiskConsult.Data;
using RiskConsult.Enumerators;
using RiskConsult.Extensions;
using System.Text;

namespace RiskConsult.Core.Performance;

public class PortfolioPeriodResult
{
	public double BpsReturn { get; }
	public double CashReturn { get; }
	public DateTime DateEnd { get; }
	public DateTime DateStart { get; }
	public CurrencyId FxCurrencyId { get; }
	public List<HoldingPeriodResult> HoldingsResults { get; }
	public string PortfolioID { get; }
	public List<PortfolioDateResult> PortfolioResults { get; }
	public PriceSourceId SourceID { get; }

	public PortfolioPeriodResult( string portfolio, DateTime start, DateTime end, PriceSourceId sourceID, CurrencyId fxCurrency, bool logToConsole = true )
	{
		if ( logToConsole )
		{
			Console.WriteLine(
				$"[{DateTime.Now.ToLocalTime()}] " +
				$"Performance for {portfolio} period " +
				$"between {start.ToShortDateString()} and {end.ToShortDateString()}" );
		}

		// Asignaciones directas
		PortfolioID = portfolio;
		SourceID = sourceID;
		FxCurrencyId = fxCurrency;
		DateStart = start.GetNextOrEqualsBusinessDay();
		DateEnd = end.GetBusinessPreviousOrEqualsDay();

		// Obtengo los resultados por cada día hábil
		PortfolioResults = [];
		IEnumerable<DateTime> period = DbZeus.Db.Dates.GetBusinessPeriod( start, end );
		foreach ( DateTime date in period )
		{
			if ( logToConsole )
			{
				Console.WriteLine( $"[{DateTime.Now.ToLocalTime()}] - Calculating {date.ToShortDateString()}..." );
			}

			PortfolioDateResult result = new( portfolio, date, sourceID, fxCurrency );
			PortfolioResults.Add( result );
		}

		// Obtengo los resultados a nivel instrumento y los agrego
		HoldingsResults =
		[
			.. HoldingPeriodResult.
						GetHoldingsPeriodResults( PortfolioResults, out var cashReturn, out var bpsReturn )
						.OrderBy( h => h.Description )
,
		];
		CashReturn = cashReturn;
		BpsReturn = bpsReturn;
	}

	public void PrintAll( string directory )
	{
		PrintPortfolioReport( Path.Combine( directory,
			$"{DateStart:yyyyMMdd}_{DateEnd:yyyyMMdd}_{PortfolioID}_PortfolioReport.csv" ) );
		PrintHoldingsReport( Path.Combine( directory,
			$"{DateStart:yyyyMMdd}_{DateEnd:yyyyMMdd}_{PortfolioID}_HoldingsReport.csv" ) );
		PrintDetailedReport( Path.Combine( directory,
			$"{DateStart:yyyyMMdd}_{DateEnd:yyyyMMdd}_{PortfolioID}_DetailedReport.csv" ) );
	}

	public void PrintDetailedReport( string filePath )
	{
		StringBuilder str = new StringBuilder()
			.AppendLine( GetReportHeaders() )
			.AppendLine( GetReportLine() )
			.AppendLine()
			.AppendLine( HoldingDateResult.GetReportHeaders() );

		foreach ( HoldingPeriodResult holdPeriod in HoldingsResults )
		{
			foreach ( HoldingDateResult holdDate in holdPeriod.Results )
			{
				_ = str.AppendLine( holdDate.GetReportLine() );
			}
		}

		File.WriteAllText( filePath, str.ToString() );
	}

	public void PrintHoldingsReport( string filePath )
	{
		StringBuilder str = new StringBuilder()
			.AppendLine( GetReportHeaders() )
			.AppendLine( GetReportLine() )
			.AppendLine()
			.AppendLine( HoldingPeriodResult.GetReportHeaders() );
		foreach ( HoldingPeriodResult holdPerRet in HoldingsResults )
		{
			_ = str.AppendLine( holdPerRet.GetReportLine() );
		}

		File.WriteAllText( filePath, str.ToString() );
	}

	public void PrintPortfolioReport( string filePath )
	{
		StringBuilder str = new StringBuilder()
			.AppendLine( GetReportHeaders() )
			.AppendLine( GetReportLine() )
			.AppendLine()
			.AppendLine( PortfolioDateResult.GetReportHeaders() );
		foreach ( PortfolioDateResult portDate in PortfolioResults )
		{
			_ = str.AppendLine( portDate.GetReportLine() );
		}

		var directory = Path.GetDirectoryName( filePath ) ?? throw new ArgumentException( "Invalid path", nameof( filePath ) );
		_ = Directory.CreateDirectory( directory );
		File.WriteAllText( filePath, str.ToString() );
	}

	internal static string GetReportHeaders()
	{
		return "----- Portfolio Period Return Report -----\n" +
			"Portfolio," +
			"DateStart," +
			"DateEnd," +
			"Periods," +
			"Holdings," +
			"FxCurrency," +
			"PriceSource," +
			"Return [bps]," +
			"Return [$]," +
			"AvReturn [bps],";
	}

	internal string GetReportLine()
	{
		return
			$"{PortfolioID}," +
			$"{DateStart.ToShortDateString()}," +
			$"{DateEnd.ToShortDateString()}," +
			$"{PortfolioResults.Count}," +
			$"{HoldingsResults.Count}," +
			$"{FxCurrencyId}," +
			$"{SourceID}," +
			$"{BpsReturn}," +
			$"{CashReturn}," +
			$"{BpsReturn / PortfolioResults.Count},";
	}
}
