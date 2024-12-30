using RiskConsult.Core;
using RiskConsult.Data;
using RiskConsult.Enumerators;

namespace RiskConsult.Extensions;

public static class ScenarioExtensions
{
	public static void AddBps( this IScenarioFactor scenarioFactor, double value )
	{
		ArgumentNullException.ThrowIfNull( DbZeus.Db.User, nameof( DbZeus.Db.User ) );
		DbZeus.Db.User.Scenarios.AddBps( scenarioFactor, value );
	}

	public static IScenarioFactor GetScenarioFactor( this IFactor factor, DateTime date )
	{
		ArgumentNullException.ThrowIfNull( DbZeus.Db.User, nameof( DbZeus.Db.User ) );
		return DbZeus.Db.User.Scenarios.GetScenarioFactor( factor, date );
	}

	public static void SaveToDatabase( this IScenario scenario )
	{
		ArgumentNullException.ThrowIfNull( DbZeus.Db.User, nameof( DbZeus.Db.User ) );
		DbZeus.Db.User.Scenarios.SaveScenarioToDataBase( scenario );
	}

	public static void SetBpsShockToCurve( this IScenario scenario, TermStructureId curveType, double value )
	{
		ArgumentNullException.ThrowIfNull( DbZeus.Db.User, nameof( DbZeus.Db.User ) );
		DbZeus.Db.User.Scenarios.SetScenarioBpsShockToCurve( scenario, curveType, value );
	}

	public static void SetBpsShockToFactorGroup( this IScenario scenario, FactorTypeId factorType, double value )
	{
		ArgumentNullException.ThrowIfNull( DbZeus.Db.User, nameof( DbZeus.Db.User ) );
		DbZeus.Db.User.Scenarios.SetScenarioBpsShockToFactorGroup( scenario, factorType, value );
	}

	public static void SetInitialValue( this IScenarioFactor scenarioFactor, DateTime date )
	{
		ArgumentNullException.ThrowIfNull( DbZeus.Db.User, nameof( DbZeus.Db.User ) );
		scenarioFactor.InitialValue = DbZeus.Db.User.Scenarios.GetScenarioInitialValue( scenarioFactor.Factor, date );
	}

	public static void SetScenarioBpsShockToAll( this IScenario scenario, double value )
	{
		ArgumentNullException.ThrowIfNull( DbZeus.Db.User, nameof( DbZeus.Db.User ) );
		DbZeus.Db.User.Scenarios.SetScenarioBpsShockToAll( scenario, value );
	}

	public static void SetValueToCurve( this IScenario scenario, TermStructureId curveType, double value )
	{
		ArgumentNullException.ThrowIfNull( DbZeus.Db.User, nameof( DbZeus.Db.User ) );
		DbZeus.Db.User.Scenarios.SetScenarioValueToCurve( scenario, curveType, value );
	}

	public static void SetValueToFactorGroup( this IScenario scenario, FactorTypeId factorType, double value )
	{
		ArgumentNullException.ThrowIfNull( DbZeus.Db.User, nameof( DbZeus.Db.User ) );
		DbZeus.Db.User.Scenarios.SetScenarioValueToFactorGroup( scenario, factorType, value );
	}
}
