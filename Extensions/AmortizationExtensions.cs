using RiskConsult.Core;
using RiskConsult.Data;
using RiskConsult.Data.Interfaces;

namespace RiskConsult.Extensions;

public static class AmortizationExtensions
{
	/// <summary> Obtiene el nominal actualizado considerando amortizaciones </summary>
	/// <returns> Nominal actualizado o 0 si no aplica </returns>
	/// <summary> Obtiene el monto sobre el precio del instrumento que se debe pagar por concepto de amortización </summary>
	/// <returns> Monto por concepto de amortización o 0 si no aplica </returns>
	public static double GetAmortizedNominalAt( this IHoldingIdProperty holdingId, DateTime date )
	{
		if ( holdingId.HoldingId >= 2000000 )
		{
			IHoldingTerms terms = holdingId as IHoldingTerms ?? holdingId.GetHoldingTerms();
			return terms.Nominal;
		}

		return DbZeus.Db.Amortizations.GetAmortizedNominalAt( holdingId, date );
	}

	/// <summary> Obtiene el monto sobre el precio del instrumento que se debe pagar por concepto de amortización </summary>
	/// <returns> Monto por concepto de amortización o 0 si no aplica </returns>
	public static double GetPayoutByAmortization( this IHoldingIdProperty holdingId, DateTime date )
	{
		if ( holdingId.HoldingId >= 2000000 )
		{
			return 0;
		}

		return DbZeus.Db.Amortizations.GetPayoutByAmortization( holdingId, date );
	}
}
