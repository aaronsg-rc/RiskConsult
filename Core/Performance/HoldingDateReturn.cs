using RiskConsult.Enumerators;
using RiskConsult.Extensions;

namespace RiskConsult.Core.Performance;

/// <summary> Clase que me permite obtener el rendimiento de un instrumento para una fecha </summary>
public class HoldingDateReturn
{
	private static readonly Dictionary<(int, PriceSourceId, DateTime), HoldingDateReturn> _holdingsReturns = [];

	/// <summary> Rendimiento en puntos base del precio </summary>
	public virtual double BpsPriceReturn { get; }

	/// <summary> Rendimiento en $ por cambio de precio en la moneda base del instrumento </summary>
	public virtual double CashPriceReturn { get; }

	/// <summary> ID de la moneda del instrumento </summary>
	public CurrencyId CurrencyId => Terms.CurrencyId;

	/// <summary> Fecha en t del rendimiento </summary>
	public DateTime Date { get; }

	/// <summary> Fecha en t-1 del rendimiento </summary>
	public DateTime DateInitial { get; }

	/// <summary> Nombre del instrumento </summary>
	public string Description => Terms.Description;

	/// <summary> ID del instrumento </summary>
	public int HoldingId => Terms.HoldingId;

	/// <summary> Cantidad sobre precio que es agregada al precio en t por concepto de amortización </summary>
	public double PayoutAmortization { get; }

	/// <summary> Cantidad sobre precio que es agregada al precio en t por concepto de cupón </summary>
	public double PayoutCoupon { get; }

	/// <summary> Cantidad sobre precio que es agregada al precio en t por concepto de derechos </summary>
	public double PayoutDividend { get; }

	/// <summary> Precio base en t </summary>
	public double PriceClean { get; }

	/// <summary> Precio afectado por cupones, amortizaciones y dividendos en t </summary>
	public double PriceFinal { get; }

	/// <summary> Precio en t-1 </summary>
	public double PriceInitial { get; }

	/// <summary> ID del tipo de precios </summary>
	public PriceSourceId SourceID { get; }

	/// <summary> Términos y condiciones del instrumento </summary>
	public IHoldingTerms Terms { get; }

	/// <summary> Obtiene el rendimiento que el instrumento tuvo en la fecha solicitada </summary>
	/// <param name="tycs"> Términos y condiciones del instrumento </param>
	/// <param name="date"> Fecha en que se quiere obtener el rendimiento </param>
	/// <param name="sourceID"> ID del tipo de precios </param>
	internal HoldingDateReturn( IHoldingTerms tycs, DateTime date, PriceSourceId sourceID )
	{
		Terms = tycs;
		SourceID = sourceID;
		Date = date;
		DateInitial = date.GetBusinessDateAdd( DateUnit.Day, -1 );
		PriceClean = tycs.GetCleanPrice( date, sourceID );
		PriceInitial = tycs.GetCleanPrice( DateInitial, sourceID );
		PayoutAmortization = tycs.GetPayoutByAmortization( date );
		PayoutDividend = tycs.GetPayoutByEvents( date, PriceClean );
		PayoutCoupon = tycs.GetPayoutByCoupon( date );
		PriceFinal = PriceClean + PayoutAmortization + PayoutDividend + PayoutCoupon;
		BpsPriceReturn = PriceInitial == 0 || PriceFinal == 0 ? 0 :
			( ( PriceFinal / PriceInitial ) - 1 ) * 10000;
		CashPriceReturn = PriceInitial == 0 || PriceFinal == 0 ? 0 :
			 PriceFinal - PriceInitial;
	}

	/// <summary> Constructor a partir de las propiedades de otro objeto </summary>
	/// <param name="other"> Otro objeto en el que se va a clonar </param>
	protected HoldingDateReturn( HoldingDateReturn other )
	{
		Terms = other.Terms;
		SourceID = other.SourceID;
		Date = other.Date;
		DateInitial = other.DateInitial;
		PriceClean = other.PriceClean;
		PayoutAmortization = other.PayoutAmortization;
		PayoutDividend = other.PayoutDividend;
		PayoutCoupon = other.PayoutCoupon;
		PriceInitial = other.PriceInitial;
		PriceFinal = other.PriceFinal;
		BpsPriceReturn = other.BpsPriceReturn;
		CashPriceReturn = other.CashPriceReturn;
	}

	/// <summary> Obtiene el rendimiento que el instrumento tuvo en la fecha solicitada </summary>
	/// <param name="tycs"> Términos y condiciones del instrumento </param>
	/// <param name="date"> Fecha en que se quiere obtener el rendimiento </param>
	/// <param name="csZeus"> Cadena de conexión a Zeus </param>
	/// <param name="dbUser"> Cadena de conexión a User </param>
	/// <param name="sourceID"> ID del tipo de precios </param>
	/// <returns> Clase inicializada con rendimientos del día </returns>
	public static HoldingDateReturn GetHoldingDateReturn( IHoldingTerms tycs, DateTime date, PriceSourceId sourceID )
	{
		if ( _holdingsReturns.TryGetValue( (tycs.HoldingId, sourceID, date), out HoldingDateReturn? holdingReturn ) )
		{
			return holdingReturn;
		}

		holdingReturn = new HoldingDateReturn( tycs, date, sourceID );
		_holdingsReturns[ (tycs.HoldingId, sourceID, date) ] = holdingReturn;
		return holdingReturn;
	}

	/// <summary> Rendimiento de la fecha y nombre del instrumento </summary>
	public override string ToString() => $"{Date.ToShortDateString()}|{BpsPriceReturn:F2} bps]|{Description}";
}
