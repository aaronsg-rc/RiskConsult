namespace RiskConsult.Core;

/// <summary> Contiene los valores del factor de riesgo utilizados para la creación de escenarios </summary>
public interface IScenarioFactor : IFactorValue
{
	/// <summary> Valor incial del factor (mostrado en Zeus) </summary>
	public double InitialValue { get; set; }
}

public class ScenarioFactor : IScenarioFactor
{
	/// <summary> Factor base </summary>
	public IFactor Factor { get; set; } = new Factor();

	public double InitialValue { get; set; }

	/// <summary> Valor final del factor (mostrado en Zeus) </summary>
	public double Value { get; set; }

	public override string ToString() => $"{InitialValue:F4}|{Value:F4}|{Factor.Description}";
}
