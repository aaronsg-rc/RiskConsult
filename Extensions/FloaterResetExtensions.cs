using RiskConsult.Data;
using RiskConsult.Data.Interfaces;

namespace RiskConsult.Extensions;

public static class FloaterResetExtensions
{
	public static double GetFloaterReset( this IHoldingIdProperty holdingId, DateTime date )
	{
		if ( holdingId.HoldingId >= 2000000 )
		{
			return 0;
		}

		return DbZeus.Db.FloaterResets.GetFloaterReset( holdingId, date );
	}

	/// <summary> Obtiene el monto pagado por el insrumento por concepto de cupón para la fecha solicitada </summary>
	/// <param name="tycs"> Términos y condiciones del instrumento </param>
	/// <param name="date"> Fecha del cúpón </param>
	/// <returns> Monto pagado por concepto de cupón o 0 si no se pago nada </returns>
	public static double GetPayoutByCoupon( this IHoldingIdProperty holdingId, DateTime date )
	{
		if ( holdingId.HoldingId >= 2000000 )
		{
			return 0;
		}

		return DbZeus.Db.FloaterResets.GetPayoutByCoupon( holdingId, date );
	}
}
