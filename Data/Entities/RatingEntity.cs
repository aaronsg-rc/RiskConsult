using RiskConsult.Data.Interfaces;

namespace RiskConsult.Data.Entities;

public interface IRatingEntity : IDateProperty, IHoldingIdProperty, IRatingAgencyIdProperty, IRatingIdProperty
{ }

/// <summary> tblDATA_Rating ( dteDate, intSecurityID, intRatingAgency, intRating ) </summary>
public class RatingEntity : IRatingEntity
{
	/// <summary> dteDate </summary>
	public DateTime Date { get; set; }

	/// <summary> intSecurityID </summary>
	public int HoldingId { get; set; }

	/// <summary> intRatingAgency </summary>
	public int RatingAgencyId { get; set; }

	/// <summary> intRating </summary>
	public int RatingId { get; set; }

	public override string ToString() => string.Join( '|', Date, HoldingId, RatingAgencyId, RatingId );
}
