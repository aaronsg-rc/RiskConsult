using RiskConsult.Core;
using RiskConsult.Enumerators;
using System.Text;

namespace RiskConsult._Tests.Performance;

/// <summary> Obtiene los rendimientos que el instrumento tuvo en la fecha solicitada </summary>
/// <param name="tycs"> Términos y condiciones del instrumento </param>
/// <param name="date"> Fecha en que se quiere obtener el rendimiento </param>
/// <param name="sourceID"> ID del tipo de precios </param>
/// <param name="fxCurrency"> Moneda sobre la que se van a obtener los rendimientos /// </param>
public class HoldingReturn( FxPriceReturn fxPriceReturn, double amount ) : IReturnData, IPrintableReport
{
	public double Amount { get; } = amount;
	public DateTime FinalDate => FxPriceReturn.FinalDate;
	/// <summary> Valor afectado por tipo de cambio, cupones, amortizaciones y dividendos en t </summary>
	public double FinalValue { get; } = amount * fxPriceReturn.FinalValue;
	public CurrencyId FxCurrency => FxPriceReturn.FxCurrency;
	public FxPriceReturn FxPriceReturn { get; } = fxPriceReturn;
	public DateTime InitialDate => FxPriceReturn.InitialDate;
	/// <summary> Valor afectado por tipo de cambio en t-1 </summary>
	public double InitialValue { get; } = amount * fxPriceReturn.InitialValue;
	public double Return => FxPriceReturn.Return;
	public PriceSourceId SourceID => FxPriceReturn.SourceID;
	public IHoldingTerms Terms => FxPriceReturn.Terms;
	/// <summary> Ponderación del instrumento en el portafolio para t-1 </summary>
	public double Weight { get; set; }

	/// <summary> Construye una linea de encabezados a partir de <paramref name="fxCurrency" /> </summary>
	public static string GetReportHeaders()
	{
		return
			$"----- Holding Return Report-----\n" +
			$"Description,Price Currency,Fx Currency,Amount,Weight," +
			$"Date_0,Date_1," +
			$"Value_0,Value_1,Return [bps],Return [$]," +
			$"FxPrice_0,FxPrice_1,FxPrice Return [bps],FxPrice Return [$]," +
			$"Price_0,Price_1,Price Return [bps],Price Return [$]," +
			$"Fx_0,Fx_1,Fx Return [bps],Fx Return [$]";
	}

	/// <summary> Obtiene la linea correspondiente de valores de la fecha </summary>
	public string GetReportLine()
	{
		var holdingData = $"{Terms.Description},{Terms.CurrencyId},{FxCurrency},{Amount},{Weight}";
		var dateRange = $"{InitialDate.ToShortDateString()},{FinalDate.ToShortDateString()}";
		var holdReturn = $"{InitialValue},{FinalValue},{Return * 10000},{FinalValue - InitialValue}";
		var fxPriceReturn = $"{FxPriceReturn.InitialValue},{FxPriceReturn.FinalValue},{FxPriceReturn.Return * 10000},{FxPriceReturn.FinalValue - FxPriceReturn.InitialValue}";
		var priceReturn = $"{FxPriceReturn.PriceReturn.InitialValue},{FxPriceReturn.PriceReturn.FinalValue},{FxPriceReturn.PriceReturn.Return * 10000},{( FxPriceReturn.PriceReturn.FinalValue - FxPriceReturn.PriceReturn.InitialValue ) * FxPriceReturn.FxReturn.FinalValue}";
		var fxReturn = $"{FxPriceReturn.FxReturn.InitialValue},{FxPriceReturn.FxReturn.FinalValue},{FxPriceReturn.FxReturn.Return * 10000},{( FxPriceReturn.FxReturn.FinalValue - FxPriceReturn.FxReturn.InitialValue ) * FxPriceReturn.PriceReturn.InitialValue}";

		return $"{holdingData},{dateRange},{holdReturn},{fxPriceReturn},{priceReturn},{fxReturn}";
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

	/// <summary> Rendimiento de la fecha y nombre del instrumento </summary>
	public override string ToString() => $"{FinalDate.ToShortDateString()}|{Return * 10000:F2} bps|{Terms.Description}";
}
