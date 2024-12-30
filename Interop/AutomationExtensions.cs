using RiskConsult.Interop;
using System.Windows.Automation;
using System.Windows.Forms;

namespace RiskConsult.Interop;

public static partial class AutomationExtensions
{
	/// <summary> Expande elemento para que se guarde en caché sus valores </summary>
	/// <param name="element"> Objeto que se va a expandir </param>
	public static void Expand( this AutomationElement element )
	{
		ArgumentNullException.ThrowIfNull( element, nameof( element ) );

		if ( element.TryGetCurrentPattern( ExpandCollapsePattern.Pattern, out var objPattern ) == false )
		{
			throw new InvalidOperationException( $"The element '{element.Current.Name}' does not support expand and collapse." );
		}

		try
		{
			var expandPattern = ( ExpandCollapsePattern ) objPattern;
			expandPattern.Expand();
		}
		catch ( Exception ex )
		{
			throw new Exception( $"Failed to expand '{element.Current.Name}': {ex.Message}", ex );
		}
	}

	/// <summary> Encuentra un elemento en dentro de otro </summary>
	/// <param name="element"> Elemento que contiene al elemento buscado </param>
	/// <param name="propertyValue"> Valor de la propiedad buscada </param>
	/// <param name="propertyType"> Tipo de propiedad accesible desde AutomationElementIdentifiers.NombrePropiedad ó AutomationElement.AutomationIdProperty </param>
	/// <param name="scope"> Ambito dentro del que se buscará </param>
	/// <returns> Primer elemento encontrado que cumpla con la propiedad buscada </returns>
	public static AutomationElement? FindElement( this AutomationElement element, object propertyValue,
		AutomationProperty? propertyType = null, TreeScope scope = TreeScope.Children )
	{
		ArgumentNullException.ThrowIfNull( element, nameof( element ) );
		ArgumentNullException.ThrowIfNull( propertyValue, nameof( propertyValue ) );
		propertyType ??= AutomationElement.AutomationIdProperty;

		// Creo una condición que será usada para encontrar el elemento que coincida con su valor y tipo
		var propCondition = new PropertyCondition( propertyType, propertyValue );
		return element.FindFirst( scope, propCondition );
	}

	/// <summary> Intenta obtener desde la ventana indicada el menu solicitado </summary>
	/// <param name="window"> Refertencia a la ventana que contiene el menu </param>
	/// <param name="menuName"> Nombre del menú, debe ser accesible con alt + inicial </param>
	/// <returns> Referencia a menu contextual o nothing si no lo encontró </returns>
	public static AutomationElement? GetContextMenuElement( this AutomationElement window, string menuName )
	{
		ArgumentNullException.ThrowIfNull( window, nameof( window ) );
		ArgumentNullException.ThrowIfNull( menuName, nameof( menuName ) );

		try
		{
			// Enfocar la ventana que contiene el menú
			window.SetWindowFocus();

			// Enviar teclas de acceso al menú contextual
			SendKeys.SendWait( $"%{menuName.ToUpper()[ 0 ]}" );
			SendKeys.Flush();

			// Esperar hasta 1 segundo para encontrar el menú contextual
			return AutomationElement.RootElement.UntilFindElement(
				ControlType.Menu,
				AutomationElement.ControlTypeProperty,
				TreeScope.Children,
				1000 );
		}
		catch ( Exception ex )
		{
			throw new InvalidOperationException( $"Failed to get context menu element: {ex.Message}", ex );
		}
	}

	/// <summary> Desencadena la acción que se encuentra en el elemento proporcionado </summary>
	/// <param name="element"> Referencia al objeto que contiene método invoke </param>
	/// <param name="newThread"> Verdadero si se desea que se cree un nuevo hilo para esta tarea </param>
	/// <param name="sleep"> Tiempo que espera despues de invocar el elemento </param>
	public static void Invoke( this AutomationElement element, bool newThread = false, int sleep = 50 )
	{
		ArgumentNullException.ThrowIfNull( element, nameof( element ) );

		if ( element.TryGetCurrentPattern( InvokePattern.Pattern, out var objPattern ) == false )
		{
			throw new InvalidOperationException( $"The element '{element.Current.Name}' does not support invoke." );
		}

		try
		{
			var invokePattern = ( InvokePattern ) objPattern;
			if ( newThread )
			{
				var appThread = new Thread( new ThreadStart( invokePattern.Invoke ) );
				appThread.Start();
			}
			else
			{
				invokePattern.Invoke();
			}

			Thread.Sleep( sleep );
		}
		catch ( Exception ex )
		{
			throw new Exception( $"Failed to invoke '{element.Current.Name}': {ex.Message}", ex );
		}
	}

	/// <summary> Accede al menú contextual indicado dentro del elemento de automatización </summary>
	/// <param name="window"> Elemento que contiene el menú solicitado </param>
	/// <param name="menuPath"> Cadena de nombre de menús con la forma "Menu&gt;Submenu1&gt;Submenu# </param>
	/// <param name="timeOut"> Tiempo en milisegundos máximo para intentar acceder al menú solicitado </param>
	public static void OpenMenu( this AutomationElement window, string menuPath, int timeOut = 5000 )
	{
		ArgumentNullException.ThrowIfNull( window, nameof( window ) );
		ArgumentException.ThrowIfNullOrWhiteSpace( menuPath, nameof( menuPath ) );

		// Manejador de timeout
		var cancellationToken = new CancellationTokenSource();
		cancellationToken.CancelAfter( timeOut );

		// Separar el menú en submenús
		var submenus = menuPath.Split( '>' );
		try
		{
			while ( true )
			{
				// Control de bucle y tiempo de espera
				cancellationToken.Token.ThrowIfCancellationRequested();

				// Obtener el menú principal
				AutomationElement? menuElement = window.GetContextMenuElement( submenus[ 0 ] );
				if ( menuElement == null )
				{
					continue;
				}

				// Acceder a cada submenú
				for ( var i = 1; i < submenus.Length; i++ )
				{
					// Referencia al submenú
					var submenuName = submenus[ i ];
					AutomationElement? submenuElement = menuElement.UntilFindElement(
						submenuName,
						AutomationElement.NameProperty,
						TreeScope.Descendants,
						500 );

					// Si no se encuentra el submenú, reiniciar el bucle
					if ( submenuElement == null )
					{
						break;
					}
					else if ( i == submenus.Length - 1 )
					{
						submenuElement.Invoke( true, 500 );
						return;
					}
					else
					{
						submenuElement.Expand();
					}
				}
			}
		}
		catch ( Exception ex )
		{
			throw new Exception( $"Failed to open menu [{menuPath}]: {ex.Message}", ex );
		}
	}

	/// <summary> Selecciona el objeto </summary>
	/// <param name="element"> Referencia al objeto a aseleccionar </param>
	public static void Select( this AutomationElement element )
	{
		ArgumentNullException.ThrowIfNull( element, nameof( element ) );
		if ( element.TryGetCurrentPattern( SelectionItemPattern.Pattern, out var objPattern ) == false )
		{
			throw new InvalidOperationException( $"The element '{element.Current.Name}' does not support selection." );
		}

		try
		{
			var selectionPattern = ( SelectionItemPattern ) objPattern;
			selectionPattern.Select();
		}
		catch ( Exception ex )
		{
			throw new Exception( $"Failed to select '{element.Current.Name}': {ex.Message}", ex );
		}
	}

	/// <summary> Cambia el valor de un ListBox </summary>
	/// <param name="listBox"> Referencia al ListBox </param>
	/// <param name="value"> Valor a cambiar, debe estar contenido dentro la lista </param>
	public static void SelectListBoxValue( this AutomationElement listBox, string value )
	{
		ArgumentNullException.ThrowIfNull( listBox, nameof( listBox ) );
		value ??= string.Empty;

		try
		{
			// Expande el elemento para actualizar los valores que contiene el list box
			listBox.Expand();
			AutomationElement objective = listBox.FindElement( value, AutomationElement.NameProperty, TreeScope.Descendants ) ??
				throw new InvalidOperationException( $"The element '{value}' was not found in the listbox." );
			objective.Select();
		}
		catch ( Exception ex )
		{
			throw new InvalidOperationException( $"Failed to select '{value}' in the listbox: {ex.Message}", ex );
		}
	}

	/// <summary> Cambia la fecha de un ComboBox especificado. </summary>
	/// <param name="datePicker"> Referencia al ComboBox. </param>
	/// <param name="date"> Fecha que se asignará al ComboBox. </param>
	/// <param name="reset"> Fecha permitida para reiniciar el ComboBox. </param>
	/// <param name="sleepTime"> Tiempo en milisegundos a esperar antes de enviar el parámetro de fecha. </param>
	public static void SetDatePickerValue( this AutomationElement datePicker,
		DateTime date, DateTime reset = default, int sleepTime = 100 )
	{
		ArgumentNullException.ThrowIfNull( datePicker, nameof( datePicker ) );

		// Hacer foco en el DatePicker y esperar
		datePicker.SetFocus();
		Thread.Sleep( sleepTime );

		// Obtener el formato de fecha del sistema
		var currentDateFormat = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;

		// Convertir la fecha al formato del sistema
		var resetDate = ( reset == default ? new DateTime( 2012, 1, 1 ) : reset ).ToString( currentDateFormat );
		var formattedDate = date.ToString( currentDateFormat );

		// Enviar la fecha al DatePicker utilizando SendKeys.SendWait
		SendKeys.SendWait( $"{resetDate}{{LEFT}}{{LEFT}}{formattedDate}" );
	}

	/// <summary> Cambia el valor del ValuePattern de un elemento </summary>
	/// <param name="element"> Referencia al objeto </param>
	/// <param name="value"> Valor al que se quiere cambiar el elemento </param>
	public static void SetValue( this AutomationElement element, string value )
	{
		ArgumentNullException.ThrowIfNull( element, nameof( element ) );
		value ??= string.Empty;

		if ( element.TryGetCurrentPattern( ValuePattern.Pattern, out var objPattern ) == false )
		{
			throw new InvalidOperationException( $"The element '{element.Current.Name}' does not support value input." );
		}

		try
		{
			var valuePattern = ( ValuePattern ) objPattern;
			element.SetFocus();
			valuePattern.SetValue( value );
		}
		catch ( Exception ex )
		{
			throw new InvalidOperationException( $"Failed to set value for '{element.Current.Name}': {ex.Message}", ex );
		}
	}

	/// <summary> Trae a primer plano la ventana contenida en el elemento </summary>
	public static void SetWindowFocus( this AutomationElement window )
	{
		ArgumentNullException.ThrowIfNull( window, nameof( window ) );

		try
		{
			// Obtener el control raíz de la ventana
			var rootControl = AutomationElement.FromHandle( window.Current.NativeWindowHandle );
			if ( rootControl.GetCurrentPattern( WindowPattern.Pattern ) is WindowPattern windowPattern )
			{
				// Asegurarse de que la ventana esté visible
				if ( windowPattern.Current.WindowVisualState == WindowVisualState.Minimized )
				{
					windowPattern.SetWindowVisualState( WindowVisualState.Normal );
				}

				// Poner la ventana en primer plano
				windowPattern.SetWindowVisualState( WindowVisualState.Maximized );
			}

			// Establecer el foco en la ventana
			window.SetFocus();
		}
		catch ( Exception ex )
		{
			throw new InvalidOperationException( $"Failed to set focus for the window: {ex.Message}", ex );
		}
	}

	/// <summary>
	/// Encuentra un elemento en dentro de otro. Usar SOLO cuando se asegure que el elemento se va a generar o tenga un retraso en tiempo de ejecución.
	/// </summary>
	/// <param name="element"> Elemento en el que se buscará </param>
	/// <param name="propertyValue"> Valor de la propiedad buscada </param>
	/// <param name="propertyType"> Tipo de propiedad accesible desde AutomationElementIdentifiers.NombrePropiedad ó AutomationElement.AutomationIdProperty </param>
	/// <param name="scope"> Ambito dentro del que se buscará </param>
	/// <param name="maxTimeOut"> Tiempo en milisegundos máximo a esperar para encontrar el elemento </param>
	/// <returns> Primer elemento encontrado que cumpla con la propiedad buscada o nothing si no encuentra nada </returns>
	public static AutomationElement? UntilFindElement( this AutomationElement element, object propertyValue,
		AutomationProperty? propertyType = null, TreeScope scope = TreeScope.Children, int maxTimeOut = -1 )
	{
		ArgumentNullException.ThrowIfNull( element, nameof( element ) );
		ArgumentNullException.ThrowIfNull( propertyValue, nameof( propertyValue ) );

		// Bucle que no se detiene hasta que encuentre el objeto referido
		CancellationTokenSource? cancellationToken = maxTimeOut > 0 ? new CancellationTokenSource( maxTimeOut ) : null;
		AutomationElement? foundElement = null;
		while ( foundElement == null )
		{
			foundElement = element.FindElement( propertyValue, propertyType, scope );

			if ( cancellationToken?.IsCancellationRequested ?? false )
			{
				break;
			}
		}

		return foundElement;
	}
}
