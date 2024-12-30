using RiskConsult.Data.Interfaces;
using RiskConsult.Enumerators;

namespace RiskConsult.Data.Entities;

public interface IHoldingEventEntity : IDateProperty, IEventIdProperty, IHoldingIdProperty, IValueProperty<double>
{ }

/// <summary> tblDATA_HoldingEvents ( dteDate, intID, intType, dblValue, dblValue2 ) </summary>
public class HoldingEventEntity : IHoldingEventEntity
{
	/// <summary> dteDate </summary>
	public DateTime Date { get; set; }

	/// <summary> intType </summary>
	public EventId EventId { get; set; }

	/// <summary> intID </summary>
	public int HoldingId { get; set; }

	/// <summary> dblValue </summary>
	public double Value { get; set; }

	public override string ToString() => string.Join( '|', Date, HoldingId, EventId, Value );
}
