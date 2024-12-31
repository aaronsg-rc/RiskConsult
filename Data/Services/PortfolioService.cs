using RiskConsult.Core;
using RiskConsult.Data.Entities;
using RiskConsult.Data.Repositories;
using RiskConsult.Enumerators;
using RiskConsult.Extensions;
using System.Data;

namespace RiskConsult.Data.Services;

public interface IPortfolioService
{
	IPortfolio CreateNewPortfolio();

	int GetNextId();

	IPortfolio? GetPortfolio( DateTime date, string name );

	IPortfolio? GetPortfolio( DateTime date, string name, PriceSourceId sourceId, CurrencyId fxCurrency );

	int GetPortfolioId( string name );

	public void SavePortfolioToDatabase( IPortfolio portfolio );
}

internal class PortfolioService( IMapStringRepository mapStringRepository, IPortfolioRepository portfolioRepository, IUnitOfWork unitOfWork ) : IPortfolioService
{
	private readonly Dictionary<(DateTime, string), IPortfolio> _portfolios = [];

	public IPortfolio CreateNewPortfolio()
	{
		return new Portfolio();
	}

	public int GetNextId()
	{
		return mapStringRepository.GetNextId( "Benchmark" );
	}

	public IPortfolio? GetPortfolio( DateTime date, string name )
	{
		return GetPortfolio( date, name, PriceSourceId.Invalid, CurrencyId.Invalid );
	}

	public IPortfolio? GetPortfolio( DateTime date, string name, PriceSourceId sourceId, CurrencyId fxCurrency )
	{
		if ( _portfolios.TryGetValue( (date, name), out IPortfolio? portfolio ) )
		{
			return portfolio.Clone();
		}

		IMapStringEntity? map = mapStringRepository.GetGroupEntity( "Benchmark", name );
		if ( map == null )
		{
			return null;
		}

		portfolio = new Portfolio
		{
			Date = date,
			PortfolioId = name,
			Id = map.Id,
			CurrencyId = fxCurrency,
			PriceSourceId = sourceId
		};

		IPortfolioEntity[] entities = portfolioRepository.GetPortfolioEntities( date, name );
		foreach ( IPortfolioEntity entity in entities )
		{
			IHolding holding;
			if ( sourceId != PriceSourceId.Invalid && fxCurrency != CurrencyId.Invalid )
			{
				holding = entity.GetHolding( date, sourceId, fxCurrency, entity.Value );
			}
			else
			{
				holding = entity.GetHolding( entity.Value );
			}

			portfolio.Holdings.Add( holding );
		}

		_portfolios[ (date, name) ] = portfolio;

		return portfolio.Clone();
	}

	public int GetPortfolioId( string name )
	{
		return mapStringRepository.GetGroupEntity( "Benchmark", name )?.Id ?? -1;
	}

	public void SavePortfolioToDatabase( IPortfolio portfolio )
	{
		var map = new MapStringEntity
		{
			Id = portfolio.Id,
			GroupId = "Benchmark",
			Name = portfolio.PortfolioId,
			Description = portfolio.PortfolioId
		};

		var ports = new List<PortfolioEntity>();
		foreach ( IHolding hold in portfolio.Holdings )
		{
			var holdEntity = new PortfolioEntity
			{
				Date = portfolio.Date,
				Name = portfolio.PortfolioId,
				HoldingId = hold.HoldingId,
				Value = hold.Amount,
			};

			ports.Add( holdEntity );
		}

		unitOfWork.BeginTransaction();
		mapStringRepository.Delete( "Benchmark", portfolio.Id );
		mapStringRepository.Delete( "Benchmark", portfolio.PortfolioId );
		mapStringRepository.Insert( map );
		portfolioRepository.Delete( portfolio.Date, portfolio.PortfolioId );
		portfolioRepository.Insert( ports.ToArray() );

		using IDbCommand command = unitOfWork.CreateCommand();
		command.CommandText =
			$"DELETE FROM tblDATA_PortfolioSettings where dteDate = @date and intPortfolioID = @id;" +
			$"DELETE FROM tblParameter_Text WHERE txtGroup='Benchmark' and intID=@id;" +
			$"DELETE FROM tblParameter_Float WHERE txtGroup='Benchmark' and intID=@id;" +
			$"DELETE FROM tblParameter_Date WHERE txtGroup='Benchmark' and intID=@id;" +
			$"DELETE FROM tblParameter_Integer WHERE txtGroup='Benchmark' and intID=@id;" +
			$"DELETE FROM tblParameter_IntVector WHERE txtGroup='Benchmark' and intID=@id;" +
			$"DELETE FROM tblParameter_DoubleVector WHERE txtGroup='Benchmark' and intID=@id;" +
			$"DELETE FROM tblParameter_IntegerMap WHERE txtGroup='Benchmark' and intID=@id;" +
			$"DELETE FROM tblParameter_DoubleMap WHERE txtGroup='Benchmark' and intID=@id;" +
			$"INSERT INTO tblDATA_PortfolioSettings values( @id, @date, 1.000000);" +
			$"INSERT INTO tblParameter_Float values ( @id, 'Benchmark', 'AmountType', 0.000000)";

		IDbDataParameter dateParam = command.CreateParameter();
		dateParam.ParameterName = "@date";
		dateParam.Value = portfolio.Date;
		command.Parameters.Add( dateParam );

		IDbDataParameter idParam = command.CreateParameter();
		idParam.ParameterName = "@id";
		idParam.Value = portfolio.Id;
		command.Parameters.Add( idParam );

		command.ExecuteNonQuery();
		unitOfWork.Commit();
	}
}
