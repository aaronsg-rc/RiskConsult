using RiskConsult.Core;
using RiskConsult.Data;
using RiskConsult.Data.Interfaces;
using RiskConsult.Enumerators;

namespace RiskConsult.Extensions;

public static class EventExtensions
{
	/// <summary> Obtiene el monto por concepto de eventos que se paga para un instrumento en una fecha </summary>
	/// <param name="holding"> ID del instrumento </param>
	/// <param name="date"> Fecha de las entidades </param>
	/// <param name="price"> Precio del instrumento en la fecha </param>
	/// <returns> Monto por concepto de eventos o 0 si no se encuentra nada </returns>
	public static double GetPayoutByEvents( this IHoldingIdProperty holding, DateTime date, double price )
	{
		IHoldingTerms terms = holding as IHoldingTerms ?? holding.GetHoldingTerms();
		if ( terms.TypeId is TypeId.Equity or TypeId.Mxequity2 )
		{
			return DbZeus.Db.Events.GetPayoutByEvents( terms.HoldingId, date, price, terms.CurrencyId );
		}

		return 0;
	}
}
