using RiskConsult.Interop;

namespace RiskConsult.Zeus.Files;

public class RiskDecompositionFile
{
	public RiskDecompositionData CommonFactor { get; }
	public RiskDecompositionData Currency { get; }
	public RiskDecompositionData Index { get; }
	public RiskDecompositionData Industries { get; }
	public RiskDecompositionData Selection { get; }
	public RiskDecompositionData Spread { get; }
	public RiskDecompositionData Style { get; }
	public RiskDecompositionData TermStructure { get; }
	public RiskDecompositionData Total { get; }
	public object[,] Values { get; }

	/// <summary> Lee los resultados del archivo </summary>
	public RiskDecompositionFile( string filePath )
	{
		if ( !File.Exists( filePath ) )
		{
			throw new FileNotFoundException( $"File not found on: {filePath}" );
		}

		// Asigno datos
		var arrFile = ExcelExtensions.GetArrayFromWorksheet( filePath );
		Total = new RiskDecompositionData( arrFile, 2 );
		CommonFactor = new RiskDecompositionData( arrFile, 3 );
		Industries = new RiskDecompositionData( arrFile, 4 );
		Style = new RiskDecompositionData( arrFile, 5 );
		TermStructure = new RiskDecompositionData( arrFile, 6 );
		Spread = new RiskDecompositionData( arrFile, 7 );
		Index = new RiskDecompositionData( arrFile, 8 );
		Currency = new RiskDecompositionData( arrFile, 9 );
		Selection = new RiskDecompositionData( arrFile, 10 );
		Values = arrFile;
	}
}