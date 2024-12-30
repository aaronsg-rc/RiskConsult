namespace RiskConsult.Zeus.Files;

public readonly struct RiskActiveData
{
	public ActiveData RaR1 { get; }
	public ActiveData RaR2 { get; }
	public ActiveData RaR3 { get; }
	public ActiveData VaR1 { get; }
	public ActiveData VaR2 { get; }
	public ActiveData VaR3 { get; }
	public ActiveData StandarDeviation { get; }
	public ActiveData Variance { get; }

	internal RiskActiveData( object[,] arr, int ixRow, int ixCol )
	{
		Variance = new ActiveData( arr, ixRow, ixCol++, true );
		StandarDeviation = new ActiveData( arr, ixRow, ixCol++, true );
		RaR1 = new ActiveData( arr, ixRow, ixCol++, true );
		VaR1 = new ActiveData( arr, ixRow, ixCol++, true );
		RaR2 = new ActiveData( arr, ixRow, ixCol++, true );
		VaR2 = new ActiveData( arr, ixRow, ixCol++, true );
		RaR3 = new ActiveData( arr, ixRow, ixCol++, true );
		VaR3 = new ActiveData( arr, ixRow, ixCol++, true );
	}
}