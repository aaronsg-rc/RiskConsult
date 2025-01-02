using RiskConsult.Data.Entities;
using System.Data;

namespace RiskConsult.Data.Repositories;

public interface IBusinessDaysRepository : IDateRepository
{ }

public interface IDateRepository : IRepository<IDateEntity>
{
	DateTime GetZeusEndDate();

	DateTime GetZeusStartDate();
}

public interface IHolidaysRepository : IDateRepository
{ }

internal class DateRepository( IUnitOfWork unitOfWork, string tableName ) : DbRepository<IDateEntity>( unitOfWork ), IBusinessDaysRepository, IHolidaysRepository
{
	public override IPropertyMap[] Properties { get; } =
	[
		new PropertyMap<DateEntity>( nameof( DateEntity.Date), "dteDate" )
	];

	public override string TableName { get; } = tableName;

	public DateTime GetZeusEndDate()
	{
		using IDbCommand command = unitOfWork.CreateCommand();
		command.CommandText = $"SELECT dteEndDate FROM tblDates";

		DateEntity? entity = command.GetEntity<DateEntity>( [ new PropertyMap<DateEntity>( nameof( DateEntity.Date ), "dteEndDate" ) ] );
		return entity?.Date ?? DateTime.MaxValue;
	}

	public DateTime GetZeusStartDate()
	{
		using IDbCommand command = unitOfWork.CreateCommand();
		command.CommandText = $"SELECT dteStartDate FROM tblDates";

		DateEntity? entity = command.GetEntity<DateEntity>( [ new PropertyMap<DateEntity>( nameof( DateEntity.Date ), "dteStartDate" ) ] );
		return entity?.Date ?? DateTime.MinValue;
	}
}
