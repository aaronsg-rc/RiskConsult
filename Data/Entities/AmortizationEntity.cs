using RiskConsult.Data.Interfaces;

namespace RiskConsult.Data.Entities;

public interface IAmortizationEntity : IDateProperty, IHoldingIdProperty, IValueProperty<double>
{ }

/// <summary> tblDATA_CashFlows ( intID, dteDate, dblInterest, dblCap, dblAmortization, dblPrincipal, dblFloaterSpread, dblParameter ) </summary>
public class AmortizationEntity : IAmortizationEntity
{
	/// <summary> dteDate </summary>
	public DateTime Date { get; set; }

	/// <summary> intID </summary>
	public int HoldingId { get; set; }

	/// <summary> dblAmortization </summary>
	public double Value { get; set; }

	public override string ToString() => string.Join( '|', HoldingId, Date, Value );
}
