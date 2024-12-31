using RiskConsult.Data.Entities;
using System.Data;

namespace RiskConsult.Data.Repositories;

public interface IFactorCumulativeRepository : IFactorRepository
{ }

public interface IFactorRepository : IRepository<IFactorEntity>
{
	IFactorEntity[] GetFactorEntities( DateTime date );
}

public interface IFactorReturnRepository : IFactorRepository
{ }

public interface IFactorValueRepository : IFactorRepository
{ }

internal class FactorRepository( IUnitOfWork unitOfWork, string tableName ) : DbRepository<IFactorEntity>( unitOfWork ), IFactorReturnRepository, IFactorValueRepository, IFactorCumulativeRepository
{
	public override IPropertyMap[] Properties { get; } =
	[
		new PropertyMap<FactorEntity>( nameof( FactorEntity.Date ), "dteDate", 0, true ),
		new PropertyMap<FactorEntity>( nameof( FactorEntity.FactorId ), "intID", 1, true ),
		new PropertyMap<FactorEntity>( nameof( FactorEntity.Value ), "dblValue", 2, false )
	];

	public override string TableName { get; } = tableName;

	/// <summary> Obtiene las entidades de los factores para la fecha que se defina </summary>
	/// <param name="date"> Fecha de los factores </param>
	public IFactorEntity[] GetFactorEntities( DateTime date )
	{
		using IDbCommand command = UnitOfWork.CreateCommand();
		command.CommandText = $"SELECT * FROM {TableName} WHERE dteDate = @date";

		IDbDataParameter dateParam = command.CreateParameter();
		dateParam.ParameterName = "@date";
		dateParam.Value = date;
		command.Parameters.Add( dateParam );

		return command.GetEntities<FactorEntity>( Properties );
	}
}
