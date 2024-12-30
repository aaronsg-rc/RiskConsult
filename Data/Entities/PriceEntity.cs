using RiskConsult.Data.Interfaces;
using RiskConsult.Enumerators;

namespace RiskConsult.Data.Entities;

public interface IPriceEntity : IDateProperty, IHoldingIdProperty, IPriceSourceIdProperty, IValueProperty<double>
{ }

/// <summary> tblDATA_Quotes ( dteDate, intSourceID, intID, dblQuote ) </summary>
public class PriceEntity : IPriceEntity
{
	/// <summary> dteDate </summary>
	public DateTime Date { get; set; }

	/// <summary> intID </summary>
	public int HoldingId { get; set; }

	/// <summary> intSourceID </summary>
	public PriceSourceId PriceSourceId { get; set; }

	/// <summary> dblQuote </summary>
	public double Value { get; set; }

	public override string ToString() => string.Join( '|', Date, PriceSourceId, HoldingId, Value );
}
