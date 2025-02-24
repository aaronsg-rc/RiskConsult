using RiskConsult.Core;
using RiskConsult.Data.Entities;
using RiskConsult.Data.Repositories;
using RiskConsult.Enumerators;
using RiskConsult.Extensions;

namespace RiskConsult.Data.Services;

public interface IScenarioService
{
	/// <summary> Modifica el valor de un factor dentro del escenario agregando una cantidad de puntos base </summary>
	/// <param name="value"> Valor a agregar en puntos base </param>
	void AddBps( IScenarioFactor scnFactor, double value );

	IScenario CreateNewScenario( DateTime date, string name );

	int GetNextId();

	/// <summary> Constructor que asigna y consulta los valores del factor para la fecha solicitada </summary>
	/// <param name="factor"> Factor base </param>
	/// <param name="date"> Fecha de la que se quieren los valores </param>
	IScenarioFactor GetScenarioFactor( IFactor factor, DateTime date );

	IScenario? GetScenarioForUpdate( DateTime date, string name );

	IScenario? GetScenarioForUpdate( DateTime date, int id );

	/// <summary> Obtiene el ID del escenario según su nombre </summary>
	/// <param name="name"> Nombre del escenario </param>
	/// <returns> El id del escenario o -1 si no existe </returns>
	int GetScenarioId( string name );

	/// <summary> Establece el valor inicial del factor y aplica la conversión en caso de curvas </summary>
	/// <param name="date"> Fecha de la que se quieren los valores </param>
	double GetScenarioInitialValue( IFactor factor, DateTime date );

	/// <summary> Cargo mi escenario a la base de datos de usuario </summary>
	void SaveScenarioToDataBase( IScenario scenario );

	void SetScenarioBpsShockToAll( IScenario scenario, double value );

	void SetScenarioBpsShockToCurve( IScenario scenario, TermStructureId curveType, double value );

	void SetScenarioBpsShockToFactorGroup( IScenario scenario, FactorTypeId factorType, double value );

	/// <summary> Asigna valor a una curva </summary>
	/// <param name="curveType"> Nombre de la curva, solo se requieren los primeros 3 carácteres </param>
	/// <param name="value"> Valor a asignar, puede ser en bps o como valor directo </param>
	void SetScenarioValueToCurve( IScenario scenario, TermStructureId curveType, double value );

	/// <summary> Asigna valor a un grupo de factores de riesgo </summary>
	/// <param name="factorType"> Grupo de factores a los que se le agregara el valor </param>
	/// <param name="value"> Valor a asignar, puede ser en bps o como valor directo </param>
	void SetScenarioValueToFactorGroup( IScenario scenario, FactorTypeId factorType, double value );
}

internal class ScenarioService( IMapStringRepository mapStringRepository, IScenarioRepository scenarioRepository, IUnitOfWork unitOfWork ) : IScenarioService
{
	private static readonly int[] _scenarioFactors = [ 0, 1, 2, 3, 4, 5, 6, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 60, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 300, 301, 302, 303, 304, 305, 306, 307, 308, 309, 310, 311, 320, 321, 322, 323, 324, 325, 326, 327, 328, 329, 330, 332, 333, 334, 1400, 1401, 1402, 1403, 1404, 1405, 10059, 10060, 10061, 10062, 10063, 10070, 10071, 10072, 10080, 10081, 10082, 10083, 10084, 10085, 10086, 10101, 10102, 10103, 10115, 10116 ];

	public void AddBps( IScenarioFactor scnFactor, double value )
	{
		if ( scnFactor.Factor.FactorTypeId is FactorTypeId.FT_KR or FactorTypeId.FT_SPREAD )
		{
			scnFactor.Value = scnFactor.InitialValue + ( value / 10000.0 );
		}
		else
		{
			scnFactor.Value = scnFactor.InitialValue * ( 1 + ( value / 10000 ) );
		}
	}

	/// <summary> Crea un nuevo escenario con el valor de los factores de la fecha proporcionada </summary>
	/// <param name="date"> Fecha del escenario </param>
	public IScenario CreateNewScenario( DateTime date, string name )
	{
		var scenario = new Scenario()
		{
			Date = date,
			Name = name,
			Description = name,
			ScenarioId = GetNextId(),
			Factors = _scenarioFactors.Select( id => id.GetFactor().GetScenarioFactor( date ) ).ToList()
		};

		return scenario;
	}

	public int GetNextId()
	{
		return mapStringRepository.GetNextId( "Scenario" );
	}

	public IScenarioFactor GetScenarioFactor( IFactor factor, DateTime date )
	{
		var initialValue = GetScenarioInitialValue( factor, date );
		return new ScenarioFactor
		{
			Factor = factor,
			InitialValue = initialValue,
			Value = initialValue
		};
	}

	public IScenario? GetScenarioForUpdate( DateTime date, string name )
	{
		IMapStringEntity? map = mapStringRepository.GetGroupEntity( "Scenario", name );
		if ( map == null )
		{
			return null;
		}

		IScenario scenario = CreateNewScenario( date, name );
		scenario.ScenarioId = map.Id;

		return scenario;
	}

	public IScenario? GetScenarioForUpdate( DateTime date, int id )
	{
		IMapStringEntity? map = mapStringRepository.GetGroupEntity( "Scenario", id );
		if ( map == null )
		{
			return null;
		}

		IScenario scenario = CreateNewScenario( date, map.Description );
		scenario.ScenarioId = map.Id;
		return scenario;
	}

	public int GetScenarioId( string name )
	{
		return mapStringRepository.GetGroupEntity( "Scenario", name )?.Id ?? -1;
	}

	public double GetScenarioInitialValue( IFactor factor, DateTime date )
	{
		var value = factor.GetFactorValue( date );
		if ( factor.TermStructureId is TermStructureId.LIB or TermStructureId.TRS or TermStructureId.EUR or TermStructureId.UMS )
		{
			value = Math.Pow( 1.0 + ( value * factor.Term / 360.0 ), 360.0 / factor.Term ) - 1.0;
		}

		return value;
	}

	public void SaveScenarioToDataBase( IScenario scenario )
	{
		var map = new MapStringEntity()
		{
			Id = scenario.ScenarioId,
			GroupId = "Scenario",
			Name = scenario.Name,
			Description = scenario.Description,
		};

		var scenarioEntities = new List<IScenarioEntity>();
		foreach ( IScenarioFactor scnFactor in scenario.Factors )
		{
			var value =
				scnFactor.Factor.FactorTypeId is FactorTypeId.FT_KR or FactorTypeId.FT_SPREAD
				? 1.0 / Math.Pow( 1.0 + scnFactor.Value, scnFactor.Factor.Term / 360.0 )
				: scnFactor.Value;

			FactorId factorId =
				scnFactor.Factor.FactorTypeId is FactorTypeId.FT_KR or FactorTypeId.FT_SPREAD
				? $"df_{scnFactor.Factor.Name}".GetFactor().FactorId
				: scnFactor.Factor.FactorId;

			scenarioEntities.Add( new ScenarioEntity
			{
				ScenarioId = scenario.ScenarioId,
				FactorId = factorId,
				Value = value
			} );

			if ( scnFactor.Factor.FactorTypeId is FactorTypeId.FT_SPREAD )
			{
				scenarioEntities.Add( new ScenarioEntity
				{
					ScenarioId = scenario.ScenarioId,
					FactorId = scnFactor.Factor.FactorId,
					Value = scnFactor.Value
				} );
			}
		}

		unitOfWork.BeginTransaction();
		mapStringRepository.Delete( "Scenario", scenario.ScenarioId );
		mapStringRepository.Delete( "Scenario", scenario.Name );
		scenarioRepository.Delete( scenario.ScenarioId );
		mapStringRepository.Insert( map );
		scenarioRepository.Insert( scenarioEntities.ToArray() );
		unitOfWork.Commit();
	}

	public void SetScenarioBpsShockToAll( IScenario scenario, double value )
	{
		foreach ( IScenarioFactor factor in scenario.Factors )
		{
			AddBps( factor, value );
		}
	}

	public void SetScenarioBpsShockToCurve( IScenario scenario, TermStructureId curveType, double value )
	{
		foreach ( IScenarioFactor factor in scenario.Factors.Where( f => f.Factor.TermStructureId == curveType ) )
		{
			AddBps( factor, value );
		}
	}

	public void SetScenarioBpsShockToFactorGroup( IScenario scenario, FactorTypeId factorType, double value )
	{
		foreach ( IScenarioFactor factor in scenario.Factors.Where( f => f.Factor.FactorTypeId == factorType ) )
		{
			AddBps( factor, value );
		}
	}

	public void SetScenarioValueToCurve( IScenario scenario, TermStructureId curveType, double value )
	{
		foreach ( IScenarioFactor factor in scenario.Factors.Where( f => f.Factor.TermStructureId == curveType ) )
		{
			factor.Value = value;
		}
	}

	public void SetScenarioValueToFactorGroup( IScenario scenario, FactorTypeId factorType, double value )
	{
		foreach ( IScenarioFactor factor in scenario.Factors.Where( f => f.Factor.FactorTypeId == factorType ) )
		{
			factor.Value = value;
		}
	}
}
