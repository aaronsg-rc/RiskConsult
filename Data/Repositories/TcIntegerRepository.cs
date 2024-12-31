using RiskConsult.Data.Entities;
using System.Data;

namespace RiskConsult.Data.Repositories;

public interface ITcIntegerRepository : IRepository<ITcIntegerEntity>
{
	ITcIntegerEntity[] GetTcIntegerGroup( int groupId );
}

internal class TcIntegerRepository( IUnitOfWork unitOfWork ) : DbRepository<ITcIntegerEntity>( unitOfWork ), ITcIntegerRepository
{
	public override IPropertyMap[] Properties { get; } =
	[
		new PropertyMap<TcIntegerEntity>(nameof(TcIntegerEntity.GroupId ), "intGroupID", 0, true ),
		new PropertyMap<TcIntegerEntity>(nameof(TcIntegerEntity.Id ), "intID", 1, true ),
		new PropertyMap<TcIntegerEntity>(nameof(TcIntegerEntity.Parameter ), "txtParameter", 2, true ),
		new PropertyMap<TcIntegerEntity>(nameof(TcIntegerEntity.Value ), "intValue", 3, false )
	];
	public override string TableName { get; } = "tblTC_Integer";

	public ITcIntegerEntity[] GetTcIntegerGroup( int groupId )
	{
		using IDbCommand command = UnitOfWork.CreateCommand();
		command.CommandText = $"SELECT * FROM tblTC_Integer WHERE intGroupID = {groupId}";

		return command.GetEntities<TcIntegerEntity>( Properties );
	}
}
