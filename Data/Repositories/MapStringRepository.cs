using RiskConsult.Data.Entities;
using RiskConsult.Data.Interfaces;
using System.Data;

namespace RiskConsult.Data.Repositories;

public interface IMapStringRepository : IRepository<IMapStringEntity>
{
	void Delete( string groupID, int id );

	void Delete( string groupID, string name );

	/// <summary> Obtiene lista de entidades a partir del nombre del grupo </summary>
	/// <param name="groupID"> Nombre del grupo </param>
	/// <returns> Lista de entidades que coincidan con el grupo </returns>
	IMapStringEntity[] GetGroupEntities( string groupID );

	/// <summary> Obtiene la entidad en función del grupo y nombre de la entidad </summary>
	/// <param name="groupID"> Nombre del grupo </param>
	/// <param name="nameOrDescription"> Nombre o decripción de la entidad </param>
	/// <returns> Entidad encontrada o nulo si no existe </returns>
	IMapStringEntity? GetGroupEntity( string groupID, string nameOrDescription );

	IMapStringEntity? GetGroupEntity( string groupID, int id );

	/// <summary> Obtiene el siguiente ID disponible para el grupo solicitado </summary>
	/// <param name="groupID"> Nombre del grupo </param>
	/// <returns> ID próximo disponible del grupo solicitado </returns>
	public int GetNextId( string groupID );
}

internal class MapStringRepository( IUnitOfWork unitOfWork ) : DbRepository<IMapStringEntity>( unitOfWork ), IMapStringRepository
{
	public override IPropertyMap[] Properties { get; } =
	[
		new PropertyMap<MapStringEntity>( nameof( MapStringEntity.Id ), "intID", 0, true ),
		new PropertyMap<MapStringEntity>( nameof( MapStringEntity.GroupId ), "txtGroupID", 1, true ),
		new PropertyMap<MapStringEntity>( nameof( MapStringEntity.Name ), "txtName", 2, false ),
		new PropertyMap<MapStringEntity>( nameof( MapStringEntity.Description ), "txtDescription", 3, false ),
	];

	public override string TableName { get; } = "tblMAP_Strings";

	public void Delete( string groupID, int id )
	{
		using IDbCommand command = UnitOfWork.CreateCommand();
		command.CommandText = $"DELETE FROM {TableName} WHERE txtGroupID = @groupID AND intID = {id}";

		IDbDataParameter param = command.CreateParameter();
		param.ParameterName = "@groupID";
		param.Value = groupID;
		command.Parameters.Add( param );

		UnitOfWork.ExecuteNonQuery( command );
	}

	public void Delete( string groupID, string name )
	{
		using IDbCommand command = UnitOfWork.CreateCommand();
		command.CommandText = $"DELETE FROM {TableName} WHERE txtGroupID = @groupID AND txtName = @name";

		IDbDataParameter groupParam = command.CreateParameter();
		groupParam.ParameterName = "@groupID";
		groupParam.Value = groupID;
		command.Parameters.Add( groupParam );

		IDbDataParameter nameParam = command.CreateParameter();
		nameParam.ParameterName = "@name";
		nameParam.Value = name;
		command.Parameters.Add( nameParam );

		UnitOfWork.ExecuteNonQuery( command );
	}

	public IMapStringEntity[] GetGroupEntities( string groupID )
	{
		using IDbCommand command = UnitOfWork.CreateCommand();
		command.CommandText = $"SELECT * FROM {TableName} WHERE txtGroupID = @groupID";

		IDbDataParameter groupParam = command.CreateParameter();
		groupParam.ParameterName = "@groupID";
		groupParam.Value = groupID;
		command.Parameters.Add( groupParam );

		return UnitOfWork.GetCommandEntities<MapStringEntity>( command, Properties );
	}

	public IMapStringEntity? GetGroupEntity( string groupID, string nameOrDescription )
	{
		using IDbCommand command = UnitOfWork.CreateCommand();
		command.CommandText = $"SELECT * FROM {TableName} WHERE txtGroupID = @groupID AND (txtName = @name OR txtDescription = @name)";

		IDbDataParameter groupParam = command.CreateParameter();
		groupParam.ParameterName = "@groupID";
		groupParam.Value = groupID;
		command.Parameters.Add( groupParam );

		IDbDataParameter nameParam = command.CreateParameter();
		nameParam.ParameterName = "@name";
		nameParam.Value = nameOrDescription;
		command.Parameters.Add( nameParam );

		return UnitOfWork.GetCommandEntity<MapStringEntity>( command, Properties );
	}

	public IMapStringEntity? GetGroupEntity( string groupID, int id )
	{
		using IDbCommand command = UnitOfWork.CreateCommand();
		command.CommandText = $"SELECT * FROM {TableName} WHERE txtGroupID = @groupID AND intID = {id}";

		IDbDataParameter groupParam = command.CreateParameter();
		groupParam.ParameterName = "@groupID";
		groupParam.Value = groupID;
		command.Parameters.Add( groupParam );

		return UnitOfWork.GetCommandEntity<MapStringEntity>( command, Properties );
	}

	public int GetNextId( string groupID )
	{
		using IDbCommand command = UnitOfWork.CreateCommand();
		command.CommandText = $"SELECT TOP 1 * FROM {TableName} WHERE txtGroupID = @groupID ORDER BY intID DESC";

		IDbDataParameter groupParam = command.CreateParameter();
		groupParam.ParameterName = "@groupID";
		groupParam.Value = groupID;
		command.Parameters.Add( groupParam );

		MapStringEntity? entity = UnitOfWork.GetCommandEntity<MapStringEntity>( command, Properties );
		return entity == null ? 2000001 : entity.Id + 1;
	}
}
