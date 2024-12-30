using Microsoft.Data.SqlClient;
using RiskConsult.Extensions;
using System.Configuration;
using System.Data;

namespace RiskConsult.FileManager;

/// <summary> Clase abstracta que facilita la lectura del archivo app.config </summary>
public abstract class AppConfig
{
	/// <summary> Acumula cada alerta de lectura basado en los niveles de requirimiento </summary>
	public List<string> Alerts { get; } = [];

	/// <summary> Acumula cada error de lectura basado en los niveles de requirimiento </summary>
	public List<string> Errors { get; } = [];

	public bool WriteToConsole { get; set; } = true;

	public enum KeyRequirement
	{
		Required,
		NotRequired,
		NotApplicable
	}

	public string? CheckDirectory( string? path, bool createDirectory = false )
	{
		var directoryPath = Path.GetDirectoryName( path );
		if ( Directory.Exists( directoryPath ) )
		{
			return directoryPath;
		}

		if ( createDirectory && directoryPath != null )
		{
			try
			{
				_ = Directory.CreateDirectory( directoryPath );
				PrintAlert( $"Directory created: {directoryPath}" );
				return directoryPath;
			}
			catch ( Exception ex )
			{
				PrintAlert( $"Cannot create directory: {directoryPath}, Error: {ex.Message}" );
				return null;
			}
		}

		PrintAlert( $"Directory not found: {directoryPath}" );
		return null;
	}

	public string? GetConnectionString( string key, KeyRequirement requirement, bool testConnection = false )
	{
		var connectionString = ConfigurationManager.ConnectionStrings[ key ]?.ConnectionString;
		if ( string.IsNullOrEmpty( connectionString ) )
		{
			if ( requirement == KeyRequirement.Required )
			{
				PrintError( $"Missing key '{key}' on App.config file" );
			}
			else if ( requirement == KeyRequirement.NotRequired )
			{
				PrintAlert( $"Missing key '{key}' on App.config file" );
			}
		}
		else if ( testConnection )
		{
			try
			{
				using var conexion = new SqlConnection( connectionString );
				conexion.Open();
			}
			catch ( Exception ex )
			{
				if ( requirement == KeyRequirement.Required )
				{
					PrintError( $"Error testing connection: {ex.Message}" );
				}
				else if ( requirement == KeyRequirement.NotRequired )
				{
					PrintAlert( $"Error testing connection: {ex.Message}" );
				}
			}
		}

		return connectionString;
	}

	public string? GetDirectory( string key, KeyRequirement requirement, bool checkDirectory, bool createDirectory,
		DateTime? date = null, string betweenChars = "" )
	{
		var directory = GetConfigurationKeyValue( key, requirement );
		if ( directory != null && date != null && betweenChars != string.Empty )
		{
			directory = directory.DateFormat( date, betweenChars );
		}

		return checkDirectory || createDirectory
			? CheckDirectory( directory, createDirectory )
			: directory;
	}

	public IEnumerable<T> GetEnumerable<T>( string key, KeyRequirement requirement, char separator = ',' )
	{
		var value = GetConfigurationKeyValue( key, requirement );
		return value == null ? [] : value.Split( separator ).Select( v => ( T ) Convert.ChangeType( v.Trim(), typeof( T ) ) );
	}

	public string? GetFilePath( string key, KeyRequirement requirement,
		bool checkDirectory = false, bool checkFile = false, bool createDirectory = false, DateTime? date = null, string betweenChars = "" )
	{
		var filePath = GetConfigurationKeyValue( key, requirement );
		if ( filePath != null && date != null && betweenChars != string.Empty )
		{
			filePath = filePath.DateFormat( date, betweenChars );
		}

		if ( string.IsNullOrEmpty( filePath ) )
		{
			return filePath;
		}

		if ( checkDirectory || createDirectory )
		{
			_ = CheckDirectory( filePath, createDirectory );
		}

		if ( checkFile && !File.Exists( filePath ) )
		{
			if ( requirement is KeyRequirement.Required )
			{
				PrintError( $"File doesn´t exist: {filePath}" );
			}
			else
			{
				PrintAlert( $"File doesn´t exist: {filePath}" );
			}
		}

		return Path.GetFullPath( filePath );
	}

	public T? GetValue<T>( string key, KeyRequirement requirement )
	{
		var value = GetConfigurationKeyValue( key, requirement );
		return value == null ? default : ( T ) Convert.ChangeType( value, typeof( T ) );
	}

	protected void PrintAlert( string message )
	{
		Alerts.Add( message );
		if ( WriteToConsole )
		{
			ConsoleColor color = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.DarkYellow;
			Console.Beep();
			Console.WriteLine( message );
			Console.ForegroundColor = color;
		}
	}

	protected void PrintError( string message )
	{
		Errors.Add( message );
		if ( WriteToConsole )
		{
			ConsoleColor color = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.DarkRed;
			Console.Beep();
			Console.WriteLine( message );
			Console.ForegroundColor = color;
		}
	}

	private string? GetConfigurationKeyValue( string key, KeyRequirement requirement )
	{
		var value = ConfigurationManager.AppSettings[ key ];
		if ( string.IsNullOrEmpty( value ) )
		{
			if ( requirement == KeyRequirement.Required )
			{
				PrintError( $"Missing key '{key}' on App.config file" );
			}
			else if ( requirement == KeyRequirement.NotRequired )
			{
				PrintAlert( $"Missing key '{key}' on App.config file" );
			}
		}

		return value;
	}
}
