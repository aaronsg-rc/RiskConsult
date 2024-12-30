using RiskConsult.Data.Entities;
using RiskConsult.Data.Interfaces;
using RiskConsult.Enumerators;
using System.Data;

namespace RiskConsult.Data.Repositories;

public interface IMapHoldingRepository : IRepository<IMapHoldingEntity>
{
	IMapHoldingEntity? GetHoldingEntity( string holdingId, HoldingIdType idType );

	int GetNextId();
}

internal class MapHoldingRepository( IUnitOfWork unitOfWork ) : DbRepository<IMapHoldingEntity>( unitOfWork ), IMapHoldingRepository
{
	public override IPropertyMap[] Properties { get; } =
	[
		new PropertyMap<MapHoldingEntity>( nameof( MapHoldingEntity.InitialDate ), "dteStart", 0, true ),
		new PropertyMap<MapHoldingEntity>( nameof( MapHoldingEntity.FinalDate ), "dteEnd", 1, true ),
		new PropertyMap<MapHoldingEntity>( nameof( MapHoldingEntity.HoldingId ), "intholdingId", 2, true ),
		new PropertyMap<MapHoldingEntity>( nameof( MapHoldingEntity.Name ), "txtHoldingName", 3, true ),
		new PropertyMap<MapHoldingEntity>( nameof( MapHoldingEntity.Description ), "txtHoldingDescription", 4, false )
	];

	public override string TableName { get; } = "tblMAP_Holdings";

	public IMapHoldingEntity? GetHoldingEntity( string holdingId, HoldingIdType idType )
	{
		if ( idType is HoldingIdType.Ticker2 or HoldingIdType.Invalid or HoldingIdType.ISIN )
		{
			return null;
		}

		var fieldName =
			idType is HoldingIdType.Description ? "txtHoldingDescription" :
			idType == HoldingIdType.Ticker ? "txtHoldingName" :
			"intholdingId";

		using IDbCommand command = UnitOfWork.CreateCommand();
		command.CommandText = $"SELECT * FROM {TableName} WHERE {fieldName} = @id";
		IDbDataParameter param = command.CreateParameter();
		param.ParameterName = "@id";
		param.Value = holdingId;
		command.Parameters.Add( param );

		return UnitOfWork.GetCommandEntity<MapHoldingEntity>( command, Properties );
	}

	/// <summary> Obtiene el último ID disponible en base de datos </summary>
	/// <returns> Último ID disponible </returns>
	public int GetNextId()
	{
		using IDbCommand command = UnitOfWork.CreateCommand();
		command.CommandText = $"SELECT TOP 1 * FROM {TableName} ORDER BY intholdingId DESC";

		MapHoldingEntity? entity = UnitOfWork.GetCommandEntity<MapHoldingEntity>( command, Properties );

		return entity == null ? 2000000 : entity.HoldingId + 1;
	}
}
