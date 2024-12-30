using RiskConsult.Interop;
using RiskConsult.Utilities;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Automation;

namespace RiskConsult.Zeus.System;

public class ZeusSystem : IDisposable
{
	/// <summary> Proceso ZEUS </summary>
	public readonly Process Process;

	/// <summary> Referencia a la interfaz, ventana principal del sistema </summary>
	public readonly AutomationElement UI;

	/// <summary> Área donde se ubica el contenido de la pestaña visualizada </summary>
	public readonly AutomationElement WorkArea;

	private static ZeusSystem? _instance;

	/// <summary> Acceso a la instancia activa de Zeus System </summary>
	public static ZeusSystem Instance
	{
		get
		{
			_instance ??= new ZeusSystem();
			return _instance;
		}
	}

	/// <summary> Objeto que permite el enlace con la aplicación con PROGID = "Zeus.Dev" </summary>
	public static ZeusDev ZeusDev => ZeusDev.Instance;

	/// <summary> Sección donde se ubican los nombres de las pestañas </summary>
	public AutomationElement? TabArea => UI.FindElement( ZeusAutoIds.Tab_Area );

	private ZeusSystem()
	{
		Process = GetZeusProcess();
		UI = GetZeusUi( Process );
		WorkArea = GetZeusWorkArea( UI );
	}

	~ZeusSystem() => Dispose();

	/// <summary> Activa la pestaña especificada dentro de Zeus </summary>
	/// <param name="name"> Nombre de la pestaña a activar </param>
	public void ActivateTab( string name )
	{
		try
		{
			AutomationElement tab = GetTab( name ) ?? throw new Exception( $"Invalid zeus tab name: {name}" );
			tab.Select();
		}
		catch ( Exception e )
		{
			throw new Exception( $"Error activating tab: {e.Message}", e );
		}
	}

	/// <summary> Cambia la fecha base de Zeus </summary>
	/// <param name="dteDate"> Día hábil al que se cambiará el sistema </param>
	public void ChangeDate( DateTime dteDate )
	{
		try
		{
			//Valido fecha de entrada
			if ( dteDate > DateTime.Today )
			{
				throw new Exception( "Invalid date, must be lower than today" );
			}

			// Cierro todas las pestañas abiertas
			CloseAll();

			// Loop hasta que la fecha sea correcta
			AutomationElement? dateElement;
			var currentDateFormat = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
			do
			{
				// Accedemos a menú opciones
				MenuAccess( "Tools>Options" );

				// Referencia a ventana de configuración
				AutomationElement settingsWindow = UI.UntilFindElement( "System Settings", AutomationElement.NameProperty ) ??
					throw new Exception( "Unable to find settingsWindow" );

				// Referencia a cuadro de fecha
				AutomationElement dateTimePicker = settingsWindow.FindElement( ZeusAutoIds.Pane_SystemDate, scope: TreeScope.Descendants ) ??
					throw new Exception( "Unable to find DateTimePicker on SettingsWindow" );
				dateTimePicker.SetDatePickerValue( dteDate );

				// Click OK
				AutomationElement okButton = settingsWindow.FindElement( ZeusAutoIds.Button_OK ) ??
					throw new Exception( "Unable to find OkButton on SettingsWindow" );
				okButton.Invoke( sleep: 2000 );

				// Obtengo referencia a fecha del sistema si esta coincide con mi cambio
				dateElement = UI.FindElement( dteDate.ToString( currentDateFormat ), AutomationElement.NameProperty, TreeScope.Descendants );
			}
			while ( dateElement == null );
		}
		catch ( Exception e )
		{
			throw new Exception( $"Error changing date: {e.Message}", e );
		}
	}

	/// <summary> Cierra todas las pestañas abiertas en Zeus </summary>
	public void CloseAll()
	{
		if ( TabArea == null )
		{
			return;
		}

		try
		{
			MenuAccess( "Window>Close All" );
		}
		catch ( Exception e )
		{
			throw new Exception( $"Error closing all tabs: {e.Message}", e );
		}
	}

	/// <summary> Cierra la pestaña en Zeus con el nombre especificado </summary>
	public void CloseTab( string name )
	{
		if ( string.IsNullOrEmpty( name ) )
		{
			return;
		}

		AutomationElement? tab = GetTab( name );
		if ( tab != null )
		{
			tab.Select();
			AutomationElement? close = WorkArea.FindElement( "Cerrar", AutomationElement.NameProperty, TreeScope.Descendants );
			close?.Invoke();
		}
	}

	/// <summary> Cierra todas las pestañas con el nombre proporcionado </summary>
	/// <param name="name"> Nombre de la pestaña </param>
	public void CloseTabsByName( string name )
	{
		AutomationElement? otherTab;
		do
		{
			CloseTab( name );
			otherTab = GetTab( name );
		} while ( otherTab != null );
	}

	public void Dispose()
	{
		ZeusDev.Dispose();
		GC.SuppressFinalize( this );
	}

	public void ExportData( AutomationElement tab, string filePath, bool exportAsHtml = false )
	{
		try
		{
			ExceptionManager.ThrowIfDirectoryNotFound( filePath );

			// Click a botón Exportar
			ActivateTab( tab.Current.Name );
			AutomationElement tabWindow = WorkArea.FindElement( tab.Current.Name.Trim(), AutomationElement.NameProperty ) ??
				throw new Exception( "Unable to find tab window" );
			AutomationElement exportButton = tabWindow.FindElement( ZeusAutoIds.Button_Export, scope: TreeScope.Descendants ) ??
				throw new Exception( "Unable to find export button" );
			exportButton.Invoke( true );

			// Creo referencia a ventana Export Grid Data
			AutomationElement exportGridData = UI.UntilFindElement( "Export Grid Data", AutomationElement.NameProperty ) ??
				throw new Exception( "Unable to find export grid data" );

			// Asignamos ruta destino
			AutomationElement pathTextBox = exportGridData.FindElement( ZeusAutoIds.TxtBx_ExportPath ) ??
				throw new Exception( "Unable to find path TextBox" );
			pathTextBox.SetValue( filePath );

			// Cambio opción HTML
			if ( exportAsHtml )
			{
				AutomationElement htmlOption = exportGridData.FindElement( "6040" ) ??
					throw new Exception( "Unable to find html option on export grid data" );
				htmlOption.Select();
			}

			// Invoco boton OK
			AutomationElement okButton = exportGridData.FindElement( ZeusAutoIds.Button_OK ) ??
				throw new Exception( "Unable to find ok button on export grid data" );
			okButton.Invoke();

			// Salgo hasta que el archivo esté creado
			while ( !File.Exists( filePath ) )
			{
				Thread.Sleep( 50 );
			}
		}
		catch ( Exception e )
		{
			throw new Exception( $"Error exporting data: {e.Message}", e );
		}
	}

	/// <summary> Exporta desde Zeus la pestaña especificada, debe estar abierta </summary>
	/// <param name="strTabName"> Nombre de la pestaña </param>
	/// <param name="filePath"> Ruta completa al archivo donde se va a guardar </param>
	public void ExportData( string strTabName, string filePath, bool exportAsHtml = false )
	{
		AutomationElement tab = GetTab( strTabName ) ?? throw new Exception( $"Invalid {nameof( strTabName )}: {strTabName}" );
		ExportData( tab, filePath, exportAsHtml );
	}

	/// <summary> Intenta obtener referencia a la pestaña con el nombre solicitado </summary>
	/// <returns> Referencia a pestaña o null si no se encuentra </returns>
	public AutomationElement? GetTab( string name )
	{
		if ( TabArea == null )
		{
			return null;
		}

		AutomationElementCollection tabsCollection = TabArea.FindAll( TreeScope.Children, Condition.TrueCondition );
		foreach ( AutomationElement tab in tabsCollection )
		{
			if ( tab.Current.Name.Contains( name, StringComparison.InvariantCultureIgnoreCase ) )
			{
				return tab;
			}
		}

		return null;
	}

	/// <summary> Accede al submenú proporcionado </summary>
	/// <param name="strMenu"> Ruta a submenú solicitado 'Menu&gt;Submenu&gt;Submenu2' </param>
	public void MenuAccess( string strMenu ) => UI.OpenMenu( strMenu );

	/// <summary> Crea nuevo archivo dentro de Zeus del tipo especificado </summary>
	/// <param name="zeusFileType"> Tipo de archivo a crear dentro de Zeus </param>
	public void NewFile( ZeusFileTypes zeusFileType )
	{
		try
		{
			if ( Enum.IsDefined( zeusFileType ) == false )
			{
				throw new ArgumentException( "Invalid ZeusFileType" );
			}

			// Accedes a menú nuevo
			MenuAccess( "File>New..." );

			// Referencia a ventana nuevo
			AutomationElement newWindow = UI.UntilFindElement( "New", AutomationElement.NameProperty ) ??
				throw new Exception( "Unable to find new window" );

			// Referencia a opciones de ventana nuevo
			AutomationElement listNewFileTypes = newWindow.FindElement( ZeusAutoIds.List_NewFile ) ??
				throw new Exception( "Unable to find list of new file types" );

			// Seleccionar tipo de archivo
			var newFileTypeName = zeusFileType == ZeusFileTypes.Performance ? "Performance Calculator" : "Portfolio Document";
			AutomationElement selection = listNewFileTypes.FindElement( newFileTypeName, AutomationElement.NameProperty ) ??
				throw new Exception( "Unable to find type of new file" );
			selection.Select();

			AutomationElement okButton = newWindow.FindElement( ZeusAutoIds.Button_OK ) ??
				throw new Exception( "Unable to find ok button" );
			okButton.Invoke();
		}
		catch ( Exception ex )
		{
			throw new Exception( $"Error creating NewFile[{zeusFileType}] - {ex.Message}", ex );
		}
	}

	/// <summary> Libera caché de la aplicación </summary>
	/// <param name="msTime"> Tiempo en milisegundosque se desea esperar despues de ejecutar reload </param>
	public void Reload( int msTime = 0 )
	{
		// Cierro pestañas abiertas

		CloseAll();

		// Accedo a menú reload
		MenuAccess( "Tools>Reload" );

		// Delay
		if ( msTime > 0 )
		{
			Thread.Sleep( msTime );
		}
	}

	private static Process GetZeusProcess()
	{
		Process? proc = null;
		try
		{
			Process[] processes = Process.GetProcessesByName( "ZEUS" );
			if ( processes.Length > 0 )
			{
				proc = processes[ 0 ];
			}

			if ( proc == null )
			{
				var procStart = new ProcessStartInfo()
				{
					FileName = @"C:\Program Files (x86)\Cyrnel\Analytics\ZEUS.exe",
					WindowStyle = ProcessWindowStyle.Normal
				};

				proc = Process.Start( procStart );
			}

			if ( proc == null )
			{
				throw new InvalidOperationException( "Valida la ruta de instalación de Zeus." );
			}
		}
		catch ( Exception ex )
		{
			throw new Exception( $"No se pudo iniciar la aplicación ZEUS: {ex.Message}", ex );
		}

		return proc;
	}

	private static AutomationElement GetZeusUi( Process process )
	{
		try
		{
			var cancelToken = new CancellationTokenSource( 30 * 1000 );
			AutomationElement? windowElement = null;
			while ( windowElement == null && cancelToken.IsCancellationRequested == false )
			{
				_ = Task.Delay( 1000 ); // Esperar un segundo antes de intentar encontrar la ventana nuevamente
				process.Refresh(); // Actualizar la información del proceso
				var procName = process.MainWindowTitle;
				var procHndl = process.MainWindowHandle;
				if ( procHndl != IntPtr.Zero )
				{
					windowElement = AutomationElement.RootElement.FindElement( process.MainWindowTitle, AutomationElement.NameProperty );
				}
			}

			return windowElement ?? throw new Exception( "Failed to find the window of the process within the specified timeout." );

			//return AutomationElement.FromHandle( process.MainWindowHandle );
		}
		catch ( Exception e )
		{
			throw new Exception( $"Error al obtener la interfaz gráfica de Zeus : {e.Message}", e );
		}
	}

	private static AutomationElement GetZeusWorkArea( AutomationElement ui )
	{
		try
		{
			return ui.FindElement( ZeusAutoIds.Pane_WorkArea ) ??
				throw new Exception( "No se ha localizado la interfaz gráfica de Zeus" );
		}
		catch ( Exception e )
		{
			throw new Exception( $"Error al obtener la interfaz gráfica de Zeus : {e.Message}", e );
		}
	}
}
