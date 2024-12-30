using RiskConsult.Data.Interfaces;

namespace RiskConsult.Data.Entities;

public interface ICatalogRatingEntity : IRatingIdProperty, IRatingAgencyIdProperty, ICodeProperty, ICommentProperty, ISeverityProperty
{ }

/// <summary> tblCatalog_Ratings ( intRatingAgency, intRating, txtCode, txtComment, intSeverity ) </summary>
public class CatalogRatingEntity : ICatalogRatingEntity
{
	/// <summary> txtCode </summary>
	public string Code { get; set; } = string.Empty;

	/// <summary> txtComment </summary>
	public string Comment { get; set; } = string.Empty;

	/// <summary> intRatingAgency </summary>
	public int RatingAgencyId { get; set; }

	/// <summary> intRating </summary>
	public int RatingId { get; set; }

	/// <summary> intSeverity </summary>
	public int Severity { get; set; }

	public override string ToString() => string.Join( '|', RatingAgencyId, RatingId, Code, Comment, Severity );
}
