using System.Diagnostics.CodeAnalysis;

namespace RiskConsult.Zeus.Files;

/// <summary> Clase de los archivos .ps para leer y manipular algunos de sus contenidos </summary>
public class PsFile
{
	/// <summary> Arreglo con cada línea del archivo ps </summary>
	private string[] _lines;

	/// <summary> Benchmark contra el que se compara el portafolio dentro del archivo ps </summary>
	public string Benchmark { get; private set; }

	/// <summary> Valor del nivel de confianza 1 dentro del archivo ps </summary>
	public string Confidence1 { get; private set; }

	/// <summary> Valor del nivel de confianza 2 dentro del archivo ps </summary>
	public string Confidence2 { get; private set; }

	/// <summary> Valor del nivel de confianza 3 dentro del archivo ps </summary>
	public string Confidence3 { get; private set; }

	/// <summary> Moneda base del portafolio dentro del archivo ps </summary>
	public string CurrencyId { get; private set; }

	public string FilePath { get; private set; }

	/// <summary> Modelo de riesgo dentro del archivo ps </summary>
	public string Model { get; private set; }

	public string Name { get; private set; }

	/// <summary> Valor del simulador dentro del archivo ps </summary>
	public string Simulator { get; private set; }

	/// <summary> Tiempo en días al que se escalan los valores de riesgo del modelo paramétrico </summary>
	public string Time_Scalling { get; private set; }

	/// <summary> Constructor a partir de ruta </summary>
	/// <param name="filePath"> Ruta del archivo .ps </param>
	public PsFile( string filePath )
	{
		Read( filePath );
	}

	/// <summary> Extrae los valores almacenados en el archivo ps basado en la línea ingresada como propiedad </summary>
	/// <param name="psProperty"> Propiedad de la que se quiere extraer el valor </param>
	/// <returns> Valor alojado en el archivo ps para la propiedad solicitada </returns>
	public string GetPropertyValue( PsProperty psProperty )
		=> GetPropertyValue( psProperty.ToString() );

	/// <summary> Extrae los valores almacenados en el archivo ps basado en la línea ingresada como propiedad </summary>
	/// <param name="psProperty"> Propiedad de la que se quiere extraer el valor </param>
	/// <returns> Valor alojado en el archivo ps para la propiedad solicitada </returns>
	public string GetPropertyValue( string psProperty )
	{
		var propertyLine = Array.Find( _lines, l => l.Contains( psProperty, StringComparison.InvariantCultureIgnoreCase ) ) ??
			throw new ArgumentNullException( $"Property {psProperty} not found in file." );

		return propertyLine.Split( '\t' ).Last().Trim();
	}

	/// <summary> Lee el archivo especificado en formato .ps y extrae las propiedades relevantes. </summary>
	/// <param name="filePath"> La ruta del archivo .ps que se va a leer. </param>
	/// <exception cref="ArgumentException"> Se lanza si filePath es nulo, vacío, no existe o no tiene la extensión .ps. </exception>
	/// <exception cref="Exception"> Se lanza si el contenido del archivo no es válido. </exception>
	[MemberNotNull(
		nameof( _lines ), nameof( Benchmark ), nameof( Simulator ), nameof( Confidence1 ), nameof( Confidence2 ), nameof( Confidence3 ),
		nameof( Model ), nameof( Time_Scalling ), nameof( CurrencyId ), nameof( FilePath ), nameof( Name ) )]
	public void Read( string filePath )
	{
		if ( string.IsNullOrEmpty( filePath ) || !File.Exists( filePath ) || Path.GetExtension( filePath ) != ".ps" )
		{
			throw new ArgumentException( $"Invalid filePath: {filePath}" );
		}

		// Obtengo propiedades desde el contenido
		Reset();
		_lines = File.ReadAllLines( filePath );
		Benchmark = GetPropertyValue( PsProperty.Benchmark );
		Simulator = GetPropertyValue( PsProperty.RiskSettings );
		Confidence1 = GetPropertyValue( PsProperty.Confidence1 );
		Confidence2 = GetPropertyValue( PsProperty.Confidence2 );
		Confidence3 = GetPropertyValue( PsProperty.Confidence3 );
		Model = GetPropertyValue( PsProperty.Model );
		Time_Scalling = GetPropertyValue( PsProperty.AnHorizon );
		CurrencyId = GetPropertyValue( PsProperty.CurrencyId );
		FilePath = Path.GetFullPath( filePath );
		Name = Path.GetFileNameWithoutExtension( filePath );
	}

	/// <summary> Cambia el valor de la propiedad del archivo ps </summary>
	/// <param name="psProperty"> Línea en que se encuentra la propiedad </param>
	/// <param name="newValue"> Valor que se va a colocar en la propiedad </param>
	public void UpdateProperty( PsProperty psProperty, string newValue )
		=> UpdateProperty( psProperty.ToString(), newValue );

	/// <summary> Cambia el valor de la propiedad del archivo ps </summary>
	/// <param name="propertyName"> Nombre de la propiedad </param>
	/// <param name="newValue"> Valor que se va a colocar en la propiedad </param>
	public void UpdateProperty( string propertyName, string newValue )
	{
		var currentValue = GetPropertyValue( propertyName );
		for ( var i = 0; i < _lines.Length; i++ )
		{
			if ( _lines[ i ].Contains( currentValue, StringComparison.InvariantCultureIgnoreCase ) )
			{
				_lines[ i ] = _lines[ i ].Replace( currentValue, newValue );
			}
		}

		Write( FilePath );
	}

	/// <summary> Escribe las líneas en el archivo ps </summary>
	public void Write( string filePath ) => File.WriteAllLines( filePath, _lines );

	private void Reset()
	{
		_lines = [];
		Name = string.Empty;
		FilePath = string.Empty;
		Benchmark = string.Empty;
		Simulator = string.Empty;
		Confidence1 = string.Empty;
		Confidence2 = string.Empty;
		Confidence3 = string.Empty;
		Model = string.Empty;
		Time_Scalling = string.Empty;
		CurrencyId = string.Empty;
	}
}

/// <summary> Hace referencia al índice de la línea en que se encuentra la propiedad dentro del ps </summary>
public enum PsProperty
{
	Benchmark, InitialPort, RiskSettings, Return, Confidence1, Confidence2, Confidence3, LiveMode, Model, MCBins,
	AnHorizon, ExplorerTree, ReturnSource, RiskFreeMode, RiskFreeTerm, RiskFreeTSID, RiskFreeRate, CurrencyId, StressModelId, IsDiagonal,
	ValueReference, ExpandComposites, AmountType, BalanceCash, DefaultBalanceCashCurrency, RefreshMode, StressID, Rating, Limits
}
