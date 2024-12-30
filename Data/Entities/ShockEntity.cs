using RiskConsult.Data.Interfaces;
using RiskConsult.Enumerators;

namespace RiskConsult.Data.Entities;

public interface IShockEntity : IFactorIdProperty, IShockIdProperty, IValueProperty<double>
{ }

/// <summary> tblDATA_Shock ( intShockID, intID, dblValue ) </summary>
public class ShockEntity : IShockEntity
{
	/// <summary> intID </summary>
	public FactorId FactorId { get; set; }

	/// <summary> intShockID </summary>
	public int ShockId { get; set; }

	/// <summary> dblValue </summary>
	public double Value { get; set; }

	public override string ToString() => string.Join( '|', ShockId, FactorId, Value );
}
