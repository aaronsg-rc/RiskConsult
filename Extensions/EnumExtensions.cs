using RiskConsult.Enumerators;

namespace RiskConsult.Extensions;

public static class EnumExtensions
{
	/// <summary> Obtiene el string correspondiente al módelo indicado </summary>
	/// <param name="model"> Referencia al modelo </param>
	/// <returns> Devuelve el nombre/descripción del modelo </returns>
	public static string GetName( this ModelId model )
	{
		var modelNames = new Dictionary<ModelId, string>()
		{
			{ ModelId.Model_01, "Model01 - MediumTerm" },
			{ ModelId.Model_02, "Model02 - MediumTerm" },
			{ ModelId.Model_03, "Model03 - LongTerm" },
			{ ModelId.Model_04, "Model04 - LongTerm" },
			{ ModelId.Model_05, "Model05 - ShortTerm" },
			{ ModelId.Model_06, "Model06 - ShortTerm" },
			{ ModelId.Model_07, "Model07 - MultiClass / MediumTerm" },
			{ ModelId.Model_08, "Model08 - MultiClass / MediumTerm" },
			{ ModelId.Model_09, "Model09 - MultiClass / LongTerm" },
			{ ModelId.Model_10, "Model10 - MultiClass / LongTerm" },
			{ ModelId.Model_11, "Model11 - MultiClass / ShortTerm" },
			{ ModelId.Model_12, "Model12 - MultiClass / ShortTerm" },
			{ ModelId.Stress, "Stress Model" },
			{ ModelId.Model_14, "Model14 - Commodities Parametric" },
			{ ModelId.Model_15, "Model15 - Commodities Simulation" },
			{ ModelId.Stress_2, "Stress Model Commodities" },
		};

		return modelNames.GetValueOrDefault( model ) ?? string.Empty;
	}

	public static string GetName( this ZeusIdType idType )
	{
		return idType switch
		{
			ZeusIdType.ZeusId => "Z",
			ZeusIdType.Ticker => "TEntity",
			ZeusIdType.Ticker2 => "Ticker2",
			_ => throw new NotImplementedException()
		};
	}

	/// <summary> Convierte el enumerador period a DateUnit </summary>
	/// <param name="PeriodId"> ID del periodo </param>
	public static DateUnit ToDateUnit( this PeriodId PeriodId )
	{
		return
			PeriodId == PeriodId.Daily ? DateUnit.Day :
			PeriodId == PeriodId.Weekly ? DateUnit.Week :
			PeriodId == PeriodId.Monthly ? DateUnit.Month :
			DateUnit.Invalid;
	}

	/// <summary> Permite la conversion de un valor a un Enum </summary>
	/// <typeparam name="TEnum"> Enum al que se quiere convertir el valor </typeparam>
	/// <returns> Conversión del valor a un enumerador, si falla intentara regresar su equivalente a -1, si falla arrojara una excepción </returns>
	public static TEnum ToEnum<TEnum>( this double value ) where TEnum : struct, Enum
		=> value.ToString().ToEnum<TEnum>();

	/// <summary> Permite la conversion de un valor a un Enum </summary>
	/// <typeparam name="TEnum"> Enum al que se quiere convertir el valor </typeparam>
	/// <returns> Conversión del valor a un enumerador, si falla intentara regresar su equivalente a -1, si falla arrojara una excepción </returns>
	public static TEnum ToEnum<TEnum>( this int value ) where TEnum : struct, Enum
		=> value.ToString().ToEnum<TEnum>();

	/// <summary> Permite la conversion de un valor a un Enum </summary>
	/// <typeparam name="TEnum"> Enum al que se quiere convertir el valor </typeparam>
	/// <returns> Conversión del valor a un enumerador, si falla intentara regresar su equivalente a -1, si falla arrojara una excepción </returns>
	public static TEnum ToEnum<TEnum>( this string value ) where TEnum : struct, Enum
	{
		return Enum.TryParse( value, true, out TEnum result )
			? result
			: Enum.TryParse( "-1", true, out TEnum invalid )
			? invalid
			: throw new Exception( $"El valor {value} no está definido en {typeof( TEnum ).Name}" );
	}
}
