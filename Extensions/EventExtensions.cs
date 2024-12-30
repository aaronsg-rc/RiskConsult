using RiskConsult.Core;
using RiskConsult.Data;
using RiskConsult.Data.Interfaces;
using RiskConsult.Enumerators;

namespace RiskConsult.Extensions;

public static class EventExtensions
{
	/// <summary> Obtiene el monto por concepto de eventos que se paga para un instrumento en una fecha </summary>
	/// <param name="holdingId"> ID del instrumento </param>
	/// <param name="date"> Fecha de las entidades </param>
	/// <param name="csZeus"> Cadena de conexión a Zeus </param>
	/// <param name="price"> Precio del instrumento en la fecha </param>
	/// <param name="fxValue"> Tipo de cambio en el que se encuentra el precio respecto a la moneda del instrumento </param>
	/// <param name="tycs"> Términos y condiciones del instrumento </param>
	/// <param name="holding"> Objeto que representa al instrumento </param>
	/// <returns> Monto por concepto de eventos o 0 si no se encuentra nada </returns>
	public static double GetPayoutByEvents( this IHoldingTerms holding, DateTime date, double price )
	{
		if ( holding.TypeId is TypeId.Equity or TypeId.Mxequity2 )
		{
			return DbZeus.Db.Events.GetPayoutByEvents( holding.HoldingId, date, price, holding.CurrencyId );
		}

		return 0;
	}

	public static double GetPayoutByEvents( this IHoldingIdProperty holding, DateTime date, double price )
	{
		return holding.GetHoldingTerms().GetPayoutByEvents( date, price );
	}
}
