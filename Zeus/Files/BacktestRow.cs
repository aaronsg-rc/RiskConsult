using System.Globalization;

namespace RiskConsult.Zeus.Files;

public class BacktestRow
{
	public DateTime Date { get; }
	public double ExcessReturn { get; }
	public double Risk { get; }
	public double RiskFreeReturn { get; }
	public double TotalReturn { get; }
	public int Violation { get; }

	internal BacktestRow( object[,] arr, int ixRow )
	{
		var ixCol = 0;
		Date = DateTime.ParseExact( arr[ ixRow, ixCol++ ].ToString() ?? string.Empty, "yyyyMMdd", CultureInfo.InvariantCulture );
		TotalReturn = Convert.ToDouble( arr[ ixRow, ixCol++ ] );
		ExcessReturn = Convert.ToDouble( arr[ ixRow, ixCol++ ] );
		RiskFreeReturn = Convert.ToDouble( arr[ ixRow, ixCol++ ] );
		Risk = Convert.ToDouble( arr[ ixRow, ixCol++ ] );
		Violation = Convert.ToInt32( arr[ ixRow, ixCol++ ] );
	}

	public override string ToString() => $"{Date.ToShortDateString()} {TotalReturn:P2}";
}