using RiskConsult.Interop;

namespace RiskConsult.Zeus.Files;

public class ExposureFile : List<ExposureData>
{
	public ExposureData this[ string nameOrDescription ] => this.FirstOrDefault( e => e.Factor.Name == nameOrDescription || e.Factor.Description == nameOrDescription );

	public object[,] Values { get; }

	/// <summary> Lee los resultados del archivo </summary>
	public ExposureFile( string filePath ) : base()
	{
		if ( !File.Exists( filePath ) )
		{
			throw new FileNotFoundException( $"File not found on: {filePath}" );
		}

		// Asigno valores
		var arrFile = ExcelExtensions.GetArrayFromWorksheet( filePath );
		for ( var i = 0; i < arrFile.GetLength( 0 ); i++ )
		{
			if ( arrFile[ i, 0 ] == null || arrFile[ i, 0 ].ToString()?.Length == 0 )
			{
				continue;
			}

			Add( new ExposureData( arrFile, i ) );
		}

		Values = arrFile;
	}
}
