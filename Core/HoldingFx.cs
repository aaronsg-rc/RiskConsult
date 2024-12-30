using RiskConsult.Enumerators;
using RiskConsult.Extensions;

namespace RiskConsult.Core;

public class HoldingFx : Holding
{
	public double Fx { get; set; }
	public CurrencyId FxCurrencyId { get; set; }
	public double FxPrice => Fx * Price;
	public double FxValue => FxPrice * Amount;

	public HoldingFx( IHoldingTerms terms, double amount = 1 )
	{
		Terms = terms;
		Amount = amount;
	}

	public void LoadPrice( DateTime date, PriceSourceId sourceID = PriceSourceId.PiP_MD, CurrencyId fxCurrency = CurrencyId.MXN )
	{
		Fx = CurrencyId.ConvertToCurrency( fxCurrency, Date );
		FxCurrencyId = fxCurrency;
		PriceExtensions.LoadPrice( this, date, sourceID, fxCurrency );
	}

	public void SetFxValue( double value ) => Amount = value / FxPrice;
}
