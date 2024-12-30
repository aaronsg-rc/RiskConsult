namespace RiskConsult.Zeus.Files;

public readonly struct HorizonAnalysisData
{
	public double Probability { get; }
	public ActiveData Returns { get; }
	public string Scenario { get; }

	internal HorizonAnalysisData( object[,] arr, int ixRow )
	{
		var ixCol = 0;
		Scenario = Convert.ToString( arr[ ixRow, ixCol++ ] ) ?? throw new Exception( "Invalid scenario name" );
		Probability = Convert.ToDouble( arr[ ixRow, ixCol++ ] );
		Returns = new ActiveData( arr, ixRow, ixCol );
	}

	public override string ToString() => $"{Scenario} | {Returns}";
}