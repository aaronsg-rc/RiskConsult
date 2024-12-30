using RiskConsult.Data.Entities;
using RiskConsult.Data.Interfaces;
using System.Data;

namespace RiskConsult.Data.Repositories;

public interface IFloaterResetRepository : IRepository<IFloaterResetEntity>
{
	IFloaterResetEntity[] GetFloaterResetEntities( int holdingId );
}

internal class FloaterResetRepository( IUnitOfWork unitOfWork ) : DbRepository<IFloaterResetEntity>( unitOfWork ), IFloaterResetRepository
{
	private readonly Dictionary<int, FloaterResetEntity[]> _cache = [];

	public override IPropertyMap[] Properties { get; } =
	[
		new  PropertyMap<FloaterResetEntity>(nameof(FloaterResetEntity.Date ), "dteDate", 0, true ),
		new  PropertyMap<FloaterResetEntity>(nameof(FloaterResetEntity.HoldingId ), "intID", 1, true ),
		new  PropertyMap<FloaterResetEntity>(nameof(FloaterResetEntity.Value ), "dblReset", 2, false )
	];

	public override string TableName { get; } = "tblDATA_FloatersReset";

	/// <summary> Obtiene lista de entidades para el instrumento solicitado </summary>
	/// <param name="holdingId"> ID del instruimento </param>
	/// <returns> Lista de entidades para el ID del instrumento </returns>
	public IFloaterResetEntity[] GetFloaterResetEntities( int holdingId )
	{
		if ( _cache.TryGetValue( holdingId, out FloaterResetEntity[]? entities ) )
		{
			return entities;
		}

		using IDbCommand command = UnitOfWork.CreateCommand();
		command.CommandText = $"SELECT dteDate, intID, dblReset FROM {TableName} WHERE intID = {holdingId}";

		return _cache[ holdingId ] = UnitOfWork.GetCommandEntities<FloaterResetEntity>( command, Properties );
	}
}
