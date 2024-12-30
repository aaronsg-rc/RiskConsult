using RiskConsult.Core;
using RiskConsult.Data.Entities;
using RiskConsult.Data.Repositories;
using RiskConsult.Enumerators;

namespace RiskConsult.Data.Services;

public interface ITermStructureService
{
	public ITermStructure GetTermStructure( DateTime date, TermStructureId TermStructureId );
}

internal class TermStructureService( ITermStructureRepository termStructureRepository, IMapStringRepository mapStringRepository ) : ITermStructureService
{
	private const string _mapGroup = "TermStructure";
	private readonly Dictionary<(DateTime, TermStructureId), ITermStructure> _termStructures = [];
	private Dictionary<TermStructureId, IMapStringEntity> _names = [];

	public ITermStructure GetTermStructure( DateTime date, TermStructureId termStructureId )
	{
		if ( termStructureId is TermStructureId.Invalid )
		{
			throw new ArgumentException( "Invalid termStructureId", nameof( termStructureId ) );
		}

		if ( _termStructures.TryGetValue( (date, termStructureId), out ITermStructure? termStructure ) )
		{
			return termStructure;
		}

		return CreateAndAddTermStructure( date, termStructureId );
	}

	private ITermStructure CreateAndAddTermStructure( DateTime date, TermStructureId termStructureId )
	{
		if ( _names.Count == 0 )
		{
			_names = mapStringRepository.GetGroupEntities( _mapGroup ).ToDictionary( e => ( TermStructureId ) e.Id, e => e );
		}

		IMapStringEntity map = _names[ termStructureId ];
		ITermStructureEntity[] entities = termStructureRepository.GetTermStructureEntities( date, ( int ) termStructureId );
		var values = termStructureId is TermStructureId.MXN or TermStructureId.TIIE or TermStructureId.UDI
			? entities.Select( e => Math.Log( 1 + e.Value ) ).ToArray()
			: entities.Select( e => Math.Log( 1 + ( e.Value * e.Term / 360 ) ) * 360 / e.Term ).ToArray();

		return _termStructures[ (date, termStructureId) ] = new TermStructure()
		{
			TermStructureId = termStructureId,
			Date = date,
			Name = map.Name,
			Description = map.Description,
			Terms = entities.Select( e => e.Term ).ToArray(),
			Values = values
		};
	}
}
