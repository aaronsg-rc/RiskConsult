using RiskConsult.Core;
using RiskConsult.Data.Entities;
using RiskConsult.Data.Repositories;
using RiskConsult.Enumerators;
using RiskConsult.Extensions;
using System.Data;

namespace RiskConsult.Data.Services;

public interface IPortfolioService
{
	int GetNextId();

	IPortfolio? GetPortfolio( DateTime date, string name );

	IPortfolio? GetPortfolio( DateTime date, string name, PriceSourceId sourceId, CurrencyId fxCurrency );

	int GetPortfolioId( string name );

	public void SavePortfolioToDatabase( IPortfolio portfolio );
}

internal class PortfolioService( IMapStringRepository mapStringRepository, IPortfolioRepository portfolioRepository ) : IPortfolioService
{
	private const string _mapGroup = "Benchmark";
	private readonly Dictionary<(DateTime, string), IPortfolio> _cache = [];

	public int GetNextId()
	{
		return mapStringRepository.GetNextId( _mapGroup );
	}

	public IPortfolio? GetPortfolio( DateTime date, string name )
	{
		if ( _cache.TryGetValue( (date, name), out var portfolio ) )
		{
			return portfolio.Clone();
		}

		IMapStringEntity? map = mapStringRepository.GetGroupEntity( _mapGroup, name );
		if ( map == null )
		{
			return null;
		}

		portfolio = new Portfolio
		{
			Date = date,
			PortfolioId = name,
			Id = map.Id,
			CurrencyId = CurrencyId.Invalid,
			PriceSourceId = PriceSourceId.Invalid
		};

		IPortfolioEntity[] entities = portfolioRepository.GetPortfolioEntities( date, name );
		foreach ( IPortfolioEntity entity in entities )
		{
			IHolding holding = entity.GetHolding( entity.Value );
			portfolio.Holdings.Add( holding );
		}

		_cache[ (date, name) ] = portfolio;

		return portfolio.Clone();
	}

	public IPortfolio? GetPortfolio( DateTime date, string name, PriceSourceId sourceId, CurrencyId fxCurrency )
	{
		IPortfolio? portfolio = GetPortfolio( date, name );
		if ( portfolio == null )
		{
			return null;
		}

		portfolio.CurrencyId = fxCurrency;
		portfolio.PriceSourceId = sourceId;
		portfolio.LoadPrices( date, sourceId, fxCurrency );

		return portfolio;
	}

	public int GetPortfolioId( string name )
	{
		return mapStringRepository.GetGroupEntity( _mapGroup, name )?.Id ?? -1;
	}

	public void SavePortfolioToDatabase( IPortfolio portfolio )
	{
		if ( portfolio.Id == -1 )
		{
			portfolio.Id = GetPortfolioId( portfolio.PortfolioId );
		}

		if ( portfolio.Id == -1 )
		{
			portfolio.Id = GetNextId();
		}

		var map = new MapStringEntity
		{
			Id = portfolio.Id,
			GroupId = _mapGroup,
			Name = portfolio.PortfolioId,
			Description = portfolio.PortfolioId
		};

		var holdings = new List<PortfolioEntity>();
		foreach ( IHolding hold in portfolio.Holdings )
		{
			var holdEntity = new PortfolioEntity
			{
				Date = portfolio.Date,
				Name = portfolio.PortfolioId,
				HoldingId = hold.HoldingId,
				Value = hold.Amount,
			};

			holdings.Add( holdEntity );
		}

		portfolioRepository.UnitOfWork.BeginTransaction();
		mapStringRepository.Delete( _mapGroup, portfolio.Id );
		mapStringRepository.Delete( _mapGroup, portfolio.PortfolioId );
		mapStringRepository.Insert( map );
		portfolioRepository.Delete( portfolio.Date, portfolio.PortfolioId );
		portfolioRepository.Insert( holdings.ToArray() );

		using IDbCommand command = portfolioRepository.UnitOfWork.CreateCommand();
		command.CommandText =
			$"DELETE FROM tblDATA_PortfolioSettings where dteDate = @date and intPortfolioID = @id;" +
			$"DELETE FROM tblParameter_Text WHERE txtGroup='{_mapGroup}' and intID=@id;" +
			$"DELETE FROM tblParameter_Float WHERE txtGroup='{_mapGroup}' and intID=@id;" +
			$"DELETE FROM tblParameter_Date WHERE txtGroup='{_mapGroup}' and intID=@id;" +
			$"DELETE FROM tblParameter_Integer WHERE txtGroup='{_mapGroup}' and intID=@id;" +
			$"DELETE FROM tblParameter_IntVector WHERE txtGroup='{_mapGroup}' and intID=@id;" +
			$"DELETE FROM tblParameter_DoubleVector WHERE txtGroup='{_mapGroup}' and intID=@id;" +
			$"DELETE FROM tblParameter_IntegerMap WHERE txtGroup='{_mapGroup}' and intID=@id;" +
			$"DELETE FROM tblParameter_DoubleMap WHERE txtGroup='{_mapGroup}' and intID=@id;" +
			$"INSERT INTO tblDATA_PortfolioSettings values( @id, @date, 1.000000);" +
			$"INSERT INTO tblParameter_Float values ( @id, '{_mapGroup}', 'AmountType', 0.000000)";

		IDbDataParameter dateParam = command.CreateParameter();
		dateParam.ParameterName = "@date";
		dateParam.Value = portfolio.Date;
		command.Parameters.Add( dateParam );

		IDbDataParameter idParam = command.CreateParameter();
		idParam.ParameterName = "@id";
		idParam.Value = portfolio.Id;
		command.Parameters.Add( idParam );

		command.ExecuteNonQuery();
		portfolioRepository.UnitOfWork.Commit();
	}
}
