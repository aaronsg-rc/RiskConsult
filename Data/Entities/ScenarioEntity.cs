using RiskConsult.Data.Interfaces;
using RiskConsult.Enumerators;

namespace RiskConsult.Data.Entities;

public interface IScenarioEntity : IFactorIdProperty, IScenarioIdProperty, IValueProperty<double>
{ }

/// <summary> tblDATA_Factors ( intScenarioID, intID, dblValue ) </summary>
public class ScenarioEntity : IScenarioEntity
{
	/// <summary> intID </summary>
	public FactorId FactorId { get; set; }

	/// <summary> intScenarioID </summary>
	public int ScenarioId { get; set; }

	/// <summary> dblValue </summary>
	public double Value { get; set; }

	public override string ToString() => string.Join( '|', ScenarioId, FactorId, Value );
}
