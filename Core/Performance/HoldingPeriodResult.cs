using RiskConsult.Data;
using RiskConsult.Enumerators;
using RiskConsult.Extensions;
using System.Text;

namespace RiskConsult.Core.Performance;

public class HoldingPeriodResult
{
	public double AvWeight { get; }
	public double BpsReturnFx { get; }
	public double BpsReturnPrice { get; }
	public double BpsReturnTotal { get; }
	public double CashReturnFx { get; }
	public double CashReturnPrice { get; }
	public double CashReturnTotal { get; }
	public CurrencyId CurrencyId => Results.FirstOrDefault()?.Terms.CurrencyId ?? CurrencyId.Invalid;
	public DateTime DateEnd { get; }
	public DateTime DateStart { get; }
	public string Description => Results.FirstOrDefault()?.Description ?? string.Empty;
	public CurrencyId FxCurrencyId => Results.FirstOrDefault()?.FxCurrencyId ?? CurrencyId.Invalid;
	public int HoldingId => Results.FirstOrDefault()?.HoldingId ?? -1;
	public List<HoldingDateResult> Results { get; }
	public PriceSourceId SourceID => Results.FirstOrDefault()?.SourceID ?? PriceSourceId.Invalid;

	public HoldingPeriodResult( int holdingId, DateTime start, DateTime end, PriceSourceId sourceID, CurrencyId fxCurrency )
	{
		double cashReturnFx = 0;
		double cashReturnPrice = 0;
		double cashReturnTotal = 0;
		double bpsReturnFx = 0;
		double bpsReturnPrice = 0;
		double bpsReturnTotal = 0;
		IHoldingTerms? terms = holdingId.GetHoldingTerms();
		IEnumerable<DateTime> period = DbZeus.Db.Dates.GetBusinessPeriod( start, end );
		Results = [];
		foreach ( DateTime date in period )
		{
			HoldingDateReturn holdRet = HoldingDateResult.GetHoldingDateReturn( terms, date, sourceID );
			var holdRes = new HoldingDateResult( holdRet, 1, fxCurrency );
			cashReturnFx += holdRes.CashFxReturn;
			cashReturnPrice += holdRes.CashPriceReturn;
			cashReturnTotal += holdRes.CashTotalReturn;
			bpsReturnPrice += holdRes.BpsPriceReturn;
			bpsReturnFx += holdRes.BpsFxReturn;
			bpsReturnTotal += holdRes.BpsTotalReturn;
			Results.Add( holdRes );
		}

		// Asignaciones
		DateStart = start;
		DateEnd = end;
		AvWeight = 1;
		CashReturnFx = cashReturnFx;
		CashReturnPrice = cashReturnPrice;
		CashReturnTotal = cashReturnTotal;
		BpsReturnFx = bpsReturnFx;
		BpsReturnPrice = bpsReturnPrice;
		BpsReturnTotal = bpsReturnTotal;
	}

	internal HoldingPeriodResult( IEnumerable<HoldingDateResult> holdingReturns, int periods )
	{
		DateTime start = DateTime.MaxValue;
		DateTime end = DateTime.MinValue;
		double weightsSum = 0;
		double cashReturnFx = 0;
		double cashReturnPrice = 0;
		double cashReturnTotal = 0;
		double bpsReturnFx = 0;
		double bpsReturnPrice = 0;
		double bpsReturnTotal = 0;
		foreach ( HoldingDateResult dateReturn in holdingReturns )
		{
			start = dateReturn.Date < start ? dateReturn.Date : start;
			end = dateReturn.Date > end ? dateReturn.Date : end;
			cashReturnFx += dateReturn.CashFxReturn;
			cashReturnPrice += dateReturn.CashPriceReturn;
			cashReturnTotal += dateReturn.CashTotalReturn;
			bpsReturnPrice += dateReturn.Weight * dateReturn.BpsPriceReturn;
			bpsReturnFx += dateReturn.Weight * dateReturn.BpsFxReturn;
			bpsReturnTotal += dateReturn.Weight * dateReturn.BpsTotalReturn;
			weightsSum += dateReturn.Weight;
		}

		// Asignaciones
		Results = new( holdingReturns );
		DateStart = start;
		DateEnd = end;
		AvWeight = weightsSum / periods;
		CashReturnFx = cashReturnFx;
		CashReturnPrice = cashReturnPrice;
		CashReturnTotal = cashReturnTotal;
		BpsReturnFx = bpsReturnFx;
		BpsReturnPrice = bpsReturnPrice;
		BpsReturnTotal = bpsReturnTotal;
	}

	public HoldingDateResult? this[ DateTime date ] => Results.FirstOrDefault( r => r.Date == date );
	public HoldingDateResult this[ int index ] => Results[ index ];

	public static List<HoldingPeriodResult> GetHoldingsPeriodResults( List<PortfolioDateResult> portfolioResults,
		out double cashReturn, out double bpsReturn )
	{
		cashReturn = 0;
		bpsReturn = 0;
		List<HoldingPeriodResult> holdsResults = [];
		IEnumerable<int> ids = portfolioResults
			.SelectMany( result => result.Holdings )
			.Select( holding => holding.HoldingId )
			.Distinct();
		foreach ( var holdingId in ids )
		{
			IEnumerable<HoldingDateResult> holdResults = portfolioResults
				.SelectMany( result => result.Holdings )
				.Where( holding => holding.HoldingId == holdingId );
			HoldingPeriodResult holdRes = new( holdResults, portfolioResults.Count );
			holdsResults.Add( holdRes );
			cashReturn += holdRes.CashReturnTotal;
			bpsReturn += holdRes.BpsReturnTotal;
		}

		return holdsResults;
	}

	public static string GetReportHeaders()
	{
		return "----- Holding Period Return Report -----\n" +
			"DateStart," +
			"DateEnd," +
			"HoldingId," +
			"Description," +
			"Currency," +
			"FxCurrency," +
			"AvWeight," +
			"PriceReturn [bps]," +
			"FxReturn [bps]," +
			"TotalReturn [bps]," +
			"PriceReturn [$]," +
			"FxReturn [$]," +
			"TotalReturn [$],";
	}

	public string GetReportLine()
	{
		return
			$"{DateStart.ToShortDateString()}," +
			$"{DateEnd.ToShortDateString()}," +
			$"{HoldingId}," +
			$"{Description}," +
			$"{CurrencyId}," +
			$"{FxCurrencyId}," +
			$"{AvWeight}," +
			$"{BpsReturnPrice}," +
			$"{BpsReturnFx}," +
			$"{BpsReturnTotal}," +
			$"{CashReturnPrice}," +
			$"{CashReturnFx}," +
			$"{CashReturnTotal},";
	}

	public void PrintReport( string filePath )
	{
		StringBuilder str = new StringBuilder()
			.AppendLine( GetReportHeaders() )
			.AppendLine( GetReportLine() )
			.AppendLine()
			.AppendLine( HoldingDateResult.GetReportHeaders() );
		foreach ( HoldingDateResult dateRet in Results )
		{
			_ = str.AppendLine( dateRet.GetReportLine() );
		}

		File.WriteAllText( filePath, str.ToString() );
	}

	/// <summary> Rendimiento total del instrumento en el periodo </summary>
	public override string ToString() => $"[{BpsReturnTotal:F2} bps] {Description}";
}
