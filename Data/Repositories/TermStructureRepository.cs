using RiskConsult.Data.Entities;
using RiskConsult.Data.Interfaces;
using System.Data;

namespace RiskConsult.Data.Repositories;

public interface ITermStructureRepository : IRepository<ITermStructureEntity>
{
	public ITermStructureEntity[] GetTermStructureEntities( DateTime date, int TermStructureId );
}

internal class TermStructureRepository( IUnitOfWork unitOfWork ) : DbRepository<ITermStructureEntity>( unitOfWork ), ITermStructureRepository
{
	public override IPropertyMap[] Properties { get; } =
	[
		new PropertyMap<TermStructureEntity>( nameof( TermStructureEntity.Date ), "dteDate", 0, true ),
		new PropertyMap<TermStructureEntity>( nameof( TermStructureEntity.TermStructureId ), "intTermStructureId", 1, true ),
		new PropertyMap<TermStructureEntity>( nameof( TermStructureEntity.Term ), "intTerm", 2, true ),
		new PropertyMap<TermStructureEntity>( nameof( TermStructureEntity.Value ), "dblValue", 3, false )
	];
	public override string TableName { get; } = "tblDATA_TermStructure";

	/// <summary> Obtiene una lista de entidadtes que representan una curva con nodos y tasas </summary>
	/// <param name="TermStructureId"> ID de la curva </param>
	/// <param name="date"> Fecha de la curva </param>
	/// <returns> Lista de entidades o nulo si no existe </returns>
	public ITermStructureEntity[] GetTermStructureEntities( DateTime date, int TermStructureId )
	{
		using IDbCommand command = UnitOfWork.CreateCommand();
		command.CommandText = $"SELECT * FROM {TableName} WHERE intTermStructureId = {TermStructureId} AND dteDate = @date ORDER BY intTerm ASC";

		IDbDataParameter dateParam = command.CreateParameter();
		dateParam.ParameterName = "@date";
		dateParam.Value = date;
		command.Parameters.Add( dateParam );

		return UnitOfWork.GetCommandEntities<TermStructureEntity>( command, Properties );
	}
}
