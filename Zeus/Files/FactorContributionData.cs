using RiskConsult.Core;
using RiskConsult.Extensions;

namespace RiskConsult.Zeus.Files;

public readonly struct FactorContributionData
{
	public double AverageExposure { get; }
	public IFactor Factor { get; }
	public double ReturnContribution { get; }
	public double Sharpe { get; }
	public double StandarDeviation { get; }

	internal FactorContributionData( object[,] arr, int ixRow )
	{
		var ixCol = 0;
		Factor = arr[ ixRow, ixCol++ ].ToString()?.GetFactor() ?? throw new InvalidOperationException( $"Invalid factor {arr[ ixRow, ixCol++ ]}" );
		AverageExposure = Convert.ToDouble( arr[ ixRow, ixCol++ ] );
		ReturnContribution = Convert.ToDouble( arr[ ixRow, ixCol++ ] );
		StandarDeviation = Convert.ToDouble( arr[ ixRow, ixCol++ ] );
		Sharpe = Convert.ToDouble( arr[ ixRow, ixCol++ ] );
	}

	public override string ToString() => $"{ReturnContribution:F2} {Factor.Description}";
}
