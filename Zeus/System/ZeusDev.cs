using RiskConsult.Enumerators;
using RiskConsult.Extensions;
using System.Runtime.InteropServices;

namespace RiskConsult.Zeus.System;

public class ZeusDev : IDisposable
{
	public readonly dynamic ComObject;

	/// <summary> Tipo de la clase con PROGID = "Zeus.Dev" </summary>
	public readonly Type Type;

	private const string _clsId = "C65C0473-C001-4BFB-9E1F-7141B5D8A31F";
	private const string _progId = "Zeus.Dev";
	private static ZeusDev? _instance;

	public static ZeusDev Instance
	{
		get
		{
			_instance ??= new ZeusDev();
			return _instance;
		}
	}

	private ZeusDev()
	{
		Type = InitializeZeusType();
		ComObject = InitializeZeusDev( Type );
	}

	~ZeusDev() => Dispose();

	public void CloseDocument( int portfolioID )
	{
		ComObject.CloseDocument( portfolioID );
	}

	public void Dispose()
	{
		if ( ComObject != null )
		{
			Marshal.FinalReleaseComObject( ComObject );
		}

		_instance = null;
		GC.SuppressFinalize( this );
	}

	public object GetPortfolioAnalytic( int portfolioID, string analyticID )
	{
		return ComObject.GetPortfolioAnalytic( portfolioID, analyticID ) ?? string.Empty;
	}

	public object GetSecurityAnalytic( int portfolioID, string holdingId, ZeusIdType idType, string analyticID )
	{
		return ComObject.GetSecurityAnalytic( portfolioID, holdingId, idType.GetName(), analyticID ) ?? string.Empty;
	}

	public string GetSecurityCode( int portfolioID, ZeusIdType idType, int holdingIndex )
	{
		return ( string ) ComObject.GetSecurityCode( portfolioID, idType.GetName(), holdingIndex ) ?? string.Empty;
	}

	public int OpenDocument( string portfolioName, int portfolioSource )
	{
		return ComObject.OpenDocument( portfolioName, portfolioSource );
	}

	private static dynamic InitializeZeusDev( Type type )
	{
		try
		{
			return Activator.CreateInstance( type ) ?? throw new Exception( "Revisa que se encuentre registrada" );
		}
		catch ( Exception e )
		{
			throw new Exception( $"Error al crear una instancia de la clase Zeus.Dev: {e.Message}", e );
		}
	}

	private static Type InitializeZeusType()
	{
		try
		{
			var type = Type.GetTypeFromProgID( _progId );
			if ( type != null )
			{
				return type;
			}

			var guid = new Guid( _clsId );
			type = Type.GetTypeFromCLSID( guid, true );
			return type ?? throw new InvalidOperationException( "No se encuentra registrado el ProgID ni ClSID" );
		}
		catch ( Exception e )
		{
			throw new Exception( $"Error al obtener el tipo Zeus.Dev: {e.Message}", e );
		}
	}
}
