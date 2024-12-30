using RiskConsult.Core;
using RiskConsult.Data;
using RiskConsult.Data.Interfaces;

namespace RiskConsult.Extensions;

public static class FloaterResetExtensions
{
	public static double GetFloaterReset( this IHoldingIdProperty holdingId, DateTime date )
	{
		return DbZeus.Db.FloaterResets.GetFloaterReset( holdingId.HoldingId, date );
	}

	/// <summary> Obtiene el monto pagado por el insrumento por concepto de cupón para la fecha solicitada </summary>
	/// <param name="tycs"> Términos y condiciones del instrumento </param>
	/// <param name="date"> Fecha del cúpón </param>
	/// <returns> Monto pagado por concepto de cupón o 0 si no se pago nada </returns>
	public static double GetPayoutByCoupon( this IHoldingTerms terms, DateTime date )
	{
		return DbZeus.Db.FloaterResets.GetPayoutByCoupon( terms, date );
	}

	public static double GetPayoutByCoupon( this IHoldingIdProperty holdingId, DateTime date )
	{
		return DbZeus.Db.FloaterResets.GetPayoutByCoupon( holdingId.GetHoldingTerms(), date );
	}
}
