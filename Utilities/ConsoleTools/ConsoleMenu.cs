using System.Text.RegularExpressions;

namespace RiskConsult.Utilities.ConsoleTools;

public static partial class ConsoleMenu
{
	/// <summary> Obtiene desde consola un valor y lo intenta convertir a su equivalente dentro de <typeparamref name="TEnum" /> </summary>
	/// <typeparam name="TEnum"> Tipo del enumerador al que se quiere convertir </typeparam>
	public static TEnum GetEnum<TEnum>() where TEnum : struct, Enum
	{
		do
		{
			Console.Write( "Write an option\t→\t" );
			var response = Console.ReadLine();
			if ( Enum.TryParse<TEnum>( response, true, out TEnum result ) )
			{
				return result;
			}

			WriteRedLine( "Invalid enum choice, try again." );
		}
		while ( true );
	}

	/// <summary> Obtiene desde consola un valor en base al tipo genérico especificado </summary>
	/// <typeparam name="T"> Tipo del valor que se quiere obtener </typeparam>
	/// <param name="paramName"> Nombre que será desplegado en la consola para solicitar el valor </param>
	/// <returns> Valor del tipo solicitado, si hay error lo manifiesta en consola y vuelve a solicitar hasta que coincida </returns>
	public static T GetValue<T>( string paramName = "value" ) where T : struct => GetValue( value => ( T ) Convert.ChangeType( value, typeof( T ) ), paramName );

	public static T GetValue<T>( Func<string, T> converter, string paramName = "value" ) where T : struct
	{
		do
		{
			Console.Write( $"Enter {paramName} of type {typeof( T )} -> " );
			var response = Console.ReadLine()?.Trim() ?? string.Empty;
			try
			{
				return converter( response );
			}
			catch ( Exception )
			{
				WriteRedLine( $"Invalid {paramName}, please enter a correct value" );
			}
		}
		while ( true );
	}

	/// <summary>
	/// Muestra un menú en consola y ejecuta la accion correspondiente a la seleccion del usuario. Las acciones pueden usarse como expresiones lambda
	/// () =&gt; MyMethod(string example);
	/// </summary>
	/// <typeparam name="TEnum"> Enumerador que se va a asociar a las acciones </typeparam>
	/// <param name="message"> Mensaje que se desea imprimir previo a la impresión ´de las opciones </param>
	/// <param name="options"> Diccionario de claves y valores, opcion de menu y su accion correspondiente </param>
	public static TEnum ShowActionMenu<TEnum>( string message, IDictionary<TEnum, Action> options ) where TEnum : struct, Enum
	{
		ShowMenu<TEnum>( message );
		while ( true )
		{
			TEnum enumChoice = GetEnum<TEnum>();
			if ( options.TryGetValue( enumChoice, out Action? action ) )
			{
				action();
				return enumChoice;
			}
			else
			{
				Console.WriteLine( "Invalid option, try again." );
			}
		}
	}

	public static void ShowMenu<TEnum>( string message ) where TEnum : struct, Enum
	{
		var enumDic = Enum
			.GetValues<TEnum>()
			.ToDictionary(
				e => Convert.ToInt32( e ),
				e => CamelCaseRegex().Replace( e.ToString(), " $1" )
			);

		ShowMenu( message, enumDic );
	}

	public static void ShowMenu<TOpt, TDesc>( string message, IDictionary<TOpt, TDesc> options )
	{
		Console.WriteLine( message );
		foreach ( KeyValuePair<TOpt, TDesc> opt in options )
		{
			Console.WriteLine( $"[{opt.Key}]\t-->\t{opt.Value}" );
		}
	}

	/// <summary> Escribe en consola un mensaje en color rojo </summary>
	public static void WriteRedLine( string message )
	{
		ConsoleColor initialColor = Console.ForegroundColor;
		Console.ForegroundColor = ConsoleColor.Red;
		Console.WriteLine( message );
		Console.ForegroundColor = initialColor;
	}

	[GeneratedRegex( "(\\B[A-Z])" )]
	private static partial Regex CamelCaseRegex();
}
