using RiskConsult.Data;
using RiskConsult.Enumerators;

namespace RiskConsult.Extensions;

public static class DatesExtensions
{
	/// <summary> Agrega un intervalo de tiempo a una fecha </summary>
	/// <param name="date"> Fecha base </param>
	/// <param name="interval"> Intervalo de tiempo </param>
	/// <param name="value"> Número de veces que se agrega un intervalo de tiempo </param>
	/// <returns> Fecha después de agregar los n intervalos </returns>
	public static DateTime Add( this DateTime date, DateUnit interval, int value )
	{
		return
			interval == DateUnit.Day ? date.AddDays( value ) :
			interval == DateUnit.Week ? date.AddDays( value * 7 ) :
			interval == DateUnit.Month ? date.AddMonths( value ) :
			interval == DateUnit.Year ? date.AddYears( value ) :
			throw new Exception( $"Invalid interval '{interval}'" );
	}

	/// <summary> Agrega un periodo de tiempo a una fecha dada </summary>
	/// <param name="date"> Fecha inicial </param>
	/// <param name="interval"> Intervalo de tiempo que se agregará a la fecha, "d", "m", "y" </param>
	/// <param name="value"> Número de intervalos de tiempo que se agregarán a la fecha </param>
	/// <returns> Día hábil obtenido despues de agregar los intervalos de tiempo </returns>
	public static DateTime GetBusinessDateAdd( this DateTime date, DateUnit interval, int value )
	{
		DateTime dteTemp = date.Add( interval, value );
		return value < 0 ? dteTemp.GetBusinessPreviousOrEqualsDay() : dteTemp.GetNextOrEqualsBusinessDay();
	}

	/// <summary> Obtiene el último día habil del mes de la fecha ingresada </summary>
	/// <param name="date"> Fecha de la que se quiere obtener el último día de mes </param>
	/// <returns> Último día habil del mes </returns>
	public static DateTime GetBusinessEndOfMonth( this DateTime date )
	{
		return DbZeus.Db.Dates.GetBusinessEndOfMonth( date );
	}

	/// <summary> Obtiene el último día habil de la semana de la fecha ingresada </summary>
	/// <param name="date"> Fecha base </param>
	/// <returns> Último día habil de la semana </returns>
	public static DateTime GetBusinessEndOfWeek( this DateTime date )
	{
		return DbZeus.Db.Dates.GetBusinessEndOfWeek( date );
	}

	public static DateTime[] GetBusinessLastDays( this DateTime date, int count )
	{
		return DbZeus.Db.Dates.GetBusinessLastDaysFrom( date, count ).ToArray();
	}

	public static DateTime GetBusinessNextDay( this DateTime date )
	{
		return DbZeus.Db.Dates.GetBusinessNextDay( date );
	}

	public static DateTime GetBusinessPreviousDay( this DateTime date )
	{
		return DbZeus.Db.Dates.GetBusinessPreviousDay( date );
	}

	/// <summary> Obtiene la fecha hábil anterior o igual a la fecha base. </summary>
	/// <param name="date"> Fecha base </param>
	/// <returns> Fecha hábil anterior o igual a la fecha base. </returns>
	public static DateTime GetBusinessPreviousOrEqualsDay( this DateTime date )
	{
		return DbZeus.Db.Dates.GetBusinessPreviousOrEqualsDay( date );
	}

	/// <summary> Obtiene el primer día habil del mes de la fecha ingresada </summary>
	/// <param name="date"> Fecha de la que se quiere obtener el inicio de mes </param>
	/// <returns> Primer día habil del mes </returns>
	public static DateTime GetBusinessStartOfMonth( this DateTime date )
	{
		return DbZeus.Db.Dates.GetBusinessStartOfMonth( date );
	}

	/// <summary> Obtiene el primer día habil de la semana de la fecha ingresada </summary>
	/// <param name="date"> Fecha de la que se quiere obtener el inicio de semana </param>
	/// <returns> Primer día habil de la semana </returns>
	public static DateTime GetBusinessStartOfWeek( this DateTime date )
	{
		return DbZeus.Db.Dates.GetBusinessStartOfWeek( date );
	}

	/// <summary> Obtiene la fecha hábil mayor o igual que la fecha base </summary>
	/// <param name="date"> Fecha base </param>
	/// <returns> Fecha hábil mayor o igual que la fecha base </returns>
	public static DateTime GetNextOrEqualsBusinessDay( this DateTime date )
	{
		return DbZeus.Db.Dates.GetBusinessNextOrEqualsDay( date );
	}

	/// <summary> Determina si una fecha es un día laboral para el sistema </summary>
	/// <param name="date"> Fecha a validar </param>
	public static bool IsBusinessDay( this DateTime date )
	{
		return DbZeus.Db.Dates.IsBusinessDay( date );
	}

	/// <summary> Determina si una fecha es un día festivo para el sistema </summary>
	/// <param name="date"> Fecha a validar </param>
	public static bool IsHoliday( this DateTime date )
	{
		return DbZeus.Db.Dates.IsHoliday( date );
	}
}
