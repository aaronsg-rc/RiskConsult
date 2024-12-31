using RiskConsult.Data.Entities;
using RiskConsult.Enumerators;
using System.Data;

namespace RiskConsult.Data.Repositories;

/// <summary> Repositorio de la entidad tblMAP_Holdings </summary>
public interface IMapHoldingRepository : IRepository<IMapHoldingEntity>
{
	/// <summary> Obtiene la entidad de un holding en base al ID y tipo de ID </summary>
	IMapHoldingEntity? GetHoldingEntity( string holdingId, HoldingIdType idType );

	/// <summary> Obtiene el último ID disponible en base de datos </summary>
	/// <returns> Último ID disponible </returns>
	int GetNextId();
}

internal class MapHoldingRepository( IUnitOfWork unitOfWork ) : DbRepository<IMapHoldingEntity>( unitOfWork ), IMapHoldingRepository
{
	/// <summary> Propiedades de la entidad </summary>
	public override IPropertyMap[] Properties { get; } =
	[
		new PropertyMap<MapHoldingEntity>( nameof( MapHoldingEntity.InitialDate ), "dteStart", 0, true ),
		new PropertyMap<MapHoldingEntity>( nameof( MapHoldingEntity.FinalDate ), "dteEnd", 1, true ),
		new PropertyMap<MapHoldingEntity>( nameof( MapHoldingEntity.HoldingId ), "intholdingId", 2, true ),
		new PropertyMap<MapHoldingEntity>( nameof( MapHoldingEntity.Name ), "txtHoldingName", 3, true ),
		new PropertyMap<MapHoldingEntity>( nameof( MapHoldingEntity.Description ), "txtHoldingDescription", 4, false )
	];

	/// <summary> Nombre de la tabla </summary>
	public override string TableName { get; } = "tblMAP_Holdings";

	public IMapHoldingEntity? GetHoldingEntity( string holdingId, HoldingIdType idType )
	{
		var fieldName = idType switch
		{
			HoldingIdType.Description => "txtHoldingDescription",
			HoldingIdType.Ticker => "txtHoldingName",
			HoldingIdType.HoldingId => "intHoldingId",
			_ => throw new ArgumentOutOfRangeException( nameof( idType ), idType, null )
		};

		using IDbCommand command = UnitOfWork.CreateCommand();
		command.CommandText = $"SELECT * FROM {TableName} WHERE {fieldName} = @id";
		IDbDataParameter param = command.CreateParameter();
		param.ParameterName = "@id";
		param.Value = holdingId;
		command.Parameters.Add( param );

		return command.GetEntity<MapHoldingEntity>( Properties );
	}

	public int GetNextId()
	{
		using IDbCommand command = UnitOfWork.CreateCommand();
		command.CommandText = $"SELECT TOP 1 * FROM {TableName} ORDER BY intholdingId DESC";

		MapHoldingEntity? entity = command.GetEntity<MapHoldingEntity>( Properties );

		return entity == null ? 2000000 : entity.HoldingId + 1;
	}
}
