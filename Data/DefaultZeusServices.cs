using Microsoft.Extensions.DependencyInjection;
using RiskConsult.Data.Entities;
using RiskConsult.Data.Interfaces;
using RiskConsult.Data.Repositories;
using RiskConsult.Data.Services;
using System.Data;

namespace RiskConsult.Data;

public static class DefaultZeusServices
{
	public static IServiceCollection AddDefaultZeusServices( this IServiceCollection services, IDbConnection connection )
	{
		return services
			.AddSingleton( connection )
			.AddSingleton<IUnitOfWork, UnitOfWork>()
			.AddZeusEntities()
			.AddZeusRepositories()
			.AddZeusServices();
	}

	public static IServiceCollection CreateDefaultZeusServices( IDbConnection connection )
	{
		return new ServiceCollection().AddDefaultZeusServices( connection );
	}

	private static IServiceCollection AddZeusEntities( this IServiceCollection services )
	{
		return services
			.AddTransient<IAmortizationEntity, AmortizationEntity>()
			.AddTransient<ICatalogRatingEntity, CatalogRatingEntity>()
			.AddTransient( typeof( ICustomHoldingEntity<> ), typeof( CustomHoldingEntity<> ) )
			.AddTransient<IDateEntity, DateEntity>()
			.AddTransient<IExposureEntity, ExposureEntity>()
			.AddTransient<IFactorEntity, FactorEntity>()
			.AddTransient<IFloaterResetEntity, FloaterResetEntity>()
			.AddTransient<IHoldingEventEntity, HoldingEventEntity>()
			.AddTransient<IMapHoldingEntity, MapHoldingEntity>()
			.AddTransient<IMapStringEntity, MapStringEntity>()
			.AddTransient<IPortfolioEntity, PortfolioEntity>()
			.AddTransient<IPriceEntity, PriceEntity>()
			.AddTransient<IRatingEntity, RatingEntity>()
			.AddTransient<IScenarioEntity, ScenarioEntity>()
			.AddTransient<IShockEntity, ShockEntity>()
			.AddTransient<ITcHoldingEntity, TcHoldingEntity>()
			.AddTransient<ITcIntegerEntity, TcIntegerEntity>()
			.AddTransient<ITermStructureEntity, TermStructureEntity>();
	}

	private static IServiceCollection AddZeusRepositories( this IServiceCollection services )
	{
		return services
			.AddSingleton<IBusinessDaysRepository>( prov => new DateRepository( prov.GetRequiredService<IUnitOfWork>(), "tblDATA_BusinessDays" ) )
			.AddSingleton<IHolidaysRepository>( prov => new DateRepository( prov.GetRequiredService<IUnitOfWork>(), "tblDATA_Holidays" ) )
			.AddSingleton<IFactorReturnRepository>( prov => new FactorRepository( prov.GetRequiredService<IUnitOfWork>(), "tblDATA_FactorReturns" ) )
			.AddSingleton<IFactorValueRepository>( prov => new FactorRepository( prov.GetRequiredService<IUnitOfWork>(), "tblDATA_Factors" ) )
			.AddSingleton<IFactorCumulativeRepository>( prov => new FactorRepository( prov.GetRequiredService<IUnitOfWork>(), "tblDATA_FactorsCumulative" ) )
			.AddSingleton<ICustomHoldingRepository<DateTime>, CustomHoldingRepository<DateTime>>()
			.AddSingleton<ICustomHoldingRepository<double>, CustomHoldingRepository<double>>()
			.AddSingleton<ICustomHoldingRepository<int>, CustomHoldingRepository<int>>()
			.AddSingleton<ICustomHoldingRepository<string>, CustomHoldingRepository<string>>()
			.AddSingleton<IAmortizationRepository, AmortizationRepository>()
			.AddSingleton<IExposureRepository, ExposureRepository>()
			.AddSingleton<IFloaterResetRepository, FloaterResetRepository>()
			.AddSingleton<IHoldingEventRepository, HoldingEventRepository>()
			.AddSingleton<IMapHoldingRepository, MapHoldingRepository>()
			.AddSingleton<IMapStringRepository, MapStringRepository>()
			.AddSingleton<IPortfolioRepository, PortfolioRepository>()
			.AddSingleton<IPriceRepository, PriceRepository>()
			.AddSingleton<IScenarioRepository, ScenarioRepository>()
			.AddSingleton<ITcHoldingRepository, TcHoldingRepository>()
			.AddSingleton<ITcIntegerRepository, TcIntegerRepository>()
			.AddSingleton<ITermStructureRepository, TermStructureRepository>();
	}

	private static IServiceCollection AddZeusServices( this IServiceCollection services )
	{
		return services
			.AddSingleton<IAmortizationService, AmortizationService>()
			.AddSingleton<ICustomHoldingService, CustomHoldingService>()
			.AddSingleton<IDateService, DateService>()
			.AddSingleton<IExposureService, ExposureService>()
			.AddSingleton<IFactorService, FactorService>()
			.AddSingleton<IFloaterResetService, FloaterResetService>()
			.AddSingleton<IEventService, EventService>()
			.AddSingleton<IHoldingService, HoldingService>()
			.AddSingleton<IPortfolioService, PortfolioService>()
			.AddSingleton<IPriceService, PriceService>()
			.AddSingleton<IScenarioService, ScenarioService>()
			.AddSingleton<ITermStructureService, TermStructureService>()
			.AddSingleton<IDbZeus, DbZeus>()
			.AddSingleton<IDbUser, DbUser>();
	}
}
