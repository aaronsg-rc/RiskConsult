namespace RiskConsult.Zeus.Files;

public readonly struct PerfatSummaryData
{
	public double AverageReturn { get; }
	public double Positive { get; }
	public double Return { get; }
	public double Risk { get; }
	public double Sharpe { get; }

	internal PerfatSummaryData( object[,] arr, int ixRow )
	{
		var ixCol = 1;
		Return = Convert.ToDouble( arr[ ixRow, ixCol++ ] );
		AverageReturn = Convert.ToDouble( arr[ ixRow, ixCol++ ] );
		Risk = Convert.ToDouble( arr[ ixRow, ixCol++ ] );
		Sharpe = Convert.ToDouble( arr[ ixRow, ixCol++ ] );
		Positive = Convert.ToDouble( arr[ ixRow, ixCol++ ] );
	}

	public override string ToString() => $"Ret: {Return:F2} | Risk: {Risk:F2}";
}