using Microsoft.Extensions.DependencyInjection;
using RiskConsult.Data.Services;
using System.Data;

namespace RiskConsult.Data;

public interface IDbUser
{
	ICustomHoldingService Customs { get; }
	IExposureService Exposures { get; }
	IPortfolioService Portfolios { get; }
	IPriceService Prices { get; }
	IScenarioService Scenarios { get; }
}

public class DbUser : IDbUser
{
	public ICustomHoldingService Customs { get; }
	public IExposureService Exposures { get; }
	public IPortfolioService Portfolios { get; }
	public IPriceService Prices { get; }
	public IScenarioService Scenarios { get; }

	private DbUser( IServiceProvider provider )
	{
		Customs = provider.GetRequiredService<ICustomHoldingService>();
		Prices = provider.GetRequiredService<IPriceService>();
		Portfolios = provider.GetRequiredService<IPortfolioService>();
		Scenarios = provider.GetRequiredService<IScenarioService>();
		Exposures = provider.GetRequiredService<IExposureService>();
	}

	public static DbUser Initialize( IServiceProvider provider ) => new( provider );

	public static DbUser Initialize( IDbConnection connection )
	{
		ServiceProvider provider = DefaultZeusServices.CreateDefaultZeusServices( connection ).BuildServiceProvider( false );
		return new DbUser( provider );
	}
}
