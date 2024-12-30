using RiskConsult.Interop;

namespace RiskConsult.Zeus.Files;

public class HorizonAnalysisFile : List<HorizonAnalysisData>
{
	public DateTime AnalysisDate { get; }
	public string Benchmark { get; }
	public DateTime HorizonDate { get; }
	public string Portfolio { get; }
	public object[,] Values { get; }

	public HorizonAnalysisFile( string filePath )
	{
		if ( !File.Exists( filePath ) )
		{
			throw new FileNotFoundException( $"File not found on: {filePath}" );
		}

		// Valido que corresponda a un BacktestFile
		var arrFile = ExcelExtensions.GetArrayFromWorksheet( filePath );
		var fileTitle = arrFile[ 1, 0 ].ToString() ?? string.Empty;
		if ( !fileTitle.Contains( "Portfolio - Horizon Analysis" ) )
		{
			throw new Exception( $"File doesn't be Horizon Analysis results: {filePath}" );
		}

		//Asigno valores
		for ( var i = 11; i < arrFile.GetLength( 0 ); i++ )
		{
			if ( arrFile[ i, 0 ] == null || arrFile[ i, 0 ].ToString()?.Length == 0 )
			{
				continue;
			}

			Add( new HorizonAnalysisData( arrFile, i ) );
		}

		Portfolio = Convert.ToString( arrFile[ 4, 1 ] ) ?? string.Empty;
		Benchmark = Convert.ToString( arrFile[ 5, 1 ] ) ?? string.Empty;
		AnalysisDate = Convert.ToDateTime( arrFile[ 6, 1 ] );
		HorizonDate = Convert.ToDateTime( arrFile[ 7, 1 ] );
		Values = arrFile;
	}
}