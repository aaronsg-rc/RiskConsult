using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace RiskConsult.FileManager;

public class IniFile : List<IniSection>
{
	/// <summary> Obtiene el valor del parámetro dentro de la sección indicada </summary>
	/// <param name="section"> Nombre de la sección </param>
	/// <param name="param"> Nombre del parámetro </param>
	/// <param name="defaultValue"> Si no se encuentra el valor devuelve el valor default </param>
	/// <returns> Valor de <paramref name="param" /> dentro de <paramref name="section" /> </returns>
	public string this[ string section, string param, string defaultValue = "" ]
		=> GetParameterValue( section, param, defaultValue );

	/// <summary> Ruta completa del Ini </summary>
	public string FilePath
	{
		get; private set;
	}

	public IniFile()
	{
		FilePath = string.Empty;
	}

	public IniFile( IEnumerable<IniSection> sections ) : base( sections )
	{
		FilePath = string.Empty;
	}

	public IniFile( string filePath, char separator )
	{
		LoadIniFile( filePath, separator );
	}

	/// <summary> Obtiene el valor del parámetro dentro de la sección indicada </summary>
	/// <param name="section"> Nombre de la sección </param>
	/// <param name="param"> Nombre del parámetro </param>
	/// <param name="defaultValue"> Si no se encuentra el valor devuelve el valor default </param>
	/// <returns> Valor de <paramref name="param" /> dentro de <paramref name="section" /> </returns>
	public string GetParameterValue( string section, string param, string defaultValue = "" )
	{
		return this
			.FirstOrDefault( s => section.Trim() == s.Name )?
			.FirstOrDefault( p => param.Trim() == p.Key )?.Value ??
			defaultValue;
	}

	public string GetParameterValue( string param, string defaultValue = "" )
	{
		return this
			.FirstOrDefault( s => s.FirstOrDefault( p => param.Trim() == p.Key ) != null )?
			.FirstOrDefault( p => param.Trim() == p.Key )?.Value ??
			defaultValue;
	}

	[MemberNotNull( nameof( FilePath ) )]
	public void LoadIniFile( string filePath, char separator )
	{
		if ( !File.Exists( filePath ) )
		{
			throw new FileNotFoundException( $"File not found: {filePath}" );
		}

		IEnumerable<string> lines = File.ReadLines( filePath );
		var sections = lines
			.Where( l => l.Contains( '[' ) && l.Contains( ']' ) )
			.Select( l => l.Substring( l.IndexOf( '[' ) + 1, l.IndexOf( ']' ) - l.IndexOf( '[' ) - 1 ).Trim() )
			.ToList();

		if ( sections.Count == 0 )
		{
			Add( new IniSection( lines, null, separator ) );
		}
		else
		{
			foreach ( var section in sections )
			{
				Add( new IniSection( lines, section, separator ) );
			}
		}

		FilePath = filePath;
	}

	public override string ToString() => $"Sections: {Count} | AllParams: {this.Sum( s => s.Count )}";

	public string ToText()
	{
		StringBuilder text = new();
		foreach ( IniSection section in this )
		{
			_ = text.AppendLine( section.ToText() + '\n' );
		}

		return text.ToString().Trim();
	}

	public void WriteToFile( string filePath )
			=> File.WriteAllText( filePath, ToText() );
}
