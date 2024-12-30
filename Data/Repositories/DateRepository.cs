using RiskConsult.Data.Entities;
using RiskConsult.Data.Interfaces;

namespace RiskConsult.Data.Repositories;

public interface IBusinessDaysRepository : IDateRepository
{ }

public interface IDateRepository : IRepository<IDateEntity>
{ }

public interface IHolidaysRepository : IDateRepository
{ }

internal class DateRepository( IUnitOfWork unitOfWork, string tableName ) : DbRepository<IDateEntity>( unitOfWork ), IBusinessDaysRepository, IHolidaysRepository
{
	public override IPropertyMap[] Properties { get; } =
	[
		new PropertyMap<DateEntity>( nameof( DateEntity.Date), "dteDate" )
	];

	public override string TableName { get; } = tableName;
}
