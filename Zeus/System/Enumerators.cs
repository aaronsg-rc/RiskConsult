namespace RiskConsult.Zeus.System;

/// <summary> AutomationIDs de objetos en ventana de backtesting </summary>
internal static class BackAutoIds
{
	public const string Button_Accept = "2";
	public const string Button_Cancel = "2";
	public const string Button_OK = "1";
	public const string Date_End = "3380";
	public const string Date_Start = "3378";
	public const string List_Portfolio = "6065";
	public const string Txt_Finished = "65535";
	public const string TxtBx_Analytic = "61783";
	public const string TxtBx_Path = "6108";
}

/// <summary> AutomationID's identificados dentro de la interfaz </summary>
internal readonly struct ZeusAutoIds
{
	public const string Button_Back = "12323";
	public const string Button_Cancel = "2";
	public const string Button_Edit = "3351";
	public const string Button_Export = "6054";
	public const string Button_Finish = "12325";
	public const string Button_Help = "9";
	public const string Button_Import = "3528";
	public const string Button_Next = "12324";
	public const string Button_OK = "1";
	public const string List_DrillDown_1 = "6036";
	public const string List_DrillDown_2 = "2223";
	public const string List_DrillDown_3 = "2212";
	public const string List_DrillDown_4 = "3348";
	public const string List_Exposure_ExposureType = "2212";
	public const string List_Exposure_FactorType = "3730";
	public const string List_NewFile = "2259";
	public const string Pane_SystemDate = "2299";
	public const string Pane_WorkArea = "59648";
	public const string Tab_Area = "12320";
	public const string TxtBx_ExportPath = "1107";
}

/// <summary> Tipo de información que se pueden generar para los factores desde Exposures(F4) </summary>
public enum ExposureType
{
	Exposure,
	MC_Risk,
	CTR,
	Sensitivity,
	FactorVolatility
}

/// <summary> Tipo usado para definir resultados que se pueden exportar de un portafolio </summary>
public enum ZeusExportableTabs
{
	None,
	Summary,
	Main,
	HorizonAnalysis,
	Cashflow
}

/// <summary> Tipos de factores que se pueden seleccionar en Exposures(F4) </summary>
public enum ExposureFactorType
{
	Curve,
	Spread,
	Style,
	Industry,
	Index,
	All
}

public enum ExposureAnalyticType
{
	Exposure,
	CTR,
	Sensitivity,
	MC_Risk,
	Volatility
}

/// <summary> Tipo usado para la creación de nuevos archivos. </summary>
public enum ZeusFileTypes
{ Portfolio, Performance }

/// <summary> Origen de apertura del portafolio </summary>
public enum ZeusOpenSource
{
	Invalid = -1,
	FromDB = 2,
	FromFile = 3
}

/// <summary> AutomationIDs de ventana de perfat </summary>
internal struct PerfatAutoIds
{
	public const string Benchmark = "6081";
	public const string Currency = "2372";
	public const string EndDate = "3380";
	public const string Model = "1143";
	public const string Name = "6163";
	public const string Portfolio = "6065";
	public const string StartDate = "3378";
}

public enum ZumConnection
{
	Internet = 0,
	DirectFileAccess = 1
}

public enum ZumCountry
{
	Mexico,
	Brasil
}

public enum ZumDbSource
{
	DataSource = 0,
	Sql = 2,
	Oracle
}

public enum ZumUpdateMode
{
	Standard = 0,
	Release = 1,
	BulkCopy = 2
}
