using Microsoft.Extensions.DependencyInjection;
using RiskConsult.Data.Services;
using System.Data;

namespace RiskConsult.Data;

public interface IDbZeus
{
	IAmortizationService Amortizations { get; }
	IDateService Dates { get; }
	IEventService Events { get; }
	IExposureService Exposures { get; }
	IFactorService Factors { get; }
	IFloaterResetService FloaterResets { get; }
	IHoldingService Holdings { get; }
	IPortfolioService Portfolios { get; }
	IPriceService Prices { get; }
	ITermStructureService TermStructures { get; }
	IDbUser? User { get; set; }
}

public class DbZeus : IDbZeus
{
	private static DbZeus? _instance;

	public static DbZeus Db => _instance ?? throw new InvalidOperationException( $"You most initilize by static method 'Initialize'" );

	public IAmortizationService Amortizations { get; }
	public IDateService Dates { get; }
	public IEventService Events { get; }
	public IExposureService Exposures { get; }
	public IFactorService Factors { get; }
	public IFloaterResetService FloaterResets { get; }
	public IHoldingService Holdings { get; }
	public IPortfolioService Portfolios { get; }
	public IPriceService Prices { get; }
	public ITermStructureService TermStructures { get; }
	public IDbUser? User { get; set; }

	private DbZeus( IServiceProvider services )
	{
		Amortizations = services.GetRequiredService<IAmortizationService>();
		TermStructures = services.GetRequiredService<ITermStructureService>();
		Dates = services.GetRequiredService<IDateService>();
		Factors = services.GetRequiredService<IFactorService>();
		Exposures = services.GetRequiredService<IExposureService>();
		Prices = services.GetRequiredService<IPriceService>();
		Portfolios = services.GetRequiredService<IPortfolioService>();
		FloaterResets = services.GetRequiredService<IFloaterResetService>();
		Holdings = services.GetRequiredService<IHoldingService>();
		Events = services.GetRequiredService<IEventService>();
		try
		{
			User = services.GetService<IDbUser>();
		}
		catch ( Exception )
		{ }
	}

	public static void Initialize( IServiceProvider serviceProvider )
	{
		_instance = new DbZeus( serviceProvider );
	}

	public static void Initialize( IDbConnection zeusConnection, IDbConnection? userConnection = null )
	{
		IServiceCollection services = new ServiceCollection().AddDefaultZeusServices( zeusConnection );
		if ( userConnection != null )
		{
			var dbUser = DbUser.Initialize( userConnection );
			services.AddSingleton<IDbUser>( dbUser );
		}

		_instance = new DbZeus( services.BuildServiceProvider() );
	}
}
