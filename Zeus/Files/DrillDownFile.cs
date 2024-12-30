using RiskConsult.Interop;

namespace RiskConsult.Zeus.Files;

public class DrillDownFile : List<DrillDownData>
{
	public object[,] Values { get; }

	public DrillDownData this[ string clasification ]
		=> this.FirstOrDefault( d => d.Clasification == clasification );

	public double this[ string clasification, string subclasification ]
		=> this[ clasification ][ subclasification ];

	/// <summary> Lee los resultados del archivo </summary>
	/// <param name="filePath"> Ruta del archivo generado por la ejecución del backtesting </param>
	public DrillDownFile( string filePath )
	{
		if ( !File.Exists( filePath ) )
		{
			throw new FileNotFoundException( $"File not found on: {filePath}" );
		}

		// Valido que corresponda a un BacktestFile
		var arrFile = ExcelExtensions.GetArrayFromWorksheet( filePath );
		for ( var i = 1; i < arrFile.GetLength( 0 ); i++ )
		{
			if ( arrFile[ i, 0 ] == null || arrFile[ i, 0 ].ToString()?.Length == 0 )
			{
				continue;
			}

			Add( new DrillDownData( arrFile, i ) );
		}

		Values = arrFile;
	}
}