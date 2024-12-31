using RiskConsult.Data.Entities;
using RiskConsult.Data.Repositories;
using System.Data;

namespace RiskConsult.Data.Services;

public interface IDateService
{
	DateTime EndDate { get; }

	DateTime StartDate { get; }

	IEnumerable<DateTime> GetBusinessDays();

	DateTime GetBusinessEndOfMonth( DateTime date );

	DateTime GetBusinessEndOfWeek( DateTime date );

	IEnumerable<DateTime> GetBusinessLastDaysFrom( DateTime date, int numberOfDays );

	DateTime GetBusinessNextDay( DateTime date );

	DateTime GetBusinessNextOrEqualsDay( DateTime date );

	/// <summary> Obtiene un periodo de días laborales comprendido entre dos fechas </summary>
	/// <param name="start"> Fecha inicial </param>
	/// <param name="end"> Fecha final </param>
	/// <returns> Enumerable de fechas que existen dentro del periodo </returns>
	IEnumerable<DateTime> GetBusinessPeriod( DateTime start, DateTime end );

	DateTime GetBusinessPreviousDay( DateTime date );

	DateTime GetBusinessPreviousOrEqualsDay( DateTime date );

	DateTime GetBusinessStartOfMonth( DateTime date );

	DateTime GetBusinessStartOfWeek( DateTime date );

	IEnumerable<DateTime> GetHolidays();

	bool IsBusinessDay( DateTime date );

	bool IsHoliday( DateTime date );
}

internal class DateService( IBusinessDaysRepository businessRepository, IHolidaysRepository holidaysRepository, IUnitOfWork unitOfWork ) : IDateService
{
	private DateTime[]? _business;
	private DateTime[]? _holidays;

	public DateTime[] Business => _business ??= [ .. businessRepository.GetAll<DateEntity>().Select( e => e.Date ).Order() ];
	public DateTime EndDate => GetZeusEndDate();
	public DateTime[] Holidays => _holidays ??= [ .. holidaysRepository.GetAll<DateEntity>().Select( e => e.Date ).Order() ];
	public DateTime StartDate => GetZeusStartDate();

	public void ClearCache()
	{
		_business = null;
		_holidays = null;
	}

	public IEnumerable<DateTime> GetBusinessDays() => Business;

	public DateTime GetBusinessEndOfMonth( DateTime date )
	{
		var dteTmp = new DateTime( date.Year, date.Month, DateTime.DaysInMonth( date.Year, date.Month ) );
		return GetBusinessPreviousOrEqualsDay( dteTmp );
	}

	public DateTime GetBusinessEndOfWeek( DateTime date )
	{
		DateTime dteWeekEnd = date;
		while ( dteWeekEnd.DayOfWeek != DayOfWeek.Sunday )
		{
			dteWeekEnd = dteWeekEnd.AddDays( 1 );
		}

		return GetBusinessPreviousOrEqualsDay( dteWeekEnd );
	}

	public IEnumerable<DateTime> GetBusinessLastDaysFrom( DateTime date, int numberOfDays )
	{
		var index = Array.BinarySearch( Business, date );
		index = index >= 0 ? index : ~index - 1;
		for ( var i = 0; i < numberOfDays && index >= 0; i++, index-- )
		{
			yield return Business[ index ];
		}
	}

	public DateTime GetBusinessNextDay( DateTime date )
	{
		var index = Array.BinarySearch( Business, date );
		index = index >= 0 ? index + 1 : ~index;
		return index < Business.Length ? Business[ index ] : throw new InvalidOperationException( "No next business day found." );
	}

	public DateTime GetBusinessNextOrEqualsDay( DateTime date )
	{
		var index = Array.BinarySearch( Business, date );
		if ( index >= 0 )
		{
			return Business[ index ];
		}

		index = ~index;
		return index < Business.Length ? Business[ index ] : throw new InvalidOperationException( "No next business day found." );
	}

	public IEnumerable<DateTime> GetBusinessPeriod( DateTime start, DateTime end )
	{
		return GetDateRange( Business, start, end );
	}

	public DateTime GetBusinessPreviousDay( DateTime date )
	{
		var index = Array.BinarySearch( Business, date );
		index = index >= 0 ? index - 1 : ~index - 1;
		return index >= 0 ? Business[ index ] : throw new InvalidOperationException( "No previous business day found." );
	}

	public DateTime GetBusinessPreviousOrEqualsDay( DateTime date )
	{
		var index = Array.BinarySearch( Business, date );
		if ( index >= 0 )
		{
			return Business[ index ];
		}

		index = ~index - 1;
		return index >= 0 ? Business[ index ] : throw new InvalidOperationException( "No previous business day found." );
	}

	public DateTime GetBusinessStartOfMonth( DateTime date )
	{
		var dteStart = new DateTime( date.Year, date.Month, 1 );
		return GetBusinessNextOrEqualsDay( dteStart );
	}

	public DateTime GetBusinessStartOfWeek( DateTime date )
	{
		// Obtengo día inicial de la semana
		DateTime dteWeekStart = date;
		while ( dteWeekStart.DayOfWeek != DayOfWeek.Monday )
		{
			dteWeekStart = dteWeekStart.AddDays( -1 );
		}

		// Regreso el primer día hábil a partir del inicio de semana
		return GetBusinessNextOrEqualsDay( dteWeekStart );
	}

	public IEnumerable<DateTime> GetHolidays() => Holidays;

	public DateTime GetZeusEndDate()
	{
		using IDbCommand command = unitOfWork.CreateCommand();
		command.CommandText = $"SELECT dteEndDate FROM tblDates";

		DateRec? entity = command.GetEntity<DateRec>( [ new PropertyMap<DateRec>( nameof( DateRec.Date ), "dteEndDate" ) ] );
		return entity?.Date ?? DateTime.MaxValue;
	}

	public DateTime GetZeusStartDate()
	{
		using IDbCommand command = unitOfWork.CreateCommand();
		command.CommandText = $"SELECT dteStartDate FROM tblDates";

		DateRec? entity = command.GetEntity<DateRec>( [ new PropertyMap<DateRec>( nameof( DateRec.Date ), "dteStartDate" ) ] );
		return entity?.Date ?? DateTime.MinValue;
	}

	public bool IsBusinessDay( DateTime date )
	{
		if ( date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday )
		{
			return false;
		}

		return Array.BinarySearch( Business, date ) >= 0;
	}

	public bool IsHoliday( DateTime date ) => Array.BinarySearch( Holidays, date ) >= 0;

	private static IEnumerable<DateTime> GetDateRange( DateTime[] dates, DateTime start, DateTime end )
	{
		var startIndex = Array.BinarySearch( dates, start );
		var endIndex = Array.BinarySearch( dates, end );

		if ( startIndex < 0 )
		{
			startIndex = ~startIndex;
		}

		if ( endIndex < 0 )
		{
			endIndex = ~endIndex - 1;
		}

		for ( var i = startIndex; i <= endIndex && i < dates.Length; i++ )
		{
			if ( dates[ i ] >= start && dates[ i ] <= end )
			{
				yield return dates[ i ];
			}
		}
	}

	private class DateRec
	{ public DateTime Date { get; set; } }
}
