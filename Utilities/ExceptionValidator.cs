namespace RiskConsult.Utilities;

public static class ExceptionManager
{
	public static void ThrowIfDirectoryNotFound( string? path )
	{
		var directory = Path.GetDirectoryName( path );
		if ( string.IsNullOrEmpty( directory ) )
		{
			throw new DirectoryNotFoundException( "Directory not found" );
		}
	}

	public static void ThrowIfFileNotFound( string? path )
	{
		if ( File.Exists( path ) == false )
		{
			throw new FileNotFoundException( "File not found", Path.GetFileName( path ) );
		}
	}
}
