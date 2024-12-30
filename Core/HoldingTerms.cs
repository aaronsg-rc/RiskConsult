using RiskConsult.Data.Interfaces;
using RiskConsult.Enumerators;

namespace RiskConsult.Core;

public interface IHoldingTerms :
	IClassIdProperty, ICountryIdProperty, ICurrencyIdProperty, IDescriptionProperty, IHoldingIdProperty,
	IModuleIdProperty, ITermStructureIdProperty, IPeriodIdProperty, ISubTypeIdProperty, ITypeIdProperty,
	ICouponRateProperty, IIsinProperty, IIssueProperty, ILotSizeProperty, IMaturityProperty,
	INominalProperty, IPayDayProperty, IPayFrequencyProperty, IStrikeProperty, ITickerProperty,
	ITicker2Property, IUnderlyingIdProperty, IWeekDayAdjustProperty
{
}

public class HoldingTerms : IHoldingTerms
{
	public ClassId ClassId { get; set; }
	public CountryId CountryId { get; set; }
	public double CouponRate { get; set; }
	public CurrencyId CurrencyId { get; set; }
	public string Description { get; set; } = string.Empty;
	public int HoldingId { get; set; }
	public string Isin { get; set; } = string.Empty;
	public DateTime Issue { get; set; }
	public int LotSize { get; set; }
	public DateTime Maturity { get; set; }
	public ModelId ModelId { get; set; }
	public ModuleId ModuleId { get; set; }
	public double Nominal { get; set; }
	public int PayDay { get; set; }
	public int PayFrequency { get; set; }
	public PeriodId PeriodId { get; set; }
	public double Strike { get; set; }
	public SubTypeId SubTypeId { get; set; }
	public TermStructureId TermStructureId { get; set; }
	public string Ticker { get; set; } = string.Empty;
	public string Ticker2 { get; set; } = string.Empty;
	public TypeId TypeId { get; set; }
	public int UnderlyingId { get; set; }
	public int WeekDayAdjust { get; set; }

	public override string ToString() => $"{HoldingId}|{Description}";
}
