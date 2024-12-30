using RiskConsult.Interop;

namespace RiskConsult.Zeus.Files;

/// <summary> OverUnder </summary>
public class OverUnderFile : List<OverUnderData>
{
	/// <summary> Portfolio Securities </summary>
	public double OverWeightedSecurities { get; }

	/// <summary> Active Securities </summary>
	public double SecuritiesHeld { get; }

	/// <summary> Benchmark Securities </summary>
	public double UnderWeightedSecurities { get; }

	public object[,] Values { get; }

	/// <summary> Hago lectura del archivo over under </summary>
	/// <param name="filePath"> Ruta de acceso al archivo </param>
	public OverUnderFile( string filePath ) : base()
	{
		if ( !File.Exists( filePath ) )
		{
			throw new Exception( $"File doesn't exist: {filePath}" );
		}

		// Valido que corresponda a un FactorContribution
		var arrOU = ExcelExtensions.GetArrayFromWorksheet( filePath );
		if ( arrOU[ 0, 0 ].ToString() != "Performance - Security Contribution" )
		{
			throw new Exception( $"File doesn't be OverUnder results: {filePath}" );
		}

		//Agrego datos
		for ( var i = 12; i < arrOU.GetLength( 0 ); i++ )
		{
			if ( arrOU[ i, 0 ] == null || arrOU[ i, 0 ].ToString()?.Length == 0 )
			{
				continue;
			}

			Add( new OverUnderData( arrOU, i ) );
		}

		OverWeightedSecurities = Convert.ToDouble( arrOU[ 5, 1 ] );
		UnderWeightedSecurities = Convert.ToDouble( arrOU[ 6, 1 ] );
		SecuritiesHeld = Convert.ToDouble( arrOU[ 7, 1 ] );
		Values = arrOU;
	}
}