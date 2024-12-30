using RiskConsult.Core;
using RiskConsult.Enumerators;
using RiskConsult.Interop;
using RiskConsult.Zeus.Files;
using System.ComponentModel;
using System.Windows.Automation;
using System.Windows.Forms;

namespace RiskConsult.Zeus.System;

/// <summary> Clase que interactua con Zeus para extraer información de un portafolio </summary>
public sealed class Portfolio : IDisposable
{
	/// <summary> Fecha en que se abrió el portafolio </summary>
	public DateTime Date { get; private set; }

	/// <summary> Número de instrumentos que tiene el portafolio </summary>
	public int HoldingsCount { get; private set; }

	/// <summary> Número de portafolio, referencia para Zeus </summary>
	public int ID { get; private set; }

	/// <summary> Nombre completo con el que se abre un portafolio </summary>
	public string InputName { get; private set; }

	/// <summary> Nombre del portafolio </summary>
	public string Name { get; private set; }

	/// <summary> Define si se abrio desde base de datos o un archivo </summary>
	public ZeusOpenSource OpenSource { get; private set; }

	/// <summary> Archivo .ps con algunas propiedades </summary>
	public PsFile? PsFile => ZpFile?.PsFile;

	/// <summary> Nombre del portafolio </summary>
	public string SourceName { get; private set; }

	/// <summary> Valor ($) del portafolio </summary>
	public double Value { get; private set; }

	public ZpFile? ZpFile { get; private set; }
	private static ZeusSystem ZeusSystem => ZeusSystem.Instance;

	/// <summary> Constructor y asigna valores default </summary>
	public Portfolio() : base()
	{
		Date = DateTime.MinValue;
		HoldingsCount = 0;
		ID = -1;
		InputName = "";
		Name = "";
		OpenSource = ZeusOpenSource.Invalid;
		SourceName = "";
		Value = 0;
		ZpFile = null;
	}

	/// <summary> Constructor que permite abrir de forma directa el portafolio </summary>
	/// <param name="portfolio"> Nombre del portafolio en base de datos o ruta al archivo zp </param>
	public Portfolio( string portfolio ) : this()
	{
		Open( portfolio );
	}

	/// <summary> Constructor que permite abrir de forma directa el portafolio en una fecha específica </summary>
	/// <param name="portfolio"> Nombre del portafolio en base de datos o ruta al archivo zp </param>
	/// <param name="dteDate"> Día hábil en que se quiere abrir el portafolio </param>
	public Portfolio( string portfolio, DateTime dteDate ) : this()
	{
		ZeusSystem.ChangeDate( dteDate );
		Open( portfolio );
	}

	/// <summary> Cambia la fecha de Zeus sin abrir ningún portafolio </summary>
	public Portfolio( DateTime dteDate ) : this()
	{
		ZeusSystem.ChangeDate( dteDate );
	}

	/// <summary> En Zeus cierra el portafolio </summary>
	public void Close() => ZeusSystem.ZeusDev.CloseDocument( ID );

	public void Dispose() => Close();

	public void ExportCashFlow( string directory, string fileNameModifier = "", bool exportAsHtml = false )
	{
		CheckExport( directory, "Cashflow", fileNameModifier, exportAsHtml,
			out var filePath,
			out var tabName );
		ZeusSystem.MenuAccess( "View>Portfolio>Cashflow" );
		ZeusSystem.ExportData( tabName, filePath, exportAsHtml );
	}

	public void ExportDrillDown( string directory,
				string filter1, string filter2, string filter3, string filter4, string fileNameModifier = "" )
				=> ExportDrillDown( directory, [ filter1, filter2, filter3, filter4 ], fileNameModifier );

	/// <summary> Asigna un set de valores a la pestaña Drill-Down dentro de Zeus </summary>
	/// <param name="values"> Array unidimensional con 4 strings con los parámetros para configurar el Drill-Down </param>
	public void ExportDrillDown( string directory, string[] values, string fileNameModifier = "" )
	{
		if ( values == null || values.Length != 4 )
		{
			throw new ArgumentException( "DrillDown values must have 4 elements" );
		}

		// Array de Ids DrillDown
		var arrDrillDown = new[] {
				ZeusAutoIds.List_DrillDown_1,
				ZeusAutoIds.List_DrillDown_2,
				ZeusAutoIds.List_DrillDown_3,
				ZeusAutoIds.List_DrillDown_4 };

		CheckExport( directory, "Drill-Dow", fileNameModifier, false,
			out var filePath,
			out var tabName );
		ZeusSystem.MenuAccess( "View>Portfolio>Drill-Down" );
		AutomationElement drillDownTab =
			ZeusSystem.WorkArea.UntilFindElement( tabName, AutomationElementIdentifiers.NameProperty ) ??
			throw new Exception( "Unable to find DrillDown tab" );

		// Asignamos valores a cada ListBox
		for ( var i = 0; i < values.Length; i++ )
		{
			AutomationElement listDrillDown =
				drillDownTab.FindElement( arrDrillDown[ i ], null, TreeScope.Descendants ) ??
				throw new Exception( $"Unable to find List_DrillDown_{i}" );

			if ( listDrillDown.Current.Name != values[ i ] )
			{
				listDrillDown.SelectListBoxValue( values[ i ] );
			}
		}

		ZeusSystem.ExportData( tabName, filePath, false );
	}

	public void ExportExposures( string directory, ExposureAnalyticType exportType, string fileNameModifier = "" )
		=> ExportExposures( directory, exportType, ExposureFactorType.All, fileNameModifier );

	/// <summary> Exporta una o todas las hojas de resultado según parámetros </summary>
	/// <param name="directory"> Ruta de carpeta destino </param>
	/// <param name="exportType"> Tipo de datos generados </param>
	/// <param name="factorGroup"> Tipo de factor a exportar, si nó se especifica se exportará todo </param>
	public void ExportExposures( string directory, ExposureAnalyticType exportType, ExposureFactorType factorGroup, string fileNameModifier = "" )
	{
		// Abro pestaña de exposiciones
		CheckExport( directory, "Exposures", fileNameModifier, false,
			out var _,
			out var tabName );
		SendKeys.SendWait( "{F4}" );

		//Obtengo referencias
		AutomationElement exposureTab =
			ZeusSystem.WorkArea.FindElement( tabName, AutomationElementIdentifiers.NameProperty ) ??
			throw new Exception( $"Unable to find {tabName} on Zeus" );
		AutomationElement listExportType =
			exposureTab.FindElement( ZeusAutoIds.List_Exposure_ExposureType ) ??
			throw new Exception( "Unable to find list of exposures types object" );
		AutomationElement listFactorTypes =
			exposureTab.FindElement( ZeusAutoIds.List_Exposure_FactorType ) ??
			throw new Exception( "Unable to find list of factor group types object" );

		//Asigno tipo de analítico
		var expTypeName = GetExposureTypeName( exportType );
		listExportType.SelectListBoxValue( expTypeName );

		// Caso único
		var factorGroupName = GetFactorTypeName( factorGroup );
		var filePath = Path.Combine( directory, $"{Name}_{exportType}_{factorGroup}{fileNameModifier}.xls" );
		if ( factorGroup != ExposureFactorType.All )
		{
			listFactorTypes.SelectListBoxValue( factorGroupName );
			ZeusSystem.ExportData( tabName, filePath );
			return;
		}

		//Caso para todos los grupos de factores
		Array factorTypes = Enum.GetValues( typeof( ExposureFactorType ) );
		foreach ( ExposureFactorType factorType in factorTypes )
		{
			if ( factorType == ExposureFactorType.All )
			{
				continue;
			}

			factorGroupName = GetFactorTypeName( factorType );
			filePath = Path.Combine( directory, $"{Name}_{exportType}_{factorType}{fileNameModifier}.xls" );
			listFactorTypes.SelectListBoxValue( factorGroupName );
			ZeusSystem.ExportData( tabName, filePath );
		}
	}

	public void ExportHorizonAnalysis( string directory, string fileNameModifier = "" )
	{
		CheckExport( directory, "Horizon", fileNameModifier, false,
			out var filePath,
			out var tabName );
		ZeusSystem.MenuAccess( "View>Portfolio>Horizon Analysis" );
		ZeusSystem.ExportData( tabName, filePath, false );
	}

	/// <summary> Exporta los resultados del portafolio </summary>
	/// <param name="directory"> Ruta de la carpeta a la que se quiere exportar </param>
	/// <param name="fileNameModifier"> Modificador de nombre de archivo PortName_TabName_NameModif.xls </param>
	/// <param name="exportAsHtml"> Determina si se exporta como html (verdadero) o xls (falso) </param>
	public void ExportMain( string directory, string fileNameModifier = "", bool exportAsHtml = false )
	{
		CheckExport( directory, "Main", fileNameModifier, exportAsHtml,
			out var filePath,
			out var tabName );
		ZeusSystem.ExportData( tabName, filePath, exportAsHtml );
	}

	public void ExportSummary( string directory, string fileNameModifier = "" )
	{
		CheckExport( directory, "Portfolio Summary", fileNameModifier, false,
			out var filePath,
			out var tabName );
		SendKeys.SendWait( "{F3}" );
		ZeusSystem.ExportData( tabName, filePath, false );
	}

	/// <summary> Extraé el valor del analítico de un instrumento en el portafolio desde el ticker </summary>
	/// <param name="ticker"> Identificador del instrumento </param>
	/// <param name="analyticID"> Nombre del analítico </param>
	public object? GetHoldingAnalytic( string ticker, string analyticID )
	{
		if ( string.IsNullOrEmpty( ticker ) || string.IsNullOrEmpty( analyticID ) )
		{
			return default;
		}

		var analytic = ZeusSystem.ZeusDev.GetSecurityAnalytic( ID, ticker, ZeusIdType.Ticker, analyticID );
		return analytic == null || analytic == DBNull.Value ? default : analytic;
	}

	public List<FactorValue> GetHoldingExposures( string ticker )
		=> GetHoldingExposures( ticker, [] );

	public List<FactorValue> GetHoldingExposures( string ticker, IEnumerable<Factor> factors )
	{
		if ( string.IsNullOrEmpty( ticker ) )
		{
			throw new ArgumentNullException( nameof( ticker ) );
		}

		ArgumentNullException.ThrowIfNull( factors );

		// Recorro la matriz de factores
		var exposures = new List<FactorValue>();
		foreach ( Factor factor in factors )
		{
			var dblExp = Convert.ToDouble( GetHoldingAnalytic( ticker, $"EXP_{factor.Name}" ) );
			if ( dblExp == 0 )
			{
				continue;
			}

			if ( factor.FactorTypeId == FactorTypeId.FT_INDUSTRY )
			{
				dblExp /= 100;
			}

			exposures.Add( new FactorValue() { Factor = factor, Value = dblExp } );
		}

		return exposures;
	}

	/// <summary> Extrae el identificador del instrumento n en el portafolio </summary>
	/// <param name="holdingIndex"> Índice del instrumento n dentro del portafolio </param>
	public string GetHoldingTicker( int holdingIndex )
		=> Convert.ToString( ZeusSystem.ZeusDev.GetSecurityCode( ID, ZeusIdType.Ticker, holdingIndex ) );

	/// <summary> Extrae el analítico de un portafolio </summary>
	/// <param name="analyticID"> Nombre del analítico a extraer </param>
	/// <returns> Valor del analítico del portafolio </returns>
	public string GetPortfolioAnalytic( string analyticID )
	{
		if ( string.IsNullOrEmpty( analyticID ) )
		{
			return string.Empty;
		}

		var analytic = ZeusSystem.ZeusDev.GetPortfolioAnalytic( ID, analyticID );
		return analytic.ToString() ?? string.Empty;
	}

	public List<FactorValue> GetPortfolioExposures()
		=> GetPortfolioExposures( [] );

	public List<FactorValue> GetPortfolioExposures( IEnumerable<Factor> factors )
	{
		ArgumentNullException.ThrowIfNull( factors );

		// Recorro la matriz de factores
		var exposures = new List<FactorValue>();
		foreach ( Factor factor in factors )
		{
			var dblExp = Convert.ToDouble( GetPortfolioAnalytic( $"EXP_{factor.Name}" ) );
			if ( dblExp == 0 )
			{
				continue;
			}

			if ( factor.FactorTypeId == FactorTypeId.FT_INDUSTRY )
			{
				dblExp /= 100;
			}

			exposures.Add( new FactorValue() { Factor = factor, Value = dblExp } );
		}

		return exposures;
	}

	/// <summary> Abre un portafolio en ZEUS y extraé algunas propiedades </summary>
	/// <param name="portfolio_name"> Ruta o nombre del portafolio sobre el que se desea abrir </param>
	public void Open( string portfolio_name )
	{
		if ( string.IsNullOrEmpty( portfolio_name ) )
		{
			throw new ArgumentNullException( nameof( portfolio_name ) );
		}

		ZeusOpenSource portfolioSource =
			File.Exists( portfolio_name )
			? ZeusOpenSource.FromFile
			: ZeusOpenSource.FromDB;
		var sourceName =
			portfolioSource == ZeusOpenSource.FromFile
			? Path.GetFileName( portfolio_name )
			: portfolio_name;
		ZpFile? zpFile =
			portfolioSource == ZeusOpenSource.FromFile
			? new ZpFile( portfolio_name )
			: null;

		var portfolioID = ZeusSystem.ZeusDev.OpenDocument( portfolio_name, ( int ) portfolioSource );

		// Asigno propiedades del portafolio
		ID = portfolioID;
		Date = Convert.ToDateTime( GetPortfolioAnalytic( "Date" ) );
		HoldingsCount = Convert.ToInt32( GetPortfolioAnalytic( "NumberOfSecurities" ) );
		InputName = portfolio_name;
		Name = GetPortfolioAnalytic( "Name" );
		OpenSource = portfolioSource;
		SourceName = sourceName;
		Value = Convert.ToDouble( GetPortfolioAnalytic( "Value" ) );
		ZpFile = zpFile;
	}

	public override string ToString() => $"{Name}[{HoldingsCount}]";

	/// <summary> Cierra el portafolio y vuelve a abrirlo para actualizar parámetros en caso de que se modifique el PS </summary>
	public void Update()
	{
		Close();
		Open( InputName );
	}

	/// <summary> Obtiene el string correspondiente al exposure type seleccionado </summary>
	private static string GetExposureTypeName( ExposureAnalyticType zeusTab )
	{
		return zeusTab switch
		{
			ExposureAnalyticType.CTR => "Contribution to Risk",
			ExposureAnalyticType.MC_Risk => "MC Risk",
			ExposureAnalyticType.Exposure => "FactorValue",
			ExposureAnalyticType.Sensitivity => "Sensitivity",
			ExposureAnalyticType.Volatility => "Factor Volatility",
			_ => throw new InvalidEnumArgumentException( $"{zeusTab} is not an exposure tab type" ),
		};
	}

	/// <summary> Obtiene el string correspondiente al factor type seleccionado </summary>
	private static string GetFactorTypeName( ExposureFactorType FactorType )
	{
		return FactorType switch
		{
			ExposureFactorType.Curve => "Interest Rate",
			ExposureFactorType.Spread => "Spread",
			ExposureFactorType.Style => "Style",
			ExposureFactorType.Industry => "Industry",
			ExposureFactorType.Index => "Index",
			ExposureFactorType.All => "All",
			_ => throw new Exception( "Factor type no definido." ),
		};
	}

	private void CheckExport( string directory, string tab, string fileNameModifier, bool exportAsHtml,
																										out string filePath,
		out string tabName )
	{
		ZeusSystem.ActivateTab( $"{SourceName}: Main" );
		if ( fileNameModifier != "" )
		{
			fileNameModifier = $"_{fileNameModifier}";
		}

		var extension = exportAsHtml ? "html" : "xls";
		var tabModifier =
			tab == "Portfolio Summary" ? "Summary" :
			tab == "Drill-Down" ? "DrillDown" : tab;
		var fileName = $"{Name}_{tabModifier}{fileNameModifier}.{extension}";

		tabName = $"{SourceName}: {tab}";
		filePath = Path.Combine( directory, fileName );
	}
}
