namespace RiskConsult.Zeus.Files;

public readonly struct RiskDecompositionData
{
	public double RaR1 { get; }
	public double RaR2 { get; }
	public double RaR3 { get; }
	public double StandarDeviation { get; }
	public double VaR1 { get; }
	public double VaR2 { get; }
	public double VaR3 { get; }
	public double Variance { get; }

	internal RiskDecompositionData( object[,] arr, int ixRow )
	{
		var ixCol = 1;
		Variance = Convert.ToDouble( arr[ ixRow, ixCol++ ] );
		StandarDeviation = Convert.ToDouble( arr[ ixRow, ixCol++ ] );
		RaR1 = Convert.ToDouble( arr[ ixRow, ixCol++ ] );
		VaR1 = Convert.ToDouble( arr[ ixRow, ixCol++ ] );
		RaR2 = Convert.ToDouble( arr[ ixRow, ixCol++ ] );
		VaR2 = Convert.ToDouble( arr[ ixRow, ixCol++ ] );
		RaR3 = Convert.ToDouble( arr[ ixRow, ixCol++ ] );
		VaR3 = Convert.ToDouble( arr[ ixRow, ixCol++ ] );
	}
}