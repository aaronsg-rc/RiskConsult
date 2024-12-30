using System.Text;

namespace RiskConsult.FileManager;

public partial class IniSection : List<IniLine>
{
	public string this[ string key ]
	{
		get => this.First( k => k.Key == key ).Value ?? "";
		set => this.First( k => k.Key == key ).Value = value;
	}

	private IniLine? _sectionLine;
	public string? Name { get; set; }

	public IniSection( IEnumerable<IniLine> lines, string? sectionName ) : base( lines )
	{
		Name = sectionName;
	}

	public IniSection( IEnumerable<string> iniLines, string? sectionName, char separator )
	{
		Load( iniLines, sectionName, separator );
	}

	public void Add( string key, string value, char separator )
		=> Add( new IniLine( $"{key} {separator} {value}", separator ) );

	public void Load( IEnumerable<string> layoutText, string? sectionName, char separator )
	{
		var currentSection = string.Empty;
		foreach ( var line in layoutText )
		{
			var lineClean = line.Trim();
			if ( string.IsNullOrEmpty( lineClean ) )
			{
				continue;
			}

			var iniLine = new IniLine( line, separator );
			if ( ( iniLine.LineType is LineType.Param or LineType.Comment ) &&
				( currentSection == sectionName || string.IsNullOrEmpty( sectionName ) ) )
			{
				Add( iniLine );
			}
			else if ( iniLine.LineType == LineType.Section && iniLine.SectionName == sectionName && currentSection == string.Empty )
			{
				_sectionLine = iniLine;
				Name = _sectionLine.SectionName;
				currentSection = Name ?? string.Empty;
			}
			else if ( iniLine.LineType == LineType.Section && currentSection == sectionName )
			{
				return;
			}
		}
	}

	public override string ToString() => $"{Name} [{Count}]";

	public string ToText()
	{
		StringBuilder text = new();
		if ( _sectionLine != null )
		{
			_ = text.AppendLine( _sectionLine.ToString() );
		}
		else if ( Name != null )
		{
			_ = text.AppendLine( $"[ {Name} ]" );
		}

		foreach ( IniLine line in this )
		{
			_ = text.AppendLine( line.ToString() );
		}

		return text.ToString().Trim();
	}
}
