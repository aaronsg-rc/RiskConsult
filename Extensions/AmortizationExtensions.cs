using RiskConsult.Core;
using RiskConsult.Data;
using RiskConsult.Data.Interfaces;
using RiskConsult.Enumerators;

namespace RiskConsult.Extensions;

public static class AmortizationExtensions
{
	/// <summary> Obtiene el nominal actualizado considerando amortizaciones </summary>
	/// <returns> Nominal actualizado o 0 si no aplica </returns>
	/// <summary> Obtiene el monto sobre el precio del instrumento que se debe pagar por concepto de amortización </summary>
	/// <returns> Monto por concepto de amortización o 0 si no aplica </returns>
	public static double GetAmortizedNominalAt( this IHoldingTerms terms, DateTime date )
	{
		if ( terms.HoldingId < 2000000 || terms.ModuleId is ModuleId.Fixed or ModuleId.Float or ModuleId.Float2 )
		{
			Dictionary<DateTime, double> calendar = DbZeus.Db.Amortizations.GetAmortizationCalendar( terms.HoldingId );
			if ( calendar.Count > 0 )
			{
				return terms.Nominal * ( 1 - calendar.Where( kvp => kvp.Key < date ).Sum( kvp => kvp.Value ) );
			}
		}

		return terms.Nominal;
	}

	public static double GetAmortizedNominalAt( this IHoldingIdProperty holdingId, DateTime date )
	{
		return holdingId.GetHoldingTerms().GetAmortizedNominalAt( date );
	}

	public static double GetPayoutByAmortization( this IHoldingIdProperty holdingId, DateTime date )
	{
		return holdingId.GetHoldingTerms().GetPayoutByAmortization( date );
	}

	/// <summary> Obtiene el monto sobre el precio del instrumento que se debe pagar por concepto de amortización </summary>
	/// <returns> Monto por concepto de amortización o 0 si no aplica </returns>
	public static double GetPayoutByAmortization( this IHoldingTerms terms, DateTime date )
	{
		if ( terms.HoldingId < 2000000 && terms.ModuleId is ModuleId.Fixed or ModuleId.Float or ModuleId.Float2 )
		{
			var amortization = DbZeus.Db.Amortizations.GetAmortizationPercent( terms.HoldingId, date );
			return amortization * terms.Nominal;
		}

		return 0;
	}
}
