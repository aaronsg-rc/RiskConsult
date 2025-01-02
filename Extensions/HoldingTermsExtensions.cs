using RiskConsult.Core;
using RiskConsult.Data;
using RiskConsult.Data.Interfaces;
using RiskConsult.Enumerators;

namespace RiskConsult.Extensions;

public static class HoldingTermsExtensions
{
	public static IHoldingTerms GetHoldingTerms( this IHoldingIdProperty holdingId )
	{
		return GetHoldingTerms( holdingId.HoldingId );
	}

	public static IHoldingTerms? GetHoldingTerms( this string holdingId, HoldingIdType idType )
	{
		if ( idType is HoldingIdType.HoldingId )
		{
			return GetHoldingTerms( Convert.ToInt32( holdingId ) );
		}

		return
			DbZeus.Db.Holdings.GetHoldingTerms( holdingId, idType ) ??
			DbZeus.Db.User?.Customs.GetHoldingTerms( holdingId, idType );
	}

	public static IHoldingTerms GetHoldingTerms( this int holdingId )
	{
		if ( holdingId >= 2000000 )
		{
			ArgumentNullException.ThrowIfNull( DbZeus.Db.User, nameof( DbZeus.Db.User ) );
			return DbZeus.Db.User.Customs.GetHoldingTerms( holdingId );
		}
		else
		{
			return DbZeus.Db.Holdings.GetHoldingTerms( holdingId );
		}
	}
}
