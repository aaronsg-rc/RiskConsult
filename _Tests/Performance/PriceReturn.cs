using RiskConsult.Core;
using RiskConsult.Enumerators;
using RiskConsult.Extensions;
using System.Text;

namespace RiskConsult._Tests.Performance;

/// <summary> Clase que me permite obtener el rendimiento de un instrumento para una fecha </summary>
public class PriceReturn : IReturnData, IPrintableReport
{
	private static readonly Dictionary<(int, DateTime, PriceSourceId), PriceReturn> _cache = [];

	/// <summary> Fecha en t del rendimiento </summary>
	public DateTime FinalDate { get; }

	/// <summary> Precio sucio en t (afectado por cupones, amortizaciones y dividendos) </summary>
	public double FinalValue { get; }

	public DateTime InitialDate { get; }

	/// <summary> Precio limpio en t-1 </summary>
	public double InitialValue { get; }

	/// <summary> Rendimiento porcentual entre el precio inicial y el final </summary>
	public double Return { get; }

	/// <summary> ID del tipo de precios </summary>
	public PriceSourceId SourceID { get; }

	/// <summary> Términos y condiciones del instrumento </summary>
	public IHoldingTerms Terms { get; }

	/// <summary> Obtiene el rendimiento que el instrumento tuvo en la fecha solicitada </summary>
	/// <param name="terms"> Términos y condiciones del instrumento </param>
	/// <param name="date"> Fecha en que se quiere obtener el rendimiento </param>
	/// <param name="sourceID"> ID del tipo de precios </param>
	private PriceReturn( IHoldingTerms terms, DateTime date, PriceSourceId sourceID )
	{
		Terms = terms;
		SourceID = sourceID;
		InitialDate = date.GetBusinessDateAdd( DateUnit.Day, -1 );
		InitialValue = terms.GetCleanPrice( InitialDate, sourceID );
		FinalDate = date;
		FinalValue = terms.GetDirtyPrice( date, sourceID, out _, out _, out _ );
		Return = InitialValue == 0 || FinalValue == 0 ? 0 : ( FinalValue / InitialValue ) - 1;
	}

	/// <summary> Obtiene el rendimiento que el instrumento tuvo en la fecha solicitada </summary>
	/// <param name="terms"> Términos y condiciones del instrumento </param>
	/// <param name="date"> Fecha en que se quiere obtener el rendimiento </param>
	/// <param name="sourceID"> ID del tipo de precios </param>
	/// <returns> Clase inicializada con rendimientos del día </returns>
	public static PriceReturn GetHoldingReturn( IHoldingTerms terms, DateTime date, PriceSourceId sourceID = PriceSourceId.PiP_MD )
	{
		if ( _cache.TryGetValue( (terms.HoldingId, date, sourceID), out PriceReturn? dateReturn ) )
		{
			return dateReturn;
		}

		dateReturn = new PriceReturn( terms, date, sourceID );

		return _cache[ (terms.HoldingId, date, sourceID) ] = dateReturn;
	}

	public static string GetReportHeaders()
	{
		return
			$"----- Price Return Report-----\n" +
			$"Description,Price Currency,Price Source," +
			$"Date_0,Date_1," +
			$"Price_0,Price_1,Price Return [bps],Price Return [$]";
	}

	public string GetReportLine()
	{
		var holdingData = $"{Terms.Description},{Terms.CurrencyId},{SourceID}";
		var dateRange = $"{InitialDate.ToShortDateString()},{FinalDate.ToShortDateString()}";
		var priceReturn = $"{InitialValue},{FinalValue},{Return * 10000},{FinalValue - InitialValue}";

		return $"{holdingData},{dateRange},{priceReturn}";
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
