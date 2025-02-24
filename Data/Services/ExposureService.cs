using RiskConsult.Core;
using RiskConsult.Data.Entities;
using RiskConsult.Data.Interfaces;
using RiskConsult.Data.Repositories;
using RiskConsult.Extensions;

namespace RiskConsult.Data.Services;

public interface IExposureService : ICachedService
{
	public IFactorValue[] GetExposures( DateTime date, int exposureId, int holdingId );
}

internal class ExposureService( IExposureRepository exposureRepository ) : IExposureService
{
	private readonly Dictionary<(DateTime, int, int), IExposureEntity[]> _cache = [];

	public void ClearCache() => _cache.Clear();

	public IFactorValue[] GetExposures( DateTime date, int exposureId, int holdingId )
	{
		return GetExposureEntities( date, exposureId, holdingId )
			.Select( e => new FactorValue { Factor = e.GetFactor(), Value = e.Value } )
			.ToArray();
	}

	private IExposureEntity[] GetExposureEntities( DateTime date, int exposureId, int holdingId )
	{
		if ( _cache.TryGetValue( (date, exposureId, holdingId), out IExposureEntity[]? exposures ) )
		{
			return exposures;
		}

		return _cache[ (date, exposureId, holdingId) ] = exposureRepository.GetExposureEntities( date, exposureId, holdingId );
	}
}
