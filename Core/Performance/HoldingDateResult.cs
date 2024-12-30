using RiskConsult.Enumerators;
using RiskConsult.Extensions;
using System.Text;

namespace RiskConsult.Core.Performance;

public class HoldingDateResult : HoldingDateReturn
{
	public double Amount { get; }

	/// <summary> Rendimiento en puntos base del tipo de cambio </summary>
	public double BpsFxReturn { get; }

	public override double BpsPriceReturn { get; }

	/// <summary> Rendimiento en bps compuesto de precio y tipo de cambio </summary>
	public double BpsTotalReturn { get; }

	/// <summary> Rendimiento en $ de la moneda del tipo de cambio </summary>
	public double CashFxReturn { get; }

	/// <summary> Rendimiento en $ por cambio de precio afectado por el tipo de cambio </summary>
	public override double CashPriceReturn { get; }

	/// <summary> Rendimiento en $ compuesto de precio y tipo de cambio </summary>
	public double CashTotalReturn { get; }

	/// <summary> Moneda sobre la que se calculan los rendimientos por FX </summary>
	public CurrencyId FxCurrencyId { get; }

	/// <summary> Tipo de cambio en t </summary>
	public double FxFinal { get; }

	/// <summary> Tipo de cambio en t-1 </summary>
	public double FxInitial { get; }

	/// <summary> Precio en t afectado por tipo de cambio </summary>
	public double PriceFxBase { get; }

	/// <summary> Precio en t afectado por tipo de cambio, cupones, amortizaciones y dividendos en t </summary>
	public double PriceFxFinal { get; }

	/// <summary> Precio en t-1 </summary>
	public double PriceFxInitial { get; }

	/// <summary> Valor afectado por tipo de cambio en t-1 </summary>
	public double Value { get; }

	/// <summary> Valor afectado por tipo de cambio, cupones, amortizaciones y dividendos en t </summary>
	public double ValueFinal { get; }

	/// <summary> Ponderación del instrumento en el portafolio para t-1 </summary>
	public double Weight { get; set; }

	/// <summary> Obtiene los rendimientos que el instrumento tuvo en la fecha solicitada </summary>
	/// <param name="tycs"> Términos y condiciones del instrumento </param>
	/// <param name="date"> Fecha en que se quiere obtener el rendimiento </param>
	/// <param name="sourceID"> ID del tipo de precios </param>
	/// <param name="fxCurrency"> Moneda sobre la que se van a obtener los rendimientos /// </param>
	internal HoldingDateResult( HoldingDateReturn dateReturn, double amount, CurrencyId fxCurrency ) : base( dateReturn )
	{
		Amount = amount;
		FxCurrencyId = fxCurrency;
		FxInitial = dateReturn.CurrencyId.ConvertToCurrency( fxCurrency, dateReturn.DateInitial );
		FxFinal = dateReturn.CurrencyId.ConvertToCurrency( fxCurrency, dateReturn.Date );
		PriceFxBase = FxFinal * dateReturn.PriceClean;
		PriceFxInitial = FxInitial * dateReturn.PriceInitial;
		PriceFxFinal = FxFinal * dateReturn.PriceFinal;
		Value = amount * PriceFxInitial;
		ValueFinal = amount * PriceFxFinal;
		if ( Value != 0 && ValueFinal != 0 )
		{
			CashPriceReturn = amount * FxFinal * ( dateReturn.PriceFinal - dateReturn.PriceInitial );
			CashFxReturn = amount * ( FxFinal - FxInitial ) * PriceInitial;
			CashTotalReturn = ValueFinal - Value;
			BpsFxReturn = ( ( FxFinal / FxInitial ) - 1 ) * 10000;
			BpsPriceReturn = ( ( PriceFinal / PriceInitial ) - 1 ) * 10000;
			BpsTotalReturn = ( ( PriceFxFinal / PriceFxInitial ) - 1 ) * 10000;
		}
	}

	/// <summary> Construye una linea de encabezados a partir de <paramref name="fxCurrency" /> </summary>
	public static string GetReportHeaders()
	{
		return "----- Holding Date Return Report-----\n" +
		   "Date," +
		   "HoldingId," +
		   "Description," +
		   "Amount," +
		   "Weight," +
		   "PriceReturn [bps]," +
		   "FxReturn [bps]," +
		   "TotalReturn [bps]," +
		   "PriceReturn [$]," +
		   "FxReturn [$]," +
		   "TotalReturn [$]," +
		   "Currency," +
		   "FxCurrency," +
		   "CleanPrice_1," +
		   "Price_0," +
		   "Price_1," +
		   "Fx_0," +
		   "Fx_1," +
		   "FxPrice_0," +
		   "FxPrice_1," +
		   "Value_0," +
		   "Value_1," +
		   "Payout Coupon," +
		   "Payout Amortization," +
		   "Payout Dividend,";
	}

	/// <summary> Obtiene la linea correspondiente de valores de la fecha </summary>
	public string GetReportLine()
	{
		return
			$"{Date.ToShortDateString()}," +
			$"{HoldingId}," +
			$"{Description}," +
			$"{Amount}," +
			$"{Weight}," +
			$"{BpsPriceReturn}," +
			$"{BpsFxReturn}," +
			$"{BpsTotalReturn}," +
			$"{CashPriceReturn}," +
			$"{CashFxReturn}," +
			$"{CashTotalReturn}," +
			$"{CurrencyId}," +
			$"{FxCurrencyId}," +
			$"{PriceClean}," +
			$"{PriceInitial}," +
			$"{PriceFinal}," +
			$"{FxInitial}," +
			$"{FxFinal}," +
			$"{PriceFxInitial}," +
			$"{PriceFxFinal}," +
			$"{Value}," +
			$"{ValueFinal}," +
			$"{PayoutCoupon}," +
			$"{PayoutAmortization}," +
			$"{PayoutDividend},";
	}

	/// <summary> Imprime los resultados en la ruta indicada </summary>
	/// <param name="directoryPath"> </param>
	public void PrintReport( string filePath )
	{
		StringBuilder str = new StringBuilder()
			.AppendLine( GetReportHeaders() )
			.AppendLine( GetReportLine() );
		File.WriteAllText( filePath, str.ToString() );
	}

	/// <summary> Rendimiento de la fecha y nombre del instrumento </summary>
	public override string ToString() => $"{Date.ToShortDateString()}|{BpsTotalReturn:F2} bps|{Description}";
}
