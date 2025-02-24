using RiskConsult.Core;
using RiskConsult.Enumerators;
using RiskConsult.Extensions;

namespace RiskConsult.Maths;

public static class ValuationModels
{
	public static double CalculateFixedCouponBondPrice( double faceValue, double couponRate, DateTime maturity, ITermStructure curve, int payFrequency, DateUnit payPeriod, int payDay = 0, int weekendDayAdjust = -1, int dayAct = 360 )
	{
		var price = 0.0;
		List<DateTime> calendar = CreatePaymentCalendar( curve.Date, maturity, payFrequency, payPeriod, payDay, weekendDayAdjust );
		var periodDays = payPeriod.ToDays();
		foreach ( DateTime date in calendar )
		{
			var days = curve.Date.Subtract( date ).Days;
			var rate = curve.GetTermValue( days );
			var timeFraction = payFrequency * periodDays / dayAct;
			var couponPayment = faceValue * couponRate * timeFraction;
			var discountFactor = 1 / Math.Pow( 1 + rate, timeFraction );
			price += couponPayment * discountFactor;

			if ( maturity.Equals( date ) )
			{
				price += faceValue * discountFactor;
			}
		}

		return price;
	}

	public static double CalculateZeroCouponBondPrice( double faceValue, double interestRate, int yearsToMaturity )
	{
		var discountFactor = 1.0 / Math.Pow( 1.0 + interestRate, yearsToMaturity );
		var price = faceValue * discountFactor;
		return price;
	}

	public static List<DateTime> CreatePaymentCalendar( DateTime date, DateTime maturity, int payFrequency, DateUnit payPeriod, int payDay = 0, int weekendDayAdjust = 0 )
	{
		ArgumentOutOfRangeException.ThrowIfNegative( payFrequency, nameof( payFrequency ) );
		ArgumentOutOfRangeException.ThrowIfNegative( payDay, nameof( payDay ) );

		if ( maturity < date )
		{
			return [];
		}
		else if ( payFrequency == 0 )
		{
			return [ maturity ];
		}

		// Genero calendario calculado desde fecha de vencimiento a fecha de valuación
		var calendar = new List<DateTime>();
		DateTime dteCalendar = maturity;
		calendar.Add( maturity );
		DateTime dtePay;
		do
		{
			// Obtengo la fecha que corresponde al pago y ajusto si son días fijos
			dteCalendar = dteCalendar.Add( payPeriod, -payFrequency );
			if ( payDay > 0 )
			{
				dteCalendar = new DateTime( dteCalendar.Year, dteCalendar.Month, payDay );
			}

			// Obtengo la fecha ajustada a dias habiles de pago de cupón
			dtePay = weekendDayAdjust > 0
				? dteCalendar.GetBusinessNextOrEqualsDay()
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

	public static int ToDays( this DateUnit unit )
	{
		return unit switch
		{
			DateUnit.Day => 1,
			DateUnit.Week => 7,
			DateUnit.Month => 30,
			DateUnit.Year => 360,
			_ => 0,
		};
	}
}
