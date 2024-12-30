using Microsoft.Extensions.Logging;

namespace RiskConsult.Utilities.PortfolioComparer;

public class PortfolioComparer( ILogger logger )
{
	public bool Compare( IPortfolio sourcePortfolio, IPortfolio otherPortfolio, int roundDigits = 0, double tolerance = 0 )
	{
		var areEqual = true;
		if ( sourcePortfolio.Holdings.Count != otherPortfolio.Holdings.Count )
		{
			logger.LogError( "Holdings count mismatch: Source[{sourceCount}] vs Target[{otherCount}]", sourcePortfolio.Holdings.Count, otherPortfolio.Holdings.Count );
			areEqual = false;
		}

		var sourceValue = Math.Round( sourcePortfolio.Value, roundDigits );
		var otherValue = Math.Round( otherPortfolio.Value, roundDigits );
		if ( Math.Abs( sourceValue - otherValue ) > tolerance )
		{
			logger.LogError( "Portfolio value mismatch: Source[{sourceValue}] vs Target[{otherValue}]", sourcePortfolio.Value.ToString( "N0" ), otherPortfolio.Value.ToString( "N0" ) );
			areEqual = false;
		}

		IEnumerable<string> allHoldingNames = sourcePortfolio.Holdings
			.Union( otherPortfolio.Holdings )
			.Select( hold => hold.Name )
			.Distinct();

		var holdComparer = new HoldingComparer( logger );
		foreach ( var name in allHoldingNames )
		{
			IHolding? sourceHold = sourcePortfolio.Holdings.GetHolding( name );
			IHolding? otherHold = otherPortfolio.Holdings.GetHolding( name );

			if ( sourceHold == null || otherHold == null )
			{
				var word = sourceHold == null ? "Source" : "Other";
				logger.LogError( "Missing holding on {word}: {name}", word, name );
				areEqual = false;
				continue;
			}

			if ( !holdComparer.Compare( sourceHold, otherHold, roundDigits, tolerance ) )
			{
				areEqual = false;
			}
		}

		return areEqual;
	}
}
