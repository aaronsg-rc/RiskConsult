using RiskConsult.Core;
using RiskConsult.Data;
using RiskConsult.Data.Interfaces;

namespace RiskConsult.Extensions;

public static class ExposureExtensions
{
	public static IEnumerable<IFactorValue> GetExposures( this IHoldingIdProperty holdingId, DateTime date, int exposureId = 0 )
	{
		var exposures = DbZeus.Db.User?.Exposures.GetExposures( date, exposureId, holdingId.HoldingId );
		if ( exposures?.Any() ?? false )
		{
			return exposures;
		}

		return DbZeus.Db.Exposures.GetExposures( date, exposureId, holdingId.HoldingId );
	}
}
