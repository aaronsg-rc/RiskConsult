namespace RiskConsult.Data.Entities;

public interface ITcHoldingEntity
{
	int? ClassId { get; set; }
	int? CountryId { get; set; }
	double? CouponRate { get; set; }
	int? CurrencyId { get; set; }
	string Description { get; set; }
	int HoldingId { get; set; }
	string? Isin { get; set; }
	DateTime? Issue { get; set; }
	int? LotSize { get; set; }
	DateTime? Maturity { get; set; }
	int? ModuleId { get; set; }
	double? Nominal { get; set; }
	int? PayDay { get; set; }
	int? PayFrequency { get; set; }
	int? PeriodId { get; set; }
	double? Strike { get; set; }
	int? SubTypeId { get; set; }
	int? TermStructureId { get; set; }
	string Ticker { get; set; }
	string? Ticker2 { get; set; }
	int TypeId { get; set; }
	int? UnderlyingId { get; set; }
	int? WeekDayAdjust { get; set; }
}

public class TcHoldingEntity : ITcHoldingEntity
{
	public int? ClassId { get; set; }
	public int? CountryId { get; set; }
	public double? CouponRate { get; set; }
	public int? CurrencyId { get; set; }
	public string Description { get; set; } = string.Empty;
	public int HoldingId { get; set; } = -1;
	public string? Isin { get; set; }
	public DateTime? Issue { get; set; }
	public int? LotSize { get; set; }
	public DateTime? Maturity { get; set; }
	public int? ModuleId { get; set; }
	public double? Nominal { get; set; }
	public int? PayDay { get; set; }
	public int? PayFrequency { get; set; }
	public int? PeriodId { get; set; }
	public double? Strike { get; set; }
	public int? SubTypeId { get; set; }
	public int? TermStructureId { get; set; }
	public string Ticker { get; set; } = string.Empty;
	public string? Ticker2 { get; set; }
	public int TypeId { get; set; }
	public int? UnderlyingId { get; set; }
	public int? WeekDayAdjust { get; set; }

	public override string ToString() => string.Join( '|', HoldingId, Ticker2, Description );
}
