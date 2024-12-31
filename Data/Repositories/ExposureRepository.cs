using RiskConsult.Data.Entities;
using System.Data;

namespace RiskConsult.Data.Repositories;

public interface IExposureRepository : IRepository<IExposureEntity>
{
	IExposureEntity[] GetExposureEntities( DateTime date, int exposureId, int holdingId );
}

/// <summary> Clase que permite extraer las exposiciones de instrumentos </summary>
internal class ExposureRepository( IUnitOfWork unitOfWork ) : DbRepository<IExposureEntity>( unitOfWork ), IExposureRepository
{
	public override IPropertyMap[] Properties { get; } =
		[
			new PropertyMap<ExposureEntity>(nameof(ExposureEntity.Date ), "dteDate", 0, true ),
			new PropertyMap<ExposureEntity>(nameof(ExposureEntity.ExposureId ), "intExposureID", 1, true ),
			new PropertyMap<ExposureEntity>(nameof(ExposureEntity.HoldingId ), "intID", 2, true ),
			new PropertyMap<ExposureEntity>(nameof(ExposureEntity.FactorId ), "intFactorId", 3, true ),
			new PropertyMap<ExposureEntity>(nameof(ExposureEntity.Value ), "dblValue", 4, false )
		];
	public override string TableName { get; } = "tblDATA_Exposures";

	/// <summary> Obtiene una lista de entidades de exposiciones de un instrumento para una fecha dada </summary>
	/// <param name="holdingId"> ID del instrumento </param>
	/// <param name="date"> Fecha de exposiciones </param>
	/// <returns> Lista de entidades que coincidan </returns>
	public IExposureEntity[] GetExposureEntities( DateTime date, int exposureId, int holdingId )
	{
		IDbCommand command = UnitOfWork.CreateCommand();
		command.CommandText = $"SELECT * FROM {TableName} WHERE dteDate = @date AND intExposureID = {exposureId} AND intId = {holdingId} AND dblValue <> 0";

		IDbDataParameter paramDate = command.CreateParameter();
		paramDate.ParameterName = "@date";
		paramDate.Value = date;
		command.Parameters.Add( paramDate );

		return command.GetEntities<ExposureEntity>( Properties );
	}
}
