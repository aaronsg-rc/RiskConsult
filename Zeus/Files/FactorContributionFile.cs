using RiskConsult.Interop;

namespace RiskConsult.Zeus.Files;

public class FactorContributionFile : List<FactorContributionData>
{
	public FactorContributionData this[ string nameOrDescription ]
		=> this.FirstOrDefault( f => f.Factor.Name == nameOrDescription || f.Factor.Description == nameOrDescription );

	public object[,] Values { get; }

	/// <summary> Constructor base </summary>
	public FactorContributionFile( string filePath ) : base()
	{
		if ( !File.Exists( filePath ) )
		{
			throw new FileNotFoundException( $"File doesn't exist: {filePath}" );
		}

		// Valido que corresponda a un FactorContribution
		var arrFC = ExcelExtensions.GetArrayFromWorksheet( filePath );
		if ( arrFC[ 1, 0 ].ToString() != "Performance - Factor Contribution" )
		{
			throw new Exception( $"File doesn't be FactorContribution results: {filePath}" );
		}

		//Valido si es un factor y lo agrego
		for ( var i = 4; i < arrFC.GetLength( 0 ); i++ )
		{
			if ( arrFC[ i, 0 ] == null || arrFC[ i, 0 ].ToString()?.Length == 0 )
			{
				break;
			}

			Add( new FactorContributionData( arrFC, i ) );
		}

		Values = arrFC;
	}
}
