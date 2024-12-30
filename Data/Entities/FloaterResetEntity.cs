using RiskConsult.Data.Interfaces;

namespace RiskConsult.Data.Entities;

public interface IFloaterResetEntity : IDateProperty, IHoldingIdProperty, IValueProperty<double>
{ }

/// <summary> tblDATA_FloatersReset ( dteDate, intID, dblReset ) </summary>
public class FloaterResetEntity : IFloaterResetEntity
{
	/// <summary> dteDate </summary>
	public DateTime Date { get; set; }

	/// <summary> intID </summary>
	public int HoldingId { get; set; }

	/// <summary> dblReset </summary>
	public double Value { get; set; }

	public override string ToString() => string.Join( '|', Date, HoldingId, Value );
}
