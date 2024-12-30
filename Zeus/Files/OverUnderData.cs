namespace RiskConsult.Zeus.Files;

public readonly struct OverUnderData
{
	public double AverageWeight { get; }
	public string Holding { get; }
	public int PeriodsHeld { get; }
	public double PeriodsHeldPercent { get; }
	public ActiveData Returns { get; }

	internal OverUnderData( object[,] arr, int ixRow )
	{
		Holding = arr[ ixRow, 0 ].ToString() ?? string.Empty;
		AverageWeight = Convert.ToDouble( arr[ ixRow, 1 ] );
		Returns = new ActiveData( arr, ixRow, 2 );
		PeriodsHeld = Convert.ToInt32( arr[ ixRow, 5 ] );
		PeriodsHeldPercent = Convert.ToDouble( arr[ ixRow, 6 ] );
	}

	public override string ToString() => $"{Returns.Portfolio:F2} {Holding}";
}