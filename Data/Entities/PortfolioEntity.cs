using RiskConsult.Data.Interfaces;

namespace RiskConsult.Data.Entities;

public interface IPortfolioEntity : IDateProperty, IHoldingIdProperty, INameProperty, IValueProperty<double>
{ }

/// <summary> tblPortfolio ( dteDate, txtPortfolioID, intID, dblAmount ) </summary>
public class PortfolioEntity : IPortfolioEntity
{
	/// <summary> dteDate </summary>
	public DateTime Date { get; set; }

	/// <summary> intID </summary>
	public int HoldingId { get; set; }

	/// <summary> txtPortfolioID </summary>
	public string Name { get; set; } = string.Empty;

	/// <summary> dblAmount </summary>
	public double Value { get; set; }

	public override string ToString() => string.Join( '|', Date, HoldingId, Name, Value );
}
