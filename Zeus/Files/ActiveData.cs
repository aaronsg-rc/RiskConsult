namespace RiskConsult.Zeus.Files;

public readonly struct ActiveData
{
	public double Active
	{
		get;
	}

	public double Benchmark
	{
		get;
	}

	public double Portfolio
	{
		get;
	}

	internal ActiveData( object[,] arr, int ixRow, int ixCol, bool toDown = false )
	{
		Portfolio = Convert.ToDouble( toDown ? arr[ ixRow++, ixCol ] : arr[ ixRow, ixCol++ ] );
		Benchmark = Convert.ToDouble( toDown ? arr[ ixRow++, ixCol ] : arr[ ixRow, ixCol++ ] );
		Active = Convert.ToDouble( toDown ? arr[ ixRow++, ixCol ] : arr[ ixRow, ixCol++ ] );
	}

	internal ActiveData( double portfolio, double benchmark, double active )
	{
		Portfolio = portfolio;
		Benchmark = benchmark;
		Active = active;
	}

	public override string ToString() => $"{Portfolio:F2} | {Benchmark:F2} | {Active:F2}";
}