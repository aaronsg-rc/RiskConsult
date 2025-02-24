using RiskConsult.Data.Entities;
using RiskConsult.Data.Interfaces;
using RiskConsult.Data.Repositories;
using System.Data;

namespace RiskConsult.Data.Services;

public interface IDateService : ICachedService
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

internal class DateService( IBusinessDaysRepository businessRepository, IHolidaysRepository holidaysRepository ) : IDateService
{
	private HashSet<DateTime>? _business;
	private HashSet<DateTime>? _holidays;

	public HashSet<DateTime> Business => _business ??= [ .. businessRepository.GetAll<DateEntity>().Select( e => e.Date ).Order() ];
	public DateTime EndDate => GetZeusEndDate();
	public HashSet<DateTime> Holidays => _holidays ??= [ .. holidaysRepository.GetAll<DateEntity>().Select( e => e.Date ).Order() ];
	public DateTime StartDate => GetZeusStartDate();

	public void ClearCache()
	{
		_business = null;
		_holidays = null;
	}

	public IEnumerable<DateTime> GetBusinessDays() => Business;

	public DateTime GetBusinessEndOfMonth( DateTime date )
	{
		var dteEndOfMonth = new DateTime( date.Year, date.Month, DateTime.DaysInMonth( date.Year, date.Month ) );
		return GetBusinessPreviousOrEqualsDay( dteEndOfMonth );
	}

	public DateTime GetBusinessEndOfWeek( DateTime date )
	{
		DateTime dteEndOfWeek = date.Date.AddDays( ( int ) DayOfWeek.Sunday - ( int ) date.DayOfWeek );
		return GetBusinessPreviousOrEqualsDay( dteEndOfWeek );
	}

	public IEnumerable<DateTime> GetBusinessLastDaysFrom( DateTime date, int numberOfDays )
	{
		for ( int count = 0; count < numberOfDays; count++ )
		{
			date = GetBusinessPreviousDay( date );
			yield return date;
		}
	}

	public DateTime GetBusinessNextDay( DateTime date )
	{
		do
		{
			date = date.AddDays( 1 );
		} while ( !IsBusinessDay( date ) );

		return date;
	}

	public DateTime GetBusinessNextOrEqualsDay( DateTime date )
	{
		if ( IsBusinessDay( date ) )
		{
			return date;
		}

		return GetBusinessNextDay( date );
	}

	public IEnumerable<DateTime> GetBusinessPeriod( DateTime start, DateTime end )
	{
		for ( DateTime current = start; current <= end; current = current.AddDays( 1 ) )
		{
			if ( IsBusinessDay( current ) )
			{
				yield return current;
			}
		}
	}

	public DateTime GetBusinessPreviousDay( DateTime date )
	{
		do
		{
			date = date.AddDays( -1 );
		} while ( !IsBusinessDay( date ) );

		return date;
	}

	public DateTime GetBusinessPreviousOrEqualsDay( DateTime date )
	{
		if ( IsBusinessDay( date ) )
		{
			return date;
		}

		return GetBusinessPreviousDay( date );
	}

	public DateTime GetBusinessStartOfMonth( DateTime date )
	{
		var dteStartOfMonth = new DateTime( date.Year, date.Month, 1 );
		return GetBusinessNextOrEqualsDay( dteStartOfMonth );
	}

	public DateTime GetBusinessStartOfWeek( DateTime date )
	{
		DateTime dteStartOfWeek = date.Date.AddDays( -( int ) date.DayOfWeek + ( int ) DayOfWeek.Monday );
		return GetBusinessNextOrEqualsDay( dteStartOfWeek );
	}

	public IEnumerable<DateTime> GetHolidays() => Holidays;

	public DateTime GetZeusEndDate() => businessRepository.GetZeusEndDate();

	public DateTime GetZeusStartDate() => businessRepository.GetZeusStartDate();

	public bool IsBusinessDay( DateTime date )
	{
		if ( date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday )
		{
			return false;
		}

		return !Holidays.Contains( date );
	}

	public bool IsHoliday( DateTime date ) => Holidays.Contains( date );

	private static IEnumerable<DateTime> GetDateRange( DateTime[] dates, DateTime start, DateTime end )
	{
		var startIndex = Array.BinarySearch( dates, start.Date );
		var endIndex = Array.BinarySearch( dates, end.Date );

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
			if ( dates[ i ] >= start.Date && dates[ i ] <= end.Date )
			{
				yield return dates[ i ];
			}
		}
	}
}
