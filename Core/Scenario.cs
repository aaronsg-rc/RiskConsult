using RiskConsult.Data.Interfaces;

namespace RiskConsult.Core;

public interface IScenario : IDateProperty, IScenarioIdProperty, INameProperty, IDescriptionProperty
{
	List<IScenarioFactor> Factors { get; set; }
}

/// <summary> Clase que permite la creación de un escenario en base de datos de Usuario </summary>
public class Scenario : IScenario
{
	public DateTime Date { get; set; }
	public string Description { get; set; } = string.Empty;
	public List<IScenarioFactor> Factors { get; set; } = [];
	public string Name { get; set; } = string.Empty;
	public int ScenarioId { get; set; } = -1;
}
