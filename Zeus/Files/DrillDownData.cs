using System.Collections.ObjectModel;

namespace RiskConsult.Zeus.Files;

public readonly struct DrillDownData
{
	public double this[ string subClasification ] => SubClasifications[ subClasification ];
	public double Aggregate => this[ "Aggregate" ];
	public string Clasification { get; }
	public ReadOnlyDictionary<string, double> SubClasifications { get; }

	internal DrillDownData( object[,] arr, int ixRow )
	{
		var dictionary = new Dictionary<string, double>();
		for ( var j = 1; j < arr.GetLength( 1 ); j++ )
		{
			var subClasification = Convert.ToString( arr[ 0, j ] ) ?? string.Empty;
			var value = Convert.ToDouble( arr[ ixRow, j ] );
			dictionary.Add( subClasification, value );
		}

		Clasification = Convert.ToString( arr[ ixRow, 0 ] ) ?? string.Empty;
		SubClasifications = new ReadOnlyDictionary<string, double>( dictionary );
	}

	public override string ToString() => $"{Aggregate:F2} | {Clasification}";
}
