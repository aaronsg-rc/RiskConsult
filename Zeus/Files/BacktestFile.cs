using RiskConsult.Interop;

namespace RiskConsult.Zeus.Files;

/// <summary> BacktestFile </summary>
public class BacktestFile : List<BacktestRow>
{
	/// <summary> Fecha final del ejercicio </summary>
	public DateTime EndDate { get; }

	/// <summary> Número de veces en que el pronóstico de VaR fue superado por la pérdida del portafolio </summary>
	public int Exceptions { get; }

	/// <summary> Kupiec Stat </summary>
	public double KupiecStat { get; }

	/// <summary> Número de escenarios o fechas hábiles del backtest </summary>
	public int Observations { get; }

	/// <summary> Fecha inicial del ejercicio </summary>
	public DateTime StartDate { get; }

	public BacktestRow? this[ DateTime date ] => this.FirstOrDefault( b => b.Date == date );

	/// <summary> Lee los resultados del archivo </summary>
	/// <param name="filePath"> Ruta del archivo generado por la ejecución del backtesting </param>
	public BacktestFile( string filePath )
	{
		if ( !File.Exists( filePath ) )
		{
			throw new FileNotFoundException( $"File not found on: {filePath}" );
		}

		// Valido que corresponda a un BacktestFile
		var arrFile = ExcelExtensions.GetArrayFromWorksheet( filePath );
		var firstCell = Convert.ToString( arrFile[ 0, 0 ] ) ?? string.Empty;
		if ( arrFile[ 0, 0 ] == null || !firstCell.Contains( "Backtesting Report" ) )
		{
			throw new Exception( $"File doesn't be BacktestFile results: {filePath}" );
		}

		// Asigno Valores
		for ( var i = 7; i < arrFile.GetLength( 0 ); i++ )
		{
			if ( arrFile[ i, 0 ] == null || arrFile[ i, 0 ].ToString()?.Length == 0 )
			{
				continue;
			}

			Add( new BacktestRow( arrFile, i ) );
		}

		StartDate = this.Min( r => r.Date );
		EndDate = this.Max( r => r.Date );
		Observations = Convert.ToInt32( arrFile[ 2, 1 ] );
		Exceptions = Convert.ToInt32( arrFile[ 3, 1 ] );
		KupiecStat = Convert.ToDouble( arrFile[ 4, 1 ] );
	}
}