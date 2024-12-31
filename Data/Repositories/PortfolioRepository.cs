using RiskConsult.Data.Entities;
using System.Data;

namespace RiskConsult.Data.Repositories;

public interface IPortfolioRepository : IRepository<IPortfolioEntity>
{
	public void Delete( DateTime date, string name );

	IPortfolioEntity[] GetPortfolioEntities( DateTime date, string portfolio );
}

/// <summary> Clase que se encarga de extraer datos referentes a portafolios </summary>
internal class PortfolioRepository( IUnitOfWork unitOfWork ) : DbRepository<IPortfolioEntity>( unitOfWork ), IPortfolioRepository
{
	public override IPropertyMap[] Properties { get; } =
	[
		new PropertyMap<PortfolioEntity>(nameof(PortfolioEntity.Date ), "dteDate", 0, true ),
		new PropertyMap<PortfolioEntity>(nameof(PortfolioEntity.Name ), "txtPortfolioID", 1, true ),
		new PropertyMap<PortfolioEntity>(nameof(PortfolioEntity.HoldingId ), "intID", 2, true ),
		new PropertyMap<PortfolioEntity>(nameof(PortfolioEntity.Value ), "dblAmount", 3, false )
	];

	public override string TableName { get; } = "tblPortfolio";

	public void Delete( DateTime date, string name )
	{
		using IDbCommand command = UnitOfWork.CreateCommand();
		command.CommandText = $"DELETE FROM {TableName} WHERE dteDate = @date AND txtPortfolioID = @portfolio";

		IDbDataParameter dateParam = command.CreateParameter();
		dateParam.Value = date;
		dateParam.ParameterName = "@date";
		command.Parameters.Add( dateParam );

		IDbDataParameter portfolioParam = command.CreateParameter();
		portfolioParam.Value = name;
		portfolioParam.ParameterName = "@portfolio";
		command.Parameters.Add( portfolioParam );
		command.ExecuteNonQuery();
	}

	/// <summary> Obtiene una lista de entidades para el portafolio y fecha dada </summary>
	/// <param name="portfolio"> Nombre del portafolio </param>
	/// <param name="date"> Fecha del portafolio </param>
	/// <returns> Lista de entidades del portafolio </returns>
	public IPortfolioEntity[] GetPortfolioEntities( DateTime date, string portfolio )
	{
		using IDbCommand command = UnitOfWork.CreateCommand();
		command.CommandText = $"SELECT * FROM {TableName} WHERE dteDate = @date AND txtPortfolioID = @portfolio";

		IDbDataParameter dateParam = command.CreateParameter();
		dateParam.Value = date;
		dateParam.ParameterName = "@date";
		command.Parameters.Add( dateParam );

		IDbDataParameter portfolioParam = command.CreateParameter();
		portfolioParam.Value = portfolio;
		portfolioParam.ParameterName = "@portfolio";
		command.Parameters.Add( portfolioParam );

		return command.GetEntities<PortfolioEntity>( Properties );
	}
}
