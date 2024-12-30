using RiskConsult.Data.Interfaces;
using RiskConsult.Enumerators;

namespace RiskConsult.Extensions;

public static class CurrencyExtensions
{
	private static readonly Dictionary<(DateTime, CurrencyId, CurrencyId), double> _cache = [];

	/// <summary> Convierte el valor al tipo de cambio solicitado </summary>
	/// <param name="fromCurrency"> Implementacion de ID de moneda de entrada </param>
	/// <param name="toCurrency"> ID de moneda a convertir </param>
	/// <param name="csZeus"> Cadena de conexión a Zeus </param>
	/// <param name="fromValue"> Valor en la moneda de origen </param>
	/// <param name="date"> Fecha de la que se quiere obtener el tipo de cambio </param>
	public static double ConvertToCurrency( this ICurrencyIdProperty fromCurrency, CurrencyId toCurrency, DateTime date, double fromValue = 1 )
	{
		return fromCurrency.CurrencyId.ConvertToCurrency( toCurrency, date, fromValue );
	}

	/// <summary> Obtiene el tipo de cambio entre dos monedas en la fecha indicada </summary>
	/// <param name="from"> Nombre de moneda de entrada, ticker de 3 letras </param>
	/// <param name="to"> Nombre de moneda a convertir, ticker de 3 letras </param>
	/// <param name="date"> Fecha de la que se quiere obtener el tipo de cambio </param>
	public static double ConvertToCurrency( this CurrencyId from, CurrencyId to, DateTime date, double fromValue = 1 )
	{
		if ( from == to )
		{
			return fromValue;
		}

		if ( _cache.TryGetValue( (date, from, to), out var fx ) )
		{
			return fromValue * fx;
		}

		// Asigno valor de valor de origen
		var dblFxFrom =
			from is CurrencyId.UDI ? FactorId.MXN.GetFactor().GetFactorValue( date ) / FactorId.UDI.GetFactor().GetFactorValue( date ) :
			from is not CurrencyId.USD ? ( ( int ) from ).GetFactor().GetFactorValue( date ) : 1;

		// Cargo valor del factor destino
		var dblFxTo =
			to is CurrencyId.UDI ? FactorId.MXN.GetFactor().GetFactorValue( date ) / FactorId.UDI.GetFactor().GetFactorValue( date ) :
			to is not CurrencyId.USD ? ( ( int ) to ).GetFactor().GetFactorValue( date ) : 1;

		// Asigno valor a cache y devuelvo
		fx = _cache[ (date, from, to) ] = dblFxTo / dblFxFrom;
		return fromValue * fx;
	}

	public static CurrencyId ToCurrencyId( this int currencyId )
	{
		return ( CurrencyId ) currencyId;
	}

	public static CurrencyId ToCurrencyId( this string currencyId )
	{
		if ( currencyId.Equals( "YEN", StringComparison.InvariantCultureIgnoreCase ) || currencyId.Equals( "JPY", StringComparison.InvariantCultureIgnoreCase ) )
		{
			return CurrencyId.JPY;
		}

		return Enum.Parse<CurrencyId>( currencyId );
	}
}
