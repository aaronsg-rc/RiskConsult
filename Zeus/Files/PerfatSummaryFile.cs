using RiskConsult.Interop;

namespace RiskConsult.Zeus.Files;

/// <summary> PerformanceSummary </summary>
public class PerfatSummaryFile
{
	public PerfatSummaryData Active { get; }
	public PerfatSummaryData Benchmark { get; }
	public PerfatSummaryData Portfolio { get; }
	public string PortfolioName { get; }
	public DateTime StartDate { get; }
	public DateTime EndDate { get; }
	public string Frequency { get; }
	public string Model { get; }
	public string BenchmarkName { get; }
	public int Periods { get; }
	public double TotalPortfolioReturn { get; }
	public double DayTradingPortfolioReturn { get; }
	public object[,] Values { get; }

	/// <summary> Realizo la lectura del archivo de resultados </summary>
	/// <param name="filePath"> Ruta de acceso al archivo de resultados </param>
	public PerfatSummaryFile( string filePath )
	{
		// Valido la ruta
		if ( !File.Exists( filePath ) )
		{
			throw new FileNotFoundException( $"File doesn't exist: {filePath}" );
		}

		// Valido que corresponda a un PerformanceSummary
		var arrPSum = ExcelExtensions.GetArrayFromWorksheet( filePath );
		if ( arrPSum[ 1, 0 ].ToString() != "Performance - Summary" )
		{
			throw new Exception( $"File doesn't be Perfat Summary File results: {filePath}" );
		}

		// Asigno propiedades
		PortfolioName = Convert.ToString( arrPSum[ 5, 1 ] ) ?? string.Empty;
		StartDate = Convert.ToDateTime( arrPSum[ 6, 1 ] );
		EndDate = Convert.ToDateTime( arrPSum[ 7, 1 ] );
		Frequency = Convert.ToString( arrPSum[ 8, 1 ] ) ?? string.Empty;
		Model = Convert.ToString( arrPSum[ 9, 1 ] ) ?? string.Empty;
		BenchmarkName = Convert.ToString( arrPSum[ 10, 1 ] ) ?? string.Empty;
		Periods = Convert.ToInt32( arrPSum[ 11, 1 ] );

		TotalPortfolioReturn = Convert.ToDouble( arrPSum[ 14, 1 ] );
		DayTradingPortfolioReturn = Convert.ToDouble( arrPSum[ 15, 1 ] );

		Portfolio = new PerfatSummaryData( arrPSum, 19 );
		Benchmark = new PerfatSummaryData( arrPSum, 20 );
		Active = new PerfatSummaryData( arrPSum, 21 );
		Values = arrPSum;
	}
}