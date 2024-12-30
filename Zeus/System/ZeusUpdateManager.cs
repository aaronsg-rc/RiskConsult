using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using RiskConsult.Enumerators;
using RiskConsult.Extensions;
using System.Diagnostics;

namespace RiskConsult.Zeus.System;

public static class ZeusUpdateManager
{
	private const string _appKeyDb = @"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Cyrnel\ZEUS\Paths\DB";
	private const string _appKeyUm = @"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Cyrnel\ZEUS\UM";
	private const string _appKeyZeus = @"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Cyrnel\ZEUS";

	public static string AppPath => $"{Registry.GetValue( _appKeyZeus, "Zeus", "" )?.ToString()}ZUM.exe";

	public static DateTime BulkDate
	{
		get => DateTimeOffset.FromUnixTimeSeconds( Convert.ToInt64( Registry.GetValue( _appKeyUm, "BulkDate", "" ) ) ).Date;
		set => Registry.SetValue( _appKeyUm, "BulkDate", new DateTimeOffset( value ).ToUnixTimeSeconds(), RegistryValueKind.DWord );
	}

	public static ZumConnection ConnectionType
	{
		get => ( ZumConnection ) Convert.ToInt32( Registry.GetValue( _appKeyUm, "Mode", "" ) );
		set => Registry.SetValue( _appKeyUm, "Mode", ( int ) value, RegistryValueKind.DWord );
	}

	public static ZumCountry Country
	{
		get
		{
			var value = Registry.GetValue( _appKeyUm, "Country", "" )?.ToString() ?? "";
			return ( ZumCountry ) Convert.ToInt32( Enum.Parse( typeof( ZumCountry ), value ) );
		}

		set => Registry.SetValue( _appKeyUm, "Country", value.ToString(), RegistryValueKind.String );
	}

	public static string DateDelim
	{
		get => Registry.GetValue( _appKeyDb, "DateDelim", "" )?.ToString() ?? "";
		set => Registry.SetValue( _appKeyDb, "DateDelim", value, RegistryValueKind.String );
	}

	public static ZumDbSource DbSource
	{
		get => ( ZumDbSource ) Convert.ToInt32( Registry.GetValue( _appKeyDb, "IsMDB", "" ) );
		set => Registry.SetValue( _appKeyDb, "IsMDB", ( int ) value, RegistryValueKind.DWord );
	}

	public static string DbUser
	{
		get => Registry.GetValue( _appKeyDb, "USER", "" )?.ToString() ?? "";
		set => Registry.SetValue( _appKeyDb, "USER", value, RegistryValueKind.String );
	}

	public static string DbZeus
	{
		get => Registry.GetValue( _appKeyDb, "ZEUS", "" )?.ToString() ?? "";
		set => Registry.SetValue( _appKeyDb, "ZEUS", value, RegistryValueKind.String );
	}

	public static DateTime FileDate
	{
		get => DateTimeOffset.FromUnixTimeSeconds( Convert.ToInt64( Registry.GetValue( _appKeyUm, "UpdDate", "" ) ) ).Date;
		set => Registry.SetValue( _appKeyUm, "UpdDate", DateTimeOffset.Parse( Convert.ToString( value ) ).ToUnixTimeSeconds(), RegistryValueKind.DWord );
	}

	public static string LogFolder
	{
		get => Registry.GetValue( _appKeyUm, "LogPath", "" )?.ToString() ?? "";
		set => Registry.SetValue( _appKeyUm, "LogPath", value, RegistryValueKind.String );
	}

	public static string Password
	{
		get => Registry.GetValue( _appKeyZeus, "Password", "" )?.ToString() ?? "";
		set => Registry.SetValue( _appKeyZeus, "Password", value, RegistryValueKind.String );
	}

	public static long Port
	{
		get => Convert.ToInt64( Registry.GetValue( _appKeyUm, "Port", "" ) );
		set => Registry.SetValue( _appKeyUm, "Port", value, RegistryValueKind.DWord );
	}

	public static DateTime ReleaseDate
	{
		get => DateTimeOffset.FromUnixTimeSeconds( Convert.ToInt64( Registry.GetValue( _appKeyUm, "ReReleaseDate", "" ) ) ).Date;
		set => Registry.SetValue( _appKeyUm, "ReReleaseDate", DateTimeOffset.Parse( Convert.ToString( value ) ).ToUnixTimeSeconds(), RegistryValueKind.DWord );
	}

	public static string Server1
	{
		get => Registry.GetValue( _appKeyUm, "Server", "" )?.ToString() ?? ""; set => Registry.SetValue( _appKeyUm, "Server", value, RegistryValueKind.String );
	}

	public static string Server2
	{
		get => Registry.GetValue( _appKeyUm, "Server2", "" )?.ToString() ?? ""; set => Registry.SetValue( _appKeyUm, "Server2", value, RegistryValueKind.String );
	}

	public static string Server3
	{
		get => Registry.GetValue( _appKeyUm, "Server3", "" )?.ToString() ?? ""; set => Registry.SetValue( _appKeyUm, "Server3", value, RegistryValueKind.String );
	}

	public static string TempFolder
	{
		get => Registry.GetValue( _appKeyUm, "TempFolder", "" )?.ToString() ?? ""; set => Registry.SetValue( _appKeyUm, "TempFolder", value, RegistryValueKind.String );
	}

	public static string UpdateFolder
	{
		get => Registry.GetValue( _appKeyUm, "LocalPath", "" )?.ToString() ?? ""; set => Registry.SetValue( _appKeyUm, "LocalPath", value, RegistryValueKind.String );
	}

	public static ZumUpdateMode UpdateMode
	{
		get
		=> ( ZumUpdateMode ) Convert.ToInt32( Registry.GetValue( _appKeyUm, "UpdateMode", "" ) );

		set => Registry.SetValue( _appKeyUm, "UpdateMode", ( int ) value, RegistryValueKind.DWord );
	}

	public static bool UseBulkLatestDate
	{
		get => Convert.ToBoolean( Registry.GetValue( _appKeyUm, "UseBulkLatestDate", "" ) );
		set => Registry.SetValue( _appKeyUm, "UseBulkLatestDate", Convert.ToInt32( value ), RegistryValueKind.DWord );
	}

	public static string Username
	{
		get => Registry.GetValue( _appKeyZeus, "Username", "" )?.ToString() ?? "";
		set => Registry.SetValue( _appKeyZeus, "Username", value, RegistryValueKind.String );
	}

	public static void Execute()
	{
		var zum = Process.Start( AppPath, "/auto" );
		zum.WaitForExit();
	}

	public static void ExecuteLocal( DateTime dteDate, ZumUpdateMode mode = ZumUpdateMode.Standard )
	{
		ConnectionType = ZumConnection.DirectFileAccess;
		UpdateMode = mode;
		FileDate = dteDate;
		Execute();
	}

	public static void ExecuteRelease( DateTime dteDate, ZumConnection connection = ZumConnection.Internet )
	{
		UpdateMode = ZumUpdateMode.Release;
		ConnectionType = connection;
		ReleaseDate = dteDate;
		Execute();
	}

	public static List<string> GetLogErrors( DateTime dteDate )
	{
		var errors = new List<string>();
		var fileName = $"ZUMLOG{dteDate:yyyyMMdd}.txt";
		var filePath = Path.Combine( LogFolder, fileName );
		if ( !File.Exists( filePath ) )
		{
			errors.Add( "Error: File doesn't exist" );
			return errors;
		}

		var fileLines = File.ReadAllLines( filePath );
		foreach ( var line in fileLines )
		{
			if ( line.Contains( "error opening file:", StringComparison.InvariantCultureIgnoreCase ) )
			{
				continue;
			}
			else if ( line.Contains( "error coping file.", StringComparison.InvariantCultureIgnoreCase ) )
			{
				continue;
			}
			else if ( line.Contains( "ERROR", StringComparison.InvariantCultureIgnoreCase ) )
			{
				errors.Add( line.Trim() );
			}
		}

		return errors;
	}

	public static void SetDefaultSettings( string user = "", string pwd = "" )
	{
		UpdateMode = ZumUpdateMode.Standard;
		Country = ZumCountry.Mexico;
		LogFolder = @"C:\Cyrnel\Logs\";
		TempFolder = @"C:\Cyrnel\Temp\";
		ConnectionType = ZumConnection.Internet;
		Username = string.IsNullOrEmpty( user ) ? "riskconsult" : user;
		Password = string.IsNullOrEmpty( pwd ) ? "R1$k2o19" : pwd;
		Server1 = "www.riskcodataserver.com";
		Server2 = "www.cyrneldataserver.com";
		Server3 = "www.riskcodataserver.com";
		Port = 80L;
		UpdateFolder = @"C:\Cyrnel\UPD\";
		DbSource = ZumDbSource.Sql;
		DateDelim = "'";
	}

	public static void VerifyZumUpdate( ILogger? log = null )
	{
		log?.LogInformation( "Verifying if Zeus is updated..." );

		try
		{
			List<string> errors = [];
			DateTime lastUpdate = DateTime.MinValue;
			DateTime lastDate = DateTime.Today.GetBusinessDateAdd( DateUnit.Day, -1 );
			do
			{
				errors = GetLogErrors( DateTime.Today );
				if ( errors.Count > 0 )
				{
					errors.ForEach( e => log?.LogError( "- ZUM UPDATE ERROR: {e}", e ) );
				}

				lastUpdate = Data.DbZeus.Db.Dates.EndDate;
				if ( lastUpdate != lastDate )
				{
					log?.LogError( $"- ZUM UPDATE ERROR: Zeus is not updated" );
				}

				if ( errors.Count > 0 || lastUpdate != lastDate )
				{
					log?.LogInformation( "- Executing Re-release for {lastDate}", lastDate.ToShortDateString() );
					ExecuteRelease( lastDate, ZumConnection.Internet );
					Thread.Sleep( 10000 );
				}
			} while ( errors.Count > 0 || lastUpdate != lastDate );
		}
		catch ( Exception ex )
		{
			throw new Exception( $"VerifyZumUpdate - {ex}" );
		}
	}
}
