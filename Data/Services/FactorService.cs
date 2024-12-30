using Microsoft.Extensions.DependencyInjection;
using RiskConsult.Core;
using RiskConsult.Data.Entities;
using RiskConsult.Data.Repositories;
using RiskConsult.Enumerators;
using System.Collections;

namespace RiskConsult.Data.Services;

public interface IFactorService : IEnumerable<IFactor>
{
	IFactor GetFactor( string nameOrDescription );

	IFactor GetFactor( int FactorId );

	double GetFactorCumulative( int FactorId, DateTime date );

	IEnumerable<double> GetFactorCumulative( int FactorId, IEnumerable<DateTime> dates );

	double GetFactorReturn( int FactorId, DateTime date );

	IEnumerable<double> GetFactorReturn( int FactorId, IEnumerable<DateTime> dates );

	/// <summary> Obtiene la entidad de un factor ajustandose a la fecha y tabla que se defina </summary>
	/// <param name="FactorId"> ID del factor </param>
	/// <param name="date"> Fecha de la entidad </param>
	/// <returnsRepository> Entidad del factor para la fecha solicitada, si no existe regresa null </returnsRepository>
	double GetFactorValue( int FactorId, DateTime date );

	/// <summary> Obtiene un enumerable de valores de un factor a partir de un enumerable de fechas </summary>
	/// <param name="dates"> Enumerable de fechas de las que se quieren obtener los factores de riesgo </param>
	/// <param name="FactorId"> Id del factor </param>
	IEnumerable<double> GetFactorValue( int FactorId, IEnumerable<DateTime> dates );
}

internal class FactorService( IServiceProvider provider ) : IFactorService
{
	private const string _mapGroup = "Factor";
	private const int _tcGroup = 1;
	private static readonly int[] _currencyFactors = [ 36, 44, 45, 62, 76, 79, 83, 87, 92, 93, 103, 107, 110, 329, 330, 332, 333, 334 ];
	private static readonly int[] _volatilityFactors = [ 10056, 10057, 10101, 10102, 10103 ];
	private readonly Dictionary<DateTime, Dictionary<int, double>> _cumulatives = [];
	private readonly IFactorCumulativeRepository _cumulativesRepository = provider.GetRequiredService<IFactorCumulativeRepository>();
	private readonly IMapStringRepository _mapStringRepository = provider.GetRequiredService<IMapStringRepository>();
	private readonly Dictionary<DateTime, Dictionary<int, double>> _returns = [];
	private readonly IFactorReturnRepository _returnsRepository = provider.GetRequiredService<IFactorReturnRepository>();
	private readonly ITcIntegerRepository _tcIntegerRepository = provider.GetRequiredService<ITcIntegerRepository>();
	private readonly Dictionary<DateTime, Dictionary<int, double>> _values = [];
	private readonly IFactorValueRepository _valuesRepository = provider.GetRequiredService<IFactorValueRepository>();
	private Dictionary<int, IFactor>? _factorStorage;
	private Dictionary<int, IFactor> Factors => _factorStorage ??= GetFactorDictionary( _mapStringRepository, _tcIntegerRepository );

	/// <summary> Limpia datos almacenados en el cache </summary>
	public void ClearCache()
	{
		_returns.Clear();
		_values.Clear();
		_cumulatives.Clear();
		_factorStorage = null;
	}

	public IEnumerator<IFactor> GetEnumerator() => Factors.Values.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => Factors.Values.GetEnumerator();

	public IFactor GetFactor( int FactorId )
	{
		return Factors[ FactorId ];
	}

	/// <summary> Obtiene Factor a partir de su nombre o descripción </summary>
	/// <param name="nameOrDescription"> nombre o descripción del factor </param>
	public IFactor GetFactor( string nameOrDescription )
	{
		return Factors.Values.First( f =>
			f.Name.Equals( nameOrDescription, StringComparison.InvariantCultureIgnoreCase ) ||
			f.Description.Equals( nameOrDescription, StringComparison.InvariantCultureIgnoreCase ) );
	}

	public double GetFactorCumulative( int FactorId, DateTime date )
	{
		return GetDateFactorValue( _cumulatives, _cumulativesRepository, FactorId, date );
	}

	public IEnumerable<double> GetFactorCumulative( int FactorId, IEnumerable<DateTime> dates )
	{
		return dates.Select( date => GetFactorCumulative( FactorId, date ) );
	}

	public double GetFactorReturn( int FactorId, DateTime date )
	{
		return GetDateFactorValue( _returns, _returnsRepository, FactorId, date );
	}

	public IEnumerable<double> GetFactorReturn( int FactorId, IEnumerable<DateTime> dates )
	{
		return dates.Select( date => GetFactorReturn( FactorId, date ) );
	}

	public double GetFactorValue( int FactorId, DateTime date )
	{
		return GetDateFactorValue( _values, _valuesRepository, FactorId, date );
	}

	public IEnumerable<double> GetFactorValue( int FactorId, IEnumerable<DateTime> dates )
	{
		return dates.Select( date => GetFactorValue( FactorId, date ) );
	}

	private static double GetDateFactorValue( Dictionary<DateTime, Dictionary<int, double>> cache, IFactorRepository repository, int FactorId, DateTime date )
	{
		if ( !cache.TryGetValue( date, out Dictionary<int, double>? dateValues ) )
		{
			dateValues = repository.GetFactorEntities( date ).ToDictionary( entity => ( int ) entity.FactorId, entity => entity.Value );
			cache[ date ] = dateValues;
		}

		return dateValues.TryGetValue( FactorId, out var value ) ? value : double.NaN;
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
