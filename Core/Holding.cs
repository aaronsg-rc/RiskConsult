using RiskConsult.Data.Interfaces;
using RiskConsult.Enumerators;

namespace RiskConsult.Core;

public interface IHolding : IDateProperty, IHoldingIdProperty, ICurrencyIdProperty, IPriceSourceIdProperty, IAmountProperty, IPriceProperty, IHoldingTermsProperty, IValueProperty<double>
{
	IHolding Clone();
}

public class Holding : IHolding
{
	public double Amount { get; set; } = 0;
	public CurrencyId CurrencyId { get; set; } = CurrencyId.Invalid;
	public DateTime Date { get; set; } = DateTime.MinValue;
	public int HoldingId { get; set; } = -1;
	public double Price { get; set; } = 0;
	public PriceSourceId PriceSourceId { get; set; } = PriceSourceId.Invalid;
	public IHoldingTerms Terms { get; set; } = new HoldingTerms();
	public double Value { get => GetValue(); set => SetValue( value ); }

	public IHolding Clone()
	{
		return new Holding()
		{
			Amount = Amount,
			CurrencyId = CurrencyId,
			Price = Price,
			Date = Date,
			HoldingId = HoldingId,
			Terms = Terms,
			PriceSourceId = PriceSourceId
		};
	}

	public override string ToString() => Terms.Description;

	private double GetValue() => Price * Amount;

	private void SetValue( double value ) => Amount = value / Price;
}
