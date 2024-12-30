using RiskConsult.Enumerators;
using RiskConsult.Extensions;

namespace RiskConsult.Performance.Providers;

public class ReturnProviderByFx : ReturnProvider
{
	public readonly CurrencyId FromCurrency;
	public readonly CurrencyId ToCurrency;

	private static readonly Dictionary<(CurrencyId, CurrencyId), ReturnProviderByFx> _providers = [];

	private ReturnProviderByFx( CurrencyId fromCurrency, CurrencyId toCurrency )
	{
		FromCurrency = fromCurrency;
		ToCurrency = toCurrency;
	}

	public static ReturnProviderByFx GetProvider( CurrencyId fromCurrency, CurrencyId toCurrency )
	{
		if ( _providers.TryGetValue( (fromCurrency, toCurrency), out ReturnProviderByFx? provider ) )
		{
			return provider;
		}

		provider = new ReturnProviderByFx( fromCurrency, toCurrency );
		_providers[ (fromCurrency, toCurrency) ] = provider;

		return provider;
	}

	public override string ToString() => $"{ToCurrency}/{FromCurrency}";

	protected override IReturnData CalculateReturn( DateTime date )
	{
		DateTime prevDate = date.GetBusinessDateAdd( DateUnit.Day, -1 );
		return new ReturnData( date,
			FromCurrency.ConvertToCurrency( ToCurrency, prevDate ),
			FromCurrency.ConvertToCurrency( ToCurrency, date ) );
	}
}
