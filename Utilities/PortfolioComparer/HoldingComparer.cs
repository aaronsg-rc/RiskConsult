using Microsoft.Extensions.Logging;

namespace RiskConsult.Utilities.PortfolioComparer;

public class HoldingComparer( ILogger logger )
{
	public bool Compare( IHolding source, IHolding other, int roundDigits = 0, double tolerance = 0 )
	{
		var sourceValue = Math.Round( source.Value, roundDigits );
		var otherValue = Math.Round( other.Value, roundDigits );
		if ( sourceValue != otherValue && Math.Abs( sourceValue - otherValue ) > tolerance )
		{
			logger.LogTrace( "Mismatch on [{sName}]: Amount [Source: {sAmount}, Other: {oAmount}], Price  [Source: {sPrice}, Other: {oPrice}], Value  [Source: {sValue}, Other: {oValue}]", source.Name,
				source.Amount.ToString( "N0" ), other.Amount.ToString( "N0" ),
				source.Price.ToString( "N4" ), other.Price.ToString( "N4" ),
				source.Value.ToString( "N0" ), other.Value.ToString( "N0" ) );
			return false;
		}

		return true;
	}
}
