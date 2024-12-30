using RiskConsult.Data.Entities;
using RiskConsult.Data.Interfaces;
using System.Data;

namespace RiskConsult.Data.Repositories;

public interface IScenarioRepository : IRepository<IScenarioEntity>
{
	void Delete( int scenarioId );

	IScenarioEntity[] GetScenarioEntities( int scenarioId );
}

internal class ScenarioRepository( IUnitOfWork unitOfWork ) : DbRepository<IScenarioEntity>( unitOfWork ), IScenarioRepository
{
	public override IPropertyMap[] Properties { get; } =
	[
		new PropertyMap<ScenarioEntity>( nameof( ScenarioEntity.ScenarioId ), "intScenarioID", 0, true ),
		new PropertyMap<ScenarioEntity>( nameof( ScenarioEntity.FactorId ), "intID", 1, true ),
		new PropertyMap<ScenarioEntity>( nameof( ScenarioEntity.Value ), "dblValue", 2, false )
	];

	public override string TableName { get; } = "tblDATA_Factors";

	public void Delete( int scenarioId )
	{
		using IDbCommand command = UnitOfWork.CreateCommand();
		command.CommandText = $"DELETE FROM {TableName} WHERE intScenarioID = {scenarioId}";

		UnitOfWork.ExecuteNonQuery( command );
	}

	public IScenarioEntity[] GetScenarioEntities( int scenarioId )
	{
		using IDbCommand command = UnitOfWork.CreateCommand();
		command.CommandText = $"SELECT * FROM {TableName} WHERE intScenarioID = {scenarioId}";

		return UnitOfWork.GetCommandEntities<ScenarioEntity>( command, Properties );
	}
}
