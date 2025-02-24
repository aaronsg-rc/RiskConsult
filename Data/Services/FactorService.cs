using Microsoft.Extensions.DependencyInjection;
using RiskConsult.Core;
using RiskConsult.Data.Entities;
using RiskConsult.Data.Interfaces;
using RiskConsult.Data.Repositories;
using RiskConsult.Enumerators;
using System.Collections;

namespace RiskConsult.Data.Services;

public interface IFactorService : IEnumerable<IFactor>, ICachedService
{
	IFactor GetFactor( string nameOrDescription );

	IFactor GetFactor( int factorId );

	double GetFactorCumulative( int factorId, DateTime date );

	IEnumerable<double> GetFactorCumulative( int factorId, IEnumerable<DateTime> dates );

	double GetFactorReturn( int factorId, DateTime date );

	IEnumerable<double> GetFactorReturn( int factorId, IEnumerable<DateTime> dates );

	/// <summary> Obtiene la entidad de un factor ajustandose a la fecha y tabla que se defina </summary>
	/// <param name="FactorId"> ID del factor </param>
	/// <param name="date"> Fecha de la entidad </param>
	/// <returnsRepository> Entidad del factor para la fecha solicitada, si no existe regresa null </returnsRepository>
	double GetFactorValue( int factorId, DateTime date );

	/// <summary> Obtiene un enumerable de valores de un factor a partir de un enumerable de fechas </summary>
	/// <param name="dates"> Enumerable de fechas de las que se quieren obtener los factores de riesgo </param>
	/// <param name="FactorId"> Id del factor </param>
	IEnumerable<double> GetFactorValue( int factorId, IEnumerable<DateTime> dates );
}

internal class FactorService : IFactorService
{
	private const string _mapGroup = "Factor";
	private const int _tcGroup = 1;
	private static readonly int[] _currencyFactors = [ 36, 44, 45, 62, 76, 79, 83, 87, 92, 93, 103, 107, 110, 329, 330, 332, 333, 334 ];
	private static readonly int[] _volatilityFactors = [ 10056, 10057, 10101, 10102, 10103 ];

	private readonly Dictionary<DateTime, Dictionary<int, double>> _cumulatives = [];
	private readonly IFactorCumulativeRepository _cumulativesRepository;
	private readonly IMapStringRepository _mapStringRepository;
	private readonly Dictionary<DateTime, Dictionary<int, double>> _returns = [];
	private readonly IFactorReturnRepository _returnsRepository;
	private readonly ITcIntegerRepository _tcIntegerRepository;
	private readonly Dictionary<DateTime, Dictionary<int, double>> _values = [];
	private readonly IFactorValueRepository _valuesRepository;

	private Lazy<Dictionary<int, IFactor>> _factors;

	public FactorService( IServiceProvider provider )
	{
		_cumulativesRepository = provider.GetRequiredService<IFactorCumulativeRepository>();
		_mapStringRepository = provider.GetRequiredService<IMapStringRepository>();
		_returnsRepository = provider.GetRequiredService<IFactorReturnRepository>();
		_tcIntegerRepository = provider.GetRequiredService<ITcIntegerRepository>();
		_valuesRepository = provider.GetRequiredService<IFactorValueRepository>();
		_factors = new( () => GetFactorDictionary( _mapStringRepository, _tcIntegerRepository ) );
	}

	/// <summary> Limpia datos almacenados en el cache </summary>
	public void ClearCache()
	{
		_returns.Clear();
		_values.Clear();
		_cumulatives.Clear();
		_factors = new( () => GetFactorDictionary( _mapStringRepository, _tcIntegerRepository ) );
	}

	public IEnumerator<IFactor> GetEnumerator() => _factors.Value.Values.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => _factors.Value.Values.GetEnumerator();

	public IFactor GetFactor( int factorId )
	{
		if ( _factors.Value.TryGetValue( factorId, out IFactor? factor ) )
		{
			return factor;
		}

		throw new KeyNotFoundException( $"Factor with ID {factorId} not found." );
	}

	/// <summary> Obtiene Factor a partir de su nombre o descripción </summary>
	/// <param name="nameOrDescription"> nombre o descripción del factor </param>
	public IFactor GetFactor( string nameOrDescription )
	{
		return _factors.Value.Values.FirstOrDefault( f =>
			f.Name.Equals( nameOrDescription, StringComparison.InvariantCultureIgnoreCase ) ||
			f.Description.Equals( nameOrDescription, StringComparison.InvariantCultureIgnoreCase ) )
			?? throw new KeyNotFoundException( $"Factor with name or description '{nameOrDescription}' not found." );
	}

	public double GetFactorCumulative( int factorId, DateTime date )
	{
		return GetCachedFactorValue( _cumulatives, _cumulativesRepository, factorId, date );
	}

	public IEnumerable<double> GetFactorCumulative( int factorId, IEnumerable<DateTime> dates )
	{
		return dates.Select( date => GetFactorCumulative( factorId, date ) );
	}

	public double GetFactorReturn( int factorId, DateTime date )
	{
		return GetCachedFactorValue( _returns, _returnsRepository, factorId, date );
	}

	public IEnumerable<double> GetFactorReturn( int factorId, IEnumerable<DateTime> dates )
	{
		return dates.Select( date => GetFactorReturn( factorId, date ) );
	}

	public double GetFactorValue( int factorId, DateTime date )
	{
		return GetCachedFactorValue( _values, _valuesRepository, factorId, date );
	}

	public IEnumerable<double> GetFactorValue( int factorId, IEnumerable<DateTime> dates )
	{
		return dates.Select( date => GetFactorValue( factorId, date ) );
	}

	private static double GetCachedFactorValue( Dictionary<DateTime, Dictionary<int, double>> cache, IFactorRepository repository, int factorId, DateTime date )
	{
		if ( !cache.TryGetValue( date, out Dictionary<int, double>? dateValues ) )
		{
			cache[ date ] = dateValues = repository.GetFactorEntities( date ).ToDictionary( e => ( int ) e.FactorId, e => e.Value );
		}

		return dateValues.TryGetValue( factorId, out var value ) ? value : double.NaN;
	}

	private static Dictionary<int, IFactor> GetFactorDictionary( IMapStringRepository mapStringRepository, ITcIntegerRepository tcIntegerRepository )
	{
		var factors = new Dictionary<int, IFactor>();
		IMapStringEntity[] maps = mapStringRepository.GetGroupEntities( _mapGroup );
		ITcIntegerEntity[] tcInts = tcIntegerRepository.GetTcIntegerGroup( _tcGroup );
		foreach ( IMapStringEntity map in maps )
		{
			IEnumerable<ITcIntegerEntity> intEntities = tcInts.Where( e => e.Id == map.Id );
			var factor = new Factor
			{
				FactorId = ( FactorId ) map.Id,
				Name = map.Name,
				Description = map.Description,
				Term = intEntities.FirstOrDefault( e => e.Parameter == "Term" )?.Value ?? -1,
				TermStructureId = ( TermStructureId ) ( intEntities.FirstOrDefault( e => e.Parameter == "TermStructure" )?.Value ?? -1 ),
				FactorTypeId = ( FactorTypeId ) ( intEntities.FirstOrDefault( e => e.Parameter == "Type" )?.Value ?? -1 )
			};

			// Ajuste del factor type
			if ( factor.FactorTypeId is FactorTypeId.FT_DISC )
			{
				factor.FactorTypeId = factor.Name.StartsWith( "df_SPD", StringComparison.InvariantCultureIgnoreCase ) ? FactorTypeId.FT_DISC_SPD : FactorTypeId.FT_DISC_KR;
			}
			else if ( factor.FactorTypeId is FactorTypeId.FT_INDEX )
			{
				if ( _currencyFactors.Contains( ( int ) factor.FactorId ) )
				{
					factor.FactorTypeId = FactorTypeId.FT_CURRENCY;
				}
				else if ( factor.Description.Contains( "Commodity", StringComparison.InvariantCultureIgnoreCase ) )
				{
					factor.FactorTypeId = FactorTypeId.FT_COMMODITY;
				}
			}
			else if ( _volatilityFactors.Contains( ( int ) factor.FactorId ) )
			{
				factor.FactorTypeId = FactorTypeId.FT_VOLATILITY;
			}

			factors[ map.Id ] = factor;
		}

		return factors;
	}
}
