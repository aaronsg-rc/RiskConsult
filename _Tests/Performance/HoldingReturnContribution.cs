using RiskConsult.Core;
using RiskConsult.Enumerators;
using System.Text;

namespace RiskConsult._Tests.Performance;

public class HoldingReturnContribution : IPrintableReport
{
	public double AverageWeight { get; }
	public double BpsFxReturn { get; } = 1;
	public double BpsPriceReturn { get; } = 1;
	public double BpsReturn { get; } = 1;
	public double CashFxReturn { get; }
	public double CashPriceReturn { get; }
	public double CashReturn { get; }
	public DateTime FinalDate { get; }
	public CurrencyId FxCurrency => HoldingReturns[ 0 ].FxCurrency;
	public List<HoldingReturn> HoldingReturns { get; } = [];
	public DateTime InitialDate { get; }
	public IHoldingTerms Terms => HoldingReturns[ 0 ].Terms;

	public HoldingReturnContribution( int holdingId, PortfolioReturn[] portfolioReturns )
	{
		foreach ( PortfolioReturn portReturn in portfolioReturns )
		{
			HoldingReturn? holdReturn = portReturn.HoldingsReturns.FirstOrDefault( hr => hr.Terms.HoldingId.Equals( holdingId ) );
			if ( holdReturn == null )
			{
				continue;
			}

			HoldingReturns.Add( holdReturn );
			AverageWeight += holdReturn.Weight;
			BpsReturn *= 1 + ( holdReturn.Weight * holdReturn.FxPriceReturn.Return );
			BpsFxReturn *= 1 + ( holdReturn.Weight * holdReturn.FxPriceReturn.FxReturn.Return );
			BpsPriceReturn *= 1 + ( holdReturn.Weight * holdReturn.FxPriceReturn.PriceReturn.Return );
			CashReturn += holdReturn.Return * holdReturn.InitialValue;
			CashFxReturn += ( holdReturn.FxPriceReturn.FxReturn.FinalValue - holdReturn.FxPriceReturn.FxReturn.InitialValue ) * holdReturn.FxPriceReturn.PriceReturn.InitialValue;
			CashPriceReturn += ( holdReturn.FxPriceReturn.PriceReturn.FinalValue - holdReturn.FxPriceReturn.PriceReturn.InitialValue ) * holdReturn.FxPriceReturn.FxReturn.FinalValue;
		}

		HoldingReturn? minRetDate = HoldingReturns.MinBy( h => h.InitialDate );
		HoldingReturn? maxRetDate = HoldingReturns.MaxBy( h => h.InitialDate );
		InitialDate = minRetDate?.InitialDate ?? DateTime.MinValue;
		FinalDate = maxRetDate?.InitialDate ?? DateTime.MaxValue;
		AverageWeight /= portfolioReturns.Length;
		BpsReturn -= 1;
		BpsFxReturn -= 1;
		BpsPriceReturn -= 1;
	}

	public static List<HoldingReturnContribution> GetHoldingReturnContributions( PortfolioReturn[] portfolioReturns )
	{
		IEnumerable<int> holdIDs = portfolioReturns.SelectMany( p => p.HoldingsReturns ).Select( h => h.Terms.HoldingId );
		var holdingsContributions = new List<HoldingReturnContribution>();
		foreach ( var id in holdIDs )
		{
			var holdContr = new HoldingReturnContribution( id, portfolioReturns );
			holdingsContributions.Add( holdContr );
		}

		return holdingsContributions;
	}

	/// <summary> Construye una linea de encabezados a partir de <paramref name="fxCurrency" /> </summary>
	public static string GetReportHeaders()
	{
		return
			$"----- Holding Return Report-----\n" +
			$"Description,Price Currency,Fx Currency,Av. Weight," +
			$"Date_0,Date_1," +
			$"Return [bps],Fx Cont. [bps],Price Cont. [bps]," +
			$"Return [$],Fx Cont. [$],Price Cont. [$]";
	}

	/// <summary> Obtiene la linea correspondiente de valores de la fecha </summary>
	public string GetReportLine()
	{
		var holdingData = $"{Terms.Description},{Terms.CurrencyId},{FxCurrency},{AverageWeight}";
		var dateRange = $"{InitialDate.ToShortDateString()},{FinalDate.ToShortDateString()}";
		var bpsReturn = $"{BpsReturn},{BpsFxReturn},{BpsPriceReturn}";
		var cashReturn = $"{CashReturn},{CashFxReturn},{CashPriceReturn}";

		return $"{holdingData},{dateRange},{bpsReturn},{cashReturn}";
	}

	/// <summary> Imprime los resultados en la ruta indicada </summary>
	/// <param name="directoryPath"> </param>
	public void PrintReport( string filePath )
	{
		StringBuilder txt = new StringBuilder()
			.AppendLine( GetReportHeaders() )
			.AppendLine( GetReportLine() );
		File.WriteAllText( filePath, txt.ToString() );
	}

	public override string ToString() => $"{Terms.Description}|{AverageWeight:P2}|{BpsReturn * 10000:F2} bps";
}
