using RiskConsult.Core;
using RiskConsult.Data.Repositories;
using RiskConsult.Extensions;

namespace RiskConsult.Data.Services;

public interface IExposureService
{
	public IEnumerable<IFactorValue> GetExposures( DateTime date, int exposureId, int holdingId );
}

internal class ExposureService( IExposureRepository exposureRepository ) : IExposureService
{
	private readonly Dictionary<(DateTime, int, int), IFactorValue[]> _exposures = [];

	public void ClearCache() => _exposures.Clear();

	public IEnumerable<IFactorValue> GetExposures( DateTime date, int exposureId, int holdingId )
	{
		if ( _exposures.TryGetValue( (date, exposureId, holdingId), out IFactorValue[]? exposures ) )
		{
			return exposures;
		}

		return _exposures[ (date, exposureId, holdingId) ] = exposureRepository
			.GetExposureEntities( date, exposureId, holdingId )
			.Select( e => new FactorValue { Factor = e.GetFactor(), Value = e.Value } )
			.ToArray();
	}
}
