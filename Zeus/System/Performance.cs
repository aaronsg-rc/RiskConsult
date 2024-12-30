using RiskConsult.Extensions;
using RiskConsult.Interop;
using RiskConsult.Zeus.Files;
using System.Windows.Automation;
using System.Windows.Forms;

namespace RiskConsult.Zeus.System;

/// <summary> Parámetros necesarios para poder ejecutar un perfat </summary>
public class Performance
{
	/// <summary> Ventana de configuración del performance </summary>
	private AutomationElement? _wizardWindow;

	/// <summary> Ruta completa al archivo Factor Contribution </summary>
	public FactorContributionFile? FactorContributionFile
	{
		get; private set;
	}

	/// <summary> Ruta completa al archivo Over Under </summary>
	public OverUnderFile? OverUnderFile
	{
		get; private set;
	}

	/// <summary> Parámetros configurables del performance </summary>
	public PerformanceSettings Settings
	{
		get; set;
	}

	/// <summary> Ruta completa al archivo Summary </summary>
	public PerfatSummaryFile? SummaryFile
	{
		get; private set;
	}

	private static ZeusSystem ZeusSystem => ZeusSystem.Instance;

	/// <summary> Constructor básico </summary>
	public Performance()
	{
		Settings = new PerformanceSettings();
	}

	public Performance( PerformanceSettings settings ) : this()
	{
		Run( settings );
	}

	public Performance( PerformanceSettings settings, string exportDirectory, string fileNameModifier = "" ) : this()
	{
		var sumPath = Path.Combine( exportDirectory, $"{settings.Portfolio}_Perfat_Summary{fileNameModifier}.xls" );
		var fcPath = Path.Combine( exportDirectory, $"{settings.Portfolio}_Perfat_FactorCont{fileNameModifier}.xls" );
		var ouPath = Path.Combine( exportDirectory, $"{settings.Portfolio}_Perfat_OverUnder{fileNameModifier}.xls" );
		if ( File.Exists( sumPath ) && File.Exists( fcPath ) && File.Exists( ouPath ) )
		{
			Settings = settings;
			SummaryFile = new PerfatSummaryFile( sumPath );
			FactorContributionFile = new FactorContributionFile( fcPath );
			OverUnderFile = new OverUnderFile( ouPath );
		}
		else
		{
			Run( settings, exportDirectory, fileNameModifier );
		}
	}

	public void ExportAll( string exportDirectory, string fileNameModifier = "" )
	{
		ExportFactorContribution( exportDirectory, fileNameModifier );
		ExportOverUnder( exportDirectory, fileNameModifier );
		ExportSummary( exportDirectory, fileNameModifier );
	}

	public void ExportFactorContribution( string exportDirectory, string fileNameModifier = "" )
	{
		if ( string.IsNullOrEmpty( exportDirectory ) )
		{
			throw new ArgumentNullException( nameof( exportDirectory ) );
		}

		if ( fileNameModifier != "" )
		{
			fileNameModifier = "_" + fileNameModifier;
		}

		ZeusSystem.CloseTabsByName( "Untitled:2" );

		// Exporto y cargo archivo factor contribution
		ZeusSystem.MenuAccess( "View>Factor Contribution" );
		_ = ZeusSystem.WorkArea.UntilFindElement( "Untitled:2", AutomationElement.NameProperty ) ??
			throw new Exception( "Unable to access factor contribution zeus tab" );
		var filePath = Path.Combine( exportDirectory, $"{Settings.Portfolio}_Perfat_FactorCont{fileNameModifier}.xls" );
		ZeusSystem.ExportData( "Untitled:2", filePath );
		ZeusSystem.CloseTabsByName( "Untitled:2" );
		FactorContributionFile = new FactorContributionFile( filePath );
	}

	/// <summary> Permite exportar resultados del perfat </summary>
	/// <param name="exportDirectory"> Carpeta en la que e va a guardar el archivo de resultados </param>
	/// <param name="fileNameModifier"> Modificador de nombre de archivo </param>
	public void ExportOverUnder( string exportDirectory, string fileNameModifier = "" )
	{
		if ( string.IsNullOrEmpty( exportDirectory ) )
		{
			throw new ArgumentNullException( nameof( exportDirectory ) );
		}

		if ( fileNameModifier != "" )
		{
			fileNameModifier = "_" + fileNameModifier;
		}

		// Exporto y cargo archivo over under
		ZeusSystem.CloseTabsByName( "Untitled:2" );
		ZeusSystem.MenuAccess( "View>Security>Over/Under" );
		_ = ZeusSystem.WorkArea.UntilFindElement( "Untitled:2", AutomationElement.NameProperty ) ??
			throw new Exception( "Unable to access over under zeus tab" );
		var filePath = Path.Combine( exportDirectory, $"{Settings.Portfolio}_Perfat_OverUnder{fileNameModifier}.xls" );
		ZeusSystem.ExportData( "Untitled:2", filePath );
		ZeusSystem.CloseTabsByName( "Untitled:2" );
		OverUnderFile = new OverUnderFile( filePath );
	}

	public void ExportSummary( string exportDirectory, string fileNameModifier = "" )
	{
		if ( string.IsNullOrEmpty( exportDirectory ) )
		{
			throw new ArgumentNullException( nameof( exportDirectory ) );
		}

		if ( fileNameModifier != "" )
		{
			fileNameModifier = "_" + fileNameModifier;
		}

		// Exporto y cargo archivo over under
		var filePath = Path.Combine( exportDirectory, $"{Settings.Portfolio}_Perfat_Summary{fileNameModifier}.xls" );
		ZeusSystem.MenuAccess( "View>Summary" );
		ZeusSystem.ExportData( "Untitled: Performance Summary", filePath );
		SummaryFile = new PerfatSummaryFile( filePath );
	}

	/// <summary> Ejecuta el perfat </summary>
	public void Run()
	{
		// Reinicio variables
		FactorContributionFile = default;
		OverUnderFile = default;
		SummaryFile = default;

		// Ejecuto performance
		ZeusSystem.CloseAll();
		ZeusSystem.NewFile( ZeusFileTypes.Performance );
		SetPerfatSettings( Settings );
		ZeusSystem.MenuAccess( "Tools>Run" );

		// Quitado para validar que funcione sin buscar ventana de progreso
		// Espero a que inicie ventana de progreso
		//_ = ZeusSystem.UI.UntilFindElement( "Progress", AutomationElement.NameProperty, TreeScope.Children );

		// Defino elemento que aseguran conclusión del performance
		AutomationElement? progress, sumSheet, errWindow;
		do
		{
			sumSheet = ZeusSystem.WorkArea.FindElement( "Untitled: Performance Summary", AutomationElement.NameProperty );
			progress = ZeusSystem.UI.FindElement( "Progress", AutomationElement.NameProperty, TreeScope.Children );
			errWindow = progress?.FindElement( "ZEUS", AutomationElement.NameProperty );
			if ( errWindow != null )
			{
				throw new Exception( "No se logró ejecutar correctamente, valida los parámetros o que existan precios" );
			}
		}
		while ( progress != null || sumSheet == null );
		Thread.Sleep( 300 );
	}

	public void Run( PerformanceSettings settings )
	{
		Settings = settings;
		Run();
	}

	public void Run( PerformanceSettings settings, string exportDirectory, string fileNameModifier = "" )
	{
		Run( settings );
		ExportAll( exportDirectory, fileNameModifier );
	}

	/// <summary> Accedo a sección de propiedades del perfat </summary>
	private void OpenWizardWindow()
	{
		do
		{
			// Hago foco en interfaz
			ZeusSystem.UI.SetWindowFocus();
			//_ = AutomationExtensions.SetForegroundWindow( Process.Id );

			// Entro a propiedades
			SendKeys.SendWait( "%{ENTER}" );

			// Referencia a ventana del wizard
			_wizardWindow = ZeusSystem.UI.UntilFindElement( "Performance Case Wizard", AutomationElement.NameProperty, TreeScope.Children, 1000 );
		} while ( _wizardWindow == null );

		// Referencia a botón next
		AutomationElement btnNext = _wizardWindow.FindElement( ZeusAutoIds.Button_Next ) ??
			throw new Exception( "Unable to find next button on perfat wizard window" );
		btnNext.Invoke();
	}

	/// <summary> Asigna el set de datos de configuración para poder ser ejecutado </summary>
	private void SetPerfatSettings( PerformanceSettings settings )
	{
		// Abro ventana de configuración
		OpenWizardWindow();
		if ( _wizardWindow == null )
		{
			throw new Exception( "Unable to access performance wizard window" );
		}

		//Asigno configuración
		var strModel = settings.Model.GetName();

		AutomationElement aeName = _wizardWindow.FindElement( PerfatAutoIds.Name, scope: TreeScope.Descendants ) ??
			throw new Exception( "Unable to find performance settings name object" );
		aeName.SetValue( settings.Name );

		AutomationElement aeModel = _wizardWindow.FindElement( PerfatAutoIds.Model, scope: TreeScope.Descendants ) ??
			throw new Exception( "Unable to find performance settings model object" );
		aeModel.SelectListBoxValue( strModel );

		AutomationElement aeBenchmark = _wizardWindow.FindElement( PerfatAutoIds.Benchmark, scope: TreeScope.Descendants ) ??
			throw new Exception( "Unable to find performance settings benchmark object" );
		aeBenchmark.SelectListBoxValue( settings.Benchmark );

		AutomationElement aeCurrency = _wizardWindow.FindElement( PerfatAutoIds.Currency, scope: TreeScope.Descendants ) ??
			throw new Exception( "Unable to find performance settings currency object" );
		aeCurrency.SelectListBoxValue( settings.Currency );

		AutomationElement aeStartDate = _wizardWindow.FindElement( PerfatAutoIds.StartDate, scope: TreeScope.Descendants ) ??
			throw new Exception( "Unable to find performance settings start date object" );
		aeStartDate.SetDatePickerValue( settings.StartDate );

		AutomationElement aeEndDate = _wizardWindow.FindElement( PerfatAutoIds.EndDate, scope: TreeScope.Descendants ) ??
			throw new Exception( "Unable to find performance settings end date object" );
		aeEndDate.SetDatePickerValue( settings.EndDate );

		// Clic siguiente
		AutomationElement aeNext = _wizardWindow.FindElement( ZeusAutoIds.Button_Next ) ??
			throw new Exception( "Unable to find performance settings next object" );
		aeNext.Invoke();

		// Asignamos portafolio
		AutomationElement aePortfolio = _wizardWindow.FindElement( PerfatAutoIds.Portfolio, scope: TreeScope.Descendants ) ??
			throw new Exception( "Unable to find performance settings portfolio object" );
		aePortfolio.SelectListBoxValue( settings.Portfolio );

		// Siguiente, siguiente y finalizar
		aeNext = _wizardWindow.FindElement( ZeusAutoIds.Button_Next ) ??
			throw new Exception( "Unable to find performance settings next object" );
		aeNext.Invoke();

		aeNext = _wizardWindow.FindElement( ZeusAutoIds.Button_Next ) ??
			throw new Exception( "Unable to find performance settings next object" );
		aeNext.Invoke();

		AutomationElement aeFinish = _wizardWindow.FindElement( ZeusAutoIds.Button_Finish, scope: TreeScope.Descendants ) ??
			throw new Exception( "Unable to find performance settings next object" );
		aeFinish.Invoke();

		// Espero a que cierre el wizard
		while ( ZeusSystem.UI.FindElement( "Performance Case Wizard", AutomationElement.NameProperty ) != null )
		{
		}

		Thread.Sleep( 200 );
	}
}
