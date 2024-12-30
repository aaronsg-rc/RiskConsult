using RiskConsult.Interop;
using System.Collections.ObjectModel;

namespace RiskConsult.Zeus.Files;

public class SummaryFile
{
	public string Portfolio { get; }
	public string Benchmark { get; }
	public double BetaCash { get; }
	public double BetaPercent { get; }
	public CurvesActiveData Convexities { get; }
	public string Currency { get; }
	public DateTime Date { get; }
	public CurvesActiveData Durations { get; }
	public int Holdings { get; }
	public RiskActiveData ParametricModel { get; }
	public ActiveData Returns { get; }
	public ActiveData Sharpes { get; }
	public ReadOnlyDictionary<string, ActiveData> Shocks { get; }
	public RiskActiveData SimulationModel { get; }
	public double Value { get; }
	public object[,] Values { get; }

	public SummaryFile( string filePath )
	{
		if ( !File.Exists( filePath ) )
		{
			throw new FileNotFoundException( $"File not found on: {filePath}" );
		}

		// Valido que corresponda a un BacktestFile
		var arrFile = ExcelExtensions.GetArrayFromWorksheet( filePath );
		var fileTitle = arrFile[ 0, 0 ].ToString() ?? string.Empty;
		if ( !fileTitle.Contains( "Portfolio  - Summary" ) )
		{
			throw new Exception( $"File doesn't be Summary results: {filePath}" );
		}

		// Obtengo shocks
		var dict = new Dictionary<string, ActiveData>();
		for ( var i = 37; i < arrFile.GetLength( 0 ); i++ )
		{
			var name = arrFile[ i, 0 ].ToString() ?? string.Empty;
			dict[ name ] = new ActiveData( arrFile, i, 1 );
		}

		//Asigno valores
		Portfolio = Convert.ToString( arrFile[ 0, 1 ] ) ?? string.Empty;
		Date = Convert.ToDateTime( arrFile[ 1, 1 ] );
		Benchmark = Convert.ToString( arrFile[ 2, 1 ] ) ?? string.Empty;
		Currency = Convert.ToString( arrFile[ 3, 1 ] ) ?? string.Empty;
		Holdings = Convert.ToInt32( arrFile[ 4, 1 ] );
		Value = Convert.ToDouble( arrFile[ 5, 1 ] ) * 1000;
		BetaPercent = Convert.ToDouble( arrFile[ 6, 1 ] );
		BetaCash = Convert.ToDouble( arrFile[ 7, 1 ] ) * 1000;
		Returns = new ActiveData( arrFile, 2, 7, true );
		Sharpes = new ActiveData( arrFile, 2, 8, true );
		ParametricModel = new RiskActiveData( arrFile, 10, 1 );
		SimulationModel = new RiskActiveData( arrFile, 14, 1 );
		Durations = new CurvesActiveData( arrFile, 19, 1 );
		Convexities = new CurvesActiveData( arrFile, 19, 5 );
		Shocks = new ReadOnlyDictionary<string, ActiveData>( dict );
		Values = arrFile;
	}
}