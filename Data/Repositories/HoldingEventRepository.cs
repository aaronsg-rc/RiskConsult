using RiskConsult.Data.Entities;
using RiskConsult.Data.Interfaces;
using System.Data;

namespace RiskConsult.Data.Repositories;

public interface IHoldingEventRepository : IRepository<IHoldingEventEntity>
{
	IHoldingEventEntity[] GetHoldingEventEntities( int holdingId );
}

internal class HoldingEventRepository( IUnitOfWork unitOfWork ) : DbRepository<IHoldingEventEntity>( unitOfWork ), IHoldingEventRepository
{
	public override IPropertyMap[] Properties { get; } =
	[
		new PropertyMap<HoldingEventEntity>(nameof(HoldingEventEntity.Date ), "dteDate", 0, true ),
		new PropertyMap<HoldingEventEntity>(nameof(HoldingEventEntity.HoldingId ), "intID", 1, true ),
		new PropertyMap<HoldingEventEntity>(nameof(HoldingEventEntity.EventId ), "intType", 2, true ),
		new PropertyMap<HoldingEventEntity>(nameof(HoldingEventEntity.Value ), "dblValue", 3, false )
	];

	public override string TableName { get; } = "tblDATA_HoldingEvents";

	public IHoldingEventEntity[] GetHoldingEventEntities( int holdingId )
	{
		using IDbCommand command = UnitOfWork.CreateCommand();
		command.CommandText = $"SELECT {string.Join( ',', Properties.Select( p => p.ColumnName ) )} FROM {TableName} WHERE intID = {holdingId};";

		return UnitOfWork.GetCommandEntities<HoldingEventEntity>( command, Properties );
	}
}
