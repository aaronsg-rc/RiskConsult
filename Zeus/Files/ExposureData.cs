using RiskConsult.Core;
using RiskConsult.Extensions;

namespace RiskConsult.Zeus.Files;

public readonly struct ExposureData
{
	public double Active { get; }
	public double Benchmark { get; }
	public IFactor Factor { get; }
	public double Portfolio { get; }

	internal ExposureData( object[,] arr, int ixRow )
	{
		var ixCol = 0;
		var cols = arr.GetLength( 1 );
		Factor = arr[ ixRow, ixCol++ ].ToString()?.GetFactor() ?? throw new InvalidOperationException( $"Invalid factor {arr[ ixRow, ixCol++ ]}" );
		Portfolio = Convert.ToDouble( arr[ ixRow, ixCol++ ] );
		Benchmark = cols < 4 ? 0 : Convert.ToDouble( arr[ ixRow, ixCol++ ] );
		Active = cols < 4 ? 0 : Convert.ToDouble( arr[ ixRow, ixCol++ ] );
	}

	public override string ToString() => $"{Portfolio:F2} {Factor.Description}";
}
