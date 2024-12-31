using RiskConsult.Data.Entities;
using System.Data;

namespace RiskConsult.Data.Repositories;

public interface ICustomHoldingRepository<T> : IRepository<ICustomHoldingEntity<T>>
{
	ICustomHoldingEntity<T>[] GetCustomHoldingEntities( int holdingId );
}

internal class CustomHoldingRepository<T> : DbRepository<ICustomHoldingEntity<T>>, ICustomHoldingRepository<T>
{
	public override IPropertyMap[] Properties { get; }

	public override string TableName { get; }

	public CustomHoldingRepository( IUnitOfWork unitOfWork ) : base( unitOfWork )
	{
		(var prefix, var sufix) = CustomHoldingRepository<T>.GetPrefixAndSufix();
		Properties = GetProperties( prefix );
		TableName = $"tblDTC_Holding_{sufix}";
	}

	public ICustomHoldingEntity<T>[] GetCustomHoldingEntities( int holdingId )
	{
		using IDbCommand command = UnitOfWork.CreateCommand();
		command.CommandText = $"SELECT * FROM {TableName} WHERE intID = {holdingId}";

		return command.GetEntities<CustomHoldingEntity<T>>( Properties );
	}

	private static (string, string) GetPrefixAndSufix()
	{
		Type type = typeof( T );
		return
			type.Equals( typeof( DateTime ) ) ? ("dte", "Date") :
			type.Equals( typeof( int ) ) ? ("int", "Integer") :
			type.Equals( typeof( double ) ) ? ("dbl", "Float") :
			type.Equals( typeof( string ) ) ? ("txt", "Text") :
			throw new Exception( $"Not supported type" );
	}

	private static IPropertyMap[] GetProperties( string prefix )
	{
		return
		[
			new PropertyMap<CustomHoldingEntity<T>>( nameof( CustomHoldingEntity<T>.Date ), "dteDate", 0, true ),
			new PropertyMap<CustomHoldingEntity<T>>( nameof( CustomHoldingEntity<T>.HoldingId ), "intID", 1, true ),
			new PropertyMap<CustomHoldingEntity<T>>( nameof( CustomHoldingEntity<T>.Parameter ), "txtParameter", 2, true ),
			new PropertyMap<CustomHoldingEntity<T>>( nameof( CustomHoldingEntity<T>.Value ), $"{prefix}Value", 3, false ),
		];
	}
}
