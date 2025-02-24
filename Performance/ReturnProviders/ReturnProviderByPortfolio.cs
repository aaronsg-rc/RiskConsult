using RiskConsult.Core;
using RiskConsult.Data;
using RiskConsult.Enumerators;
using RiskConsult.Extensions;

namespace RiskConsult.Performance.ReturnProviders;

public class ReturnProviderByPortfolio( string name, CurrencyId fxCurrency, PriceSourceId sourceID ) : ReturnProvider
{
	public readonly string Name = name;
	public CurrencyId FxCurrency = fxCurrency;
	public PriceSourceId SourceID = sourceID;

	public IEnumerable<IHolding> GetComposition( DateTime date )
		=> DbZeus.Db.User?.Portfolios.GetPortfolio( date, Name )?.Holdings ??
		DbZeus.Db.Portfolios.GetPortfolio( date, Name )?.Holdings ??
			throw new Exception( $"Invalid portfolio {Name}" );

	protected override IReturnData CalculateReturn( DateTime date )
	{
		DateTime prevDate = date.GetBusinessDateAdd( DateUnit.Day, -1 );
		IPortfolio port =
			DbZeus.Db.User?.Portfolios.GetPortfolio( prevDate, Name ) ??
			DbZeus.Db.Portfolios.GetPortfolio( prevDate, Name ) ??
			throw new Exception( $"Invalid portfolio {Name}" );
		var initialValue = 0.0;
		var finalValue = 0.0;
		foreach ( IHolding holding in port.Holdings )
		{
			var fxPriceReturnProvider = ReturnProviderByFxPrice.GetProvider( holding.Terms, FxCurrency, SourceID );
			IReturnData holdReturn = fxPriceReturnProvider.GetReturn( date );
			var amount = holding.Amount;
			initialValue += holdReturn.InitialValue * amount;
			finalValue += holdReturn.FinalValue * amount;
		}

		return new ReturnData( date, initialValue, finalValue );
	}
}
