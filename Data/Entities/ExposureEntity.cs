using RiskConsult.Data.Interfaces;
using RiskConsult.Enumerators;

namespace RiskConsult.Data.Entities;

public interface IExposureEntity : IDateProperty, IExposureIdProperty, IFactorIdProperty, IHoldingIdProperty, IValueProperty<double>
{ }

/// <summary> tblDATA_Exposures ( dteDate, intExposureID, intID, intFactorId, dblValue ) </summary>
public class ExposureEntity : IExposureEntity
{
	/// <summary> dteDate </summary>
	public DateTime Date { get; set; }

	/// <summary> intExposureID </summary>
	public int ExposureId { get; set; }

	/// <summary> intFactorId </summary>
	public FactorId FactorId { get; set; }

	/// <summary> intID </summary>
	public int HoldingId { get; set; }

	/// <summary> dblValue </summary>
	public double Value { get; set; }

	public override string ToString() => string.Join( '|', Date, HoldingId, FactorId, Value );
}
