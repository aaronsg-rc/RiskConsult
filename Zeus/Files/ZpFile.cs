using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace RiskConsult.Zeus.Files;

public class ZpFile
{
	public string FilePath
	{
		get; private set;
	}

	public Dictionary<string, double> Holdings
	{
		get; private set;
	}

	public string Name
	{
		get; private set;
	}

	public PsFile? PsFile
	{
		get; private set;
	}

	public double WeightedValue
	{
		get; set;
	}

	public ZpFile()
	{
		Reset();
	}

	public ZpFile( string filePath )
	{
		Read( filePath );
	}

	public ZpFile( Dictionary<string, double> holdings )
		: this( holdings, string.Empty, 0 )
	{
	}

	public ZpFile( Dictionary<string, double> holdings, string name )
		: this( holdings, name, 0 )
	{
	}

	public ZpFile( Dictionary<string, double> holdings, string name, double weightedValue )
	{
		Holdings = holdings ?? [];
		Name = name;
		WeightedValue = weightedValue;
		PsFile = null;
		FilePath = string.Empty;
	}

	public static void Create( Dictionary<string, double> holdings, string filePath )
		=> Create( holdings, filePath, 0 );

	public static void Create( Dictionary<string, double> holdings, string filePath, double weightedValue )
	{
		if ( string.IsNullOrEmpty( filePath ) )
		{
			throw new ArgumentException( $"Invalid file path: {filePath}" );
		}

		// Genero cadena de texto del portafolio
		var str = new StringBuilder();
		if ( weightedValue != 0 )
		{
			_ = str.Append( "#WEIGHT\t" ).Append( weightedValue ).AppendLine();
		}

		foreach ( KeyValuePair<string, double> pair in holdings )
		{
			var ticker = pair.Key;
			var amount = pair.Value;
			_ = str.Append( ticker ).Append( "\tT\t" ).Append( amount ).AppendLine();
		}

		// Creo archivo
		var directory = Path.GetDirectoryName( filePath ) ?? throw new Exception( $"Invalid path {filePath}" );
		_ = Directory.CreateDirectory( directory );

		File.WriteAllText( Path.ChangeExtension( filePath, ".zp" ), str.ToString() );
	}

	public void Add( string ticker, double amount )
	{
		Holdings[ ticker ] = Holdings.TryGetValue( ticker, out var existingAmount )
		 ? existingAmount + amount
		 : amount;
	}

	/// <summary> Leo archivo zp y si existe archivo ps tambien se carga </summary>
	[MemberNotNull( nameof( FilePath ), nameof( Holdings ), nameof( Name ) )]
	public void Read( string filePath )
	{
		if ( !File.Exists( filePath ) )
		{
			throw new FileNotFoundException( nameof( filePath ) );
		}

		if ( !string.Equals( Path.GetExtension( filePath ), ".zp", StringComparison.OrdinalIgnoreCase ) )
		{
			throw new ArgumentException( $"Invalid extension on {filePath}" );
		}

		Reset();
		var lines = File.ReadAllLines( filePath );
		if ( lines.Length == 0 )
		{
			return;
		}

		var holdings = new Dictionary<string, double>();
		var weightedValue = 0.0;
		foreach ( var line in lines )
		{
			if ( line.Contains( "#SHARES" ) )
			{
				weightedValue = 0;
			}
			else if ( line.Contains( "#WEIGHT" ) )
			{
				weightedValue = Convert.ToDouble( line[ ( line.IndexOf( '\t' ) + 1 ).. ].Trim() );
			}
			else
			{
				var values = line.Trim().Split( '\t' );
				var ticker = values[ 0 ].Trim();
				var value = Convert.ToDouble( values[ 2 ].Trim() );
				holdings.Add( ticker, value );
			}
		}

		var psFilePath = Path.ChangeExtension( filePath, ".ps" );
		PsFile? psFile = File.Exists( psFilePath ) ? new PsFile( psFilePath ) : null;

		Name = Path.GetFileNameWithoutExtension( filePath );
		FilePath = Path.GetFullPath( filePath );
		Holdings = holdings;
		WeightedValue = weightedValue;
		PsFile = psFile;
	}

	[MemberNotNull( nameof( FilePath ), nameof( Holdings ), nameof( Name ) )]
	public void Reset()
	{
		Holdings = [];
		PsFile = null;
		Name = string.Empty;
		FilePath = string.Empty;
		WeightedValue = 0.0;
	}

	public override string ToString() => $"{Name} [{Holdings.Count}]";

	public void Write( string filePath ) => Write( filePath, 0 );

	public void Write( string filePath, double weightedValue )
	{
		Create( Holdings, filePath, weightedValue );
		var psFilePath = Path.ChangeExtension( filePath, ".ps" );
		PsFile = File.Exists( psFilePath ) ? new PsFile( psFilePath ) : null;
		Name = Path.GetFileName( filePath );
		FilePath = Path.ChangeExtension( filePath, ".zp" );
		WeightedValue = weightedValue;
	}
}