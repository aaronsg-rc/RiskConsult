using RiskConsult.Interop;
using RiskConsult.Zeus.Files;
using System.Windows.Automation;

namespace RiskConsult.Zeus.System;

/// <summary> Clase que permite ejecutar un backtesting desde Zeus </summary>
public class Backtest
{
	/// <summary> Permite visualizar la información de los resultados generados </summary>
	public BacktestFile? Results { get; private set; }

	/// <summary> Set de configuración necesario para poder arrancar el backtesting </summary>
	public BacktestSettings Settings { get; set; }

	private static ZeusSystem ZeusSystem => ZeusSystem.Instance;

	/// <summary> Constructor base </summary>
	public Backtest()
	{
		Settings = new BacktestSettings();
	}

	/// <summary> Constructor que asigna la configuración </summary>
	public Backtest( BacktestSettings settings )
	{
		Settings = settings;
		Run();
	}

	/// <summary> Constructor que asigna la configuración </summary>
	public Backtest( DateTime startDate, DateTime endDate, string portfolio, string analytic, string filePath )
		: this( new BacktestSettings( startDate, endDate, portfolio, analytic, filePath ) )
	{ }

	/// <summary> Ejecuta el backtesting en Zeus </summary>
	public void Run()
	{
		// Reinicio
		Results = null;

		// Cerramos pestañas
		ZeusSystem.CloseAll();

		// Accedemos al menú
		ZeusSystem.MenuAccess( "Tools>Batch>Backtesting..." );

		// Referencias de ventana de configuración del backtesting
		AutomationElement backtestWindow = ZeusSystem.UI.UntilFindElement( "Backtest Settings", AutomationElement.NameProperty ) ??
			throw new Exception( "Unable to get Backtest Settings Window" );
		AutomationElement aeDateStart = backtestWindow.FindElement( BackAutoIds.Date_Start ) ??
			throw new Exception( "Unable to find backtest 'Date Start' object" );
		AutomationElement aeDateEnd = backtestWindow.FindElement( BackAutoIds.Date_End ) ??
			throw new Exception( "Unable to find backtest 'Date End' object" );
		AutomationElement aePortfolio = backtestWindow.FindElement( BackAutoIds.List_Portfolio ) ??
			throw new Exception( "Unable to find backtest 'Portfolio' object" );
		AutomationElement aeAnalytic = backtestWindow.FindElement( BackAutoIds.TxtBx_Analytic ) ??
			throw new Exception( "Unable to find backtest 'Analytic' object" );
		AutomationElement aePath = backtestWindow.FindElement( BackAutoIds.TxtBx_Path ) ??
			throw new Exception( "Unable to find backtest 'Path' object" );
		AutomationElement aeOK = backtestWindow.FindElement( BackAutoIds.Button_OK ) ??
			throw new Exception( "Unable to find backtest 'OK' object" );

		// Asignamos configuración
		aeDateStart.SetDatePickerValue( Settings.StartDate );
		aeDateEnd.SetDatePickerValue( Settings.EndDate );
		aePortfolio.SelectListBoxValue( Settings.Portfolio );
		aeAnalytic.SetValue( Settings.Analytic );
		aePath.SetValue( Settings.FilePath );
		aeOK.Invoke();

		// Espera hasta que concluya el proceso del backtesting y defino ventana de finalización
		AutomationElement finishWindow = ZeusSystem.UI.UntilFindElement( "ZEUS", AutomationElement.NameProperty ) ??
			throw new Exception( "Unable to find backtest 'Finish Window' object" );
		AutomationElement finishMessage = finishWindow.FindElement( BackAutoIds.Txt_Finished ) ??
			throw new Exception( "Unable to find backtest 'Finish Message' object" );
		AutomationElement finishButton = finishWindow.FindElement( BackAutoIds.Button_Accept ) ??
			throw new Exception( "Unable to find backtest 'Finish Accept Button' object" );

		// Según el mensaje de finalizado
		if ( finishMessage.Current.Name == "Concluded!" )
		{
			finishButton.Invoke();
		}
		else
		{
			throw new Exception( "Error running backtest verify configuration or prices." );
		}
	}

	/// <summary> Asigna la configuración y ejecuta el backtesting en Zeus </summary>
	public void Run( BacktestSettings settings )
	{
		Settings = settings;
		Run();
	}

	/// <summary> Ejecuto el backtesting de forma segura, lo que repite la operación hasta que las fechas del resultado sean correctas </summary>
	public void RunSecure()
	{
		do
		{
			Run();
			Results = new BacktestFile( Settings.FilePath );
		} while ( Results.StartDate != Settings.StartDate || Results.EndDate != Settings.EndDate );
	}

	/// <summary>
	/// Asigna la configuración y ejecuto el backtesting de forma segura, lo que repite la operación hasta que las fechas del resultado sean correctas
	/// </summary>
	public void RunSecure( BacktestSettings settings )
	{
		Settings = settings;
		RunSecure();
	}
}
