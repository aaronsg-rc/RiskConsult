namespace RiskConsult.Core;

public readonly struct CashFlowDate
{
	public double CashFlow { get; }

	public DateTime Date { get; }

	public int DaysToFlow { get; }

	public double DiscountRate { get; }

	public double PresentValue { get; }

	internal CashFlowDate( DateTime date, int daysToFlow, double discountRate, double cashFlow, double presentValue )
	{
		Date = date;
		DaysToFlow = daysToFlow;
		DiscountRate = discountRate;
		CashFlow = cashFlow;
		PresentValue = presentValue;
	}

	public override string ToString() => $"{Date.ToShortDateString()}|{PresentValue:F2}";
}