using RiskConsult.Data;
using RiskConsult.Enumerators;
using RiskConsult.Extensions;

namespace RiskConsult.Core;

public static class CashFlowsCalculator
{
	/// <summary> Obtiene una lista de los cashflows según los términos y condiciones del instrumento </summary>
	/// <param name="discountCurve"> Implementación de lafecha y curva de descuento </param>
	/// <param name="interval"> Periodicidad </param>
	/// <param name="frequency"> Cada cuantos periodos paga </param>
	/// <param name="nominal"> Nominal del instrumento </param>
	/// <param name="couponRate"> Tasa del cupón del instrumento </param>
	/// <param name="frequency"> Frecuencia del pago de cupón </param>
	/// <param name="payCalendar"> Enumerable con las fechas de pago </param>
	/// <returns> Lista de los cashflows </returns>
	public static List<CashFlowDate> GetCashFlows( ITermStructure discountCurve, IEnumerable<DateTime> payCalendar, double nominal,
		double couponRate, int frequency, DateUnit interval )
	{
		DateTime maturity = payCalendar.Max();
		var freqDays = frequency * (
			interval == DateUnit.Day ? 1 :
			interval == DateUnit.Week ? 7 :
			interval == DateUnit.Month ? 30 :
			interval == DateUnit.Year ? 360 : -1 );
		var freqYearly = freqDays / 360.0;
		var cashFlows = new List<CashFlowDate>();
		foreach ( DateTime flowDate in payCalendar )
		{
			var daysToFlow = flowDate.Subtract( discountCurve.Date ).Days;
			var discountRate = discountCurve.GetTermValue( daysToFlow );
			var flowValue = nominal * couponRate * freqYearly;
			if ( flowDate == maturity )
			{
				flowValue += nominal;
			}

			var presentValue = flowValue / Math.Pow( 1.0 + discountRate, daysToFlow / 360.0 );
			cashFlows.Add( new CashFlowDate( flowDate, daysToFlow, discountRate, flowValue, presentValue ) );
		}

		return cashFlows;
	}

	public static List<CashFlowDate> GetCashFlows( ITermStructure discountCurve, IEnumerable<DateTime> payCalendar, double nominal,
		Dictionary<DateTime, double> couponCalendar, int frequency, DateUnit interval )
	{
		DateTime maturity = payCalendar.Max();
		var freqDays = frequency * (
			interval == DateUnit.Day ? 1 :
			interval == DateUnit.Week ? 7 :
			interval == DateUnit.Month ? 30 :
			interval == DateUnit.Year ? 360 : -1 );
		var freqYearly = freqDays / 360.0;
		var cashFlows = new List<CashFlowDate>();
		foreach ( DateTime flowDate in payCalendar )
		{
			var daysToFlow = flowDate.Subtract( discountCurve.Date ).Days;
			var discountRate = discountCurve.GetTermValue( daysToFlow );
			var couponRate = couponCalendar.TryGetValue( flowDate, out var value ) ? value : 0;
			var flowValue = nominal * couponRate * freqYearly;
			if ( flowDate == maturity )
			{
				flowValue += nominal;
			}

			// flowValue / Math.Pow( 1.0 + discountRate, daysToFlow / 360.0 ); flowValue * Math.Exp( -discountRate * daysToFlow / 360.0 ) ;
			var presentValue = flowValue * Math.Exp( -discountRate * daysToFlow / 360.0 );
			cashFlows.Add( new CashFlowDate( flowDate, daysToFlow, discountRate, flowValue, presentValue ) );
		}

		return cashFlows;
	}

	/// <summary> Obtiene una lista de los cashflows según los términos y condiciones del instrumento </summary>
	/// <param name="tycs"> Objeto que representa al instrumento </param>
	/// <param name="date"> Fecha de valuación </param>
	/// <param name="csZeus"> Cadena de conexión a Zeus </param>
	/// <returns> Lista de los cashflows </returns>
	public static List<CashFlowDate> GetCashFlows( this IHoldingTerms tycs, DateTime date )
	{
		Dictionary<DateTime, double> couponCalendar = DbZeus.Db.FloaterResets.GetFloaterResetCalendar( tycs );
		List<DateTime> calendar = GetPaymentCalendar( date, tycs.Maturity, tycs.PayFrequency, tycs.PeriodId.ToDateUnit(),
			tycs.PayDay, tycs.WeekDayAdjust );
		ITermStructure curve = tycs.GetTermStructure( date );
		return GetCashFlows( curve, calendar, tycs.Nominal, couponCalendar, tycs.PayFrequency, tycs.PeriodId.ToDateUnit() );
	}

	/// <summary> Obtiene una lista de los cashflows según los términos y condiciones del instrumento </summary>
	/// <param name="holding"> Objeto que representa al instrumento </param>
	/// <param name="date"> Fecha de valuación </param>
	/// <param name="csZeus"> Cadena de conexión a Zeus </param>
	/// <returns> Lista de los cashflows </returns>
	public static List<CashFlowDate> GetCashFlows( this Holding holding, DateTime date )
		=> GetCashFlows( holding.Terms, date );

	/// <summary> Genera calendario de fechas futuras en las que pagará el instrumento </summary>
	/// <param name="csZeus"> Cadena de conexión a base de datos de Zeus </param>
	/// <param name="date"> Fecha en la que se evalua el instrumento </param>
	/// <param name="tycs"> Términos y condiciones del instrumento </param>
	public static List<DateTime> GetPaymentCalendar( this IHoldingTerms tycs, DateTime date )
		=> GetPaymentCalendar( date, tycs.Maturity, tycs.PayFrequency, tycs.PeriodId.ToDateUnit(), tycs.PayDay, tycs.WeekDayAdjust );

	/// <summary> Genera calendario de fechas futuras en las que pagará el instrumento </summary>
	/// <param name="csZeus"> Cadena de conexión a base de datos de Zeus </param>
	/// <param name="date"> Fecha en la que se evalua el instrumento </param>
	/// <param name="holding"> Instrumento del que se quiere calcular su calendario de pago </param>
	public static List<DateTime> GetPaymentCalendar( this Holding holding, DateTime date )
		=> holding.Terms.GetPaymentCalendar( date );

	/// <summary> Genera calendario de fechas futuras en las que pagará el instrumento </summary>
	/// <param name="csZeus"> Cadena de conexión a base de datos de Zeus </param>
	/// <param name="date"> Fecha en la que se evalua el instrumento </param>
	/// <param name="maturity"> Fecha de vencimiento </param>
	/// <param name="interval"> Periodicidad </param>
	/// <param name="frequency"> Cada cuantos periodos paga </param>
	/// <param name="payDay"> Día del mes en que se paga el cupón, si es 0 se calcula con payFrequency </param>
	/// <param name="weekendDayAdjust"> Si el pago cae en fin de semana se ajusta al anterior o siguiente día hábiles </param>
	public static List<DateTime> GetPaymentCalendar( DateTime date, DateTime maturity, int frequency, DateUnit interval,
		int payDay = 0, int weekendDayAdjust = -1 )
	{
		var calendar = new List<DateTime>();
		if ( maturity < date )
		{
			return calendar;
		}

		// Genero calendario calculado desde fecha de vencimiento a fecha de valuación
		DateTime dteCalendar = maturity;
		calendar.Add( maturity );
		if ( interval == DateUnit.Invalid )
		{
			return calendar;
		}

		DateTime dtePay;
		do
		{
			// Obtengo la fecha que corresponde al pago y ajusto si son días fijos
			dteCalendar = dteCalendar.Add( interval, -frequency );
			if ( payDay > 0 )
			{
				dteCalendar = new DateTime( dteCalendar.Year, dteCalendar.Month, payDay );
			}

			// Obtengo la fecha ajustada a dias habiles de pago de cupón
			dtePay = weekendDayAdjust > 0
				? dteCalendar.GetNextOrEqualsBusinessDay()
				: dteCalendar.GetBusinessPreviousOrEqualsDay();

			//Agrego
			if ( dtePay >= date )
			{
				calendar.Add( dtePay );
			}
		} while ( dtePay > date );
		calendar.Sort();

		return calendar;
	}

	/// <summary> Obtiene el valor presente basado en los términos y condiciones </summary>
	/// <param name="holding"> Instrumento con términos y condiciones </param>
	/// <param name="date"> Fecha de valuación </param>
	/// <param name="csZeus"> Cadena de conexión a base de datos de Zeus </param>
	/// <returns> Valor presente del instrumento o 0 si su fecha de valuación no es válida </returns>
	public static double GetPresentValue( this Holding holding, DateTime date )
		=> GetPresentValue( holding.Terms, date );

	/// <summary> Obtiene el valor presente basado en los términos y condiciones </summary>
	/// <param name="tycs"> Términos y condiciones del instrumento </param>
	/// <param name="date"> Fecha de valuación </param>
	/// <param name="csZeus"> Cadena de conexión a base de datos de Zeus </param>
	/// <returns> Valor presente del instrumento o 0 si su fecha de valuación no es válida </returns>
	public static double GetPresentValue( this IHoldingTerms tycs, DateTime date ) => tycs.Maturity < date ? 0 : tycs.GetCashFlows( date ).Sum( c => c.PresentValue );
}
