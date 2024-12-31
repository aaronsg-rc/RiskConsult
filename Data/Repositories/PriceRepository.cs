using RiskConsult.Data.Entities;
using System.Data;

namespace RiskConsult.Data.Repositories;

public interface IPriceRepository : IRepository<IPriceEntity>
{
	IPriceEntity[] GetPriceEntities( DateTime date, int PriceSourceId );
}

internal class PriceRepository( IUnitOfWork unitOfWork ) : DbRepository<IPriceEntity>( unitOfWork ), IPriceRepository
{
	public override IPropertyMap[] Properties { get; } =
	[
		new  PropertyMap<PriceEntity>( nameof( PriceEntity.Date ), "dteDate", 0, true ),
		new  PropertyMap<PriceEntity>( nameof( PriceEntity.PriceSourceId ), "intSourceID", 1, true ),
		new  PropertyMap<PriceEntity>( nameof( PriceEntity.HoldingId ), "intID", 2, true ),
		new  PropertyMap<PriceEntity>( nameof( PriceEntity.Value ), "dblQuote", 3, false )
	];

	public override string TableName { get; } = "tblDATA_Quotes";

	public IPriceEntity[] GetPriceEntities( DateTime date, int sourceID )
	{
		using IDbCommand command = UnitOfWork.CreateCommand();
		command.CommandText = $"SELECT * FROM {TableName} WHERE dteDate = @date AND intSourceID = {sourceID}";

		IDbDataParameter dateParam = command.CreateParameter();
		dateParam.Value = date;
		dateParam.ParameterName = "@date";
		command.Parameters.Add( dateParam );

		return command.GetEntities<PriceEntity>( Properties );
	}
}
