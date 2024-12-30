using RiskConsult.Core;
using RiskConsult.Data;
using RiskConsult.Data.Interfaces;
using RiskConsult.Enumerators;

namespace RiskConsult.Extensions;

public static class FactorExtensions
{
	public static IFactor GetFactor( this int factorId )
	{
		return DbZeus.Db.Factors.GetFactor( factorId );
	}

	public static IFactor GetFactor( this FactorId factorId )
	{
		return ( ( int ) factorId ).GetFactor();
	}

	public static IFactor GetFactor( this string nameOrDescription )
	{
		return DbZeus.Db.Factors.First( f =>
			f.Name.Equals( nameOrDescription, StringComparison.InvariantCultureIgnoreCase ) ||
			f.Description.Equals( nameOrDescription, StringComparison.InvariantCultureIgnoreCase ) );
	}

	public static IFactor GetFactor( this IFactorIdProperty FactorId )
	{
		return DbZeus.Db.Factors.GetFactor( ( int ) FactorId.FactorId );
	}

	public static double GetFactorCumulative( this IFactorIdProperty FactorId, DateTime date )
	{
		return DbZeus.Db.Factors.GetFactorCumulative( ( int ) FactorId.FactorId, date );
	}

	public static double GetFactorReturn( this IFactorIdProperty FactorId, DateTime date )
	{
		return DbZeus.Db.Factors.GetFactorReturn( ( int ) FactorId.FactorId, date );
	}

	public static double[,] GetFactorsCumulativesMatrix( this IEnumerable<IFactorIdProperty> factors, IEnumerable<DateTime> dates )
	{
		return Utilities.Utilities.GetMatrix( dates, factors, ( date, factor ) => factor.GetFactorCumulative( date ) );
	}

	public static double[,] GetFactorsReturnsMatrix( this IEnumerable<IFactorIdProperty> factors, IEnumerable<DateTime> dates )
	{
		return Utilities.Utilities.GetMatrix( dates, factors, ( date, factor ) => factor.GetFactorReturn( date ) );
	}

	public static double[,] GetFactorsValuesMatrix( this IEnumerable<IFactorIdProperty> factors, IEnumerable<DateTime> dates )
	{
		return Utilities.Utilities.GetMatrix( dates, factors, ( date, factor ) => factor.GetFactorValue( date ) );
	}

	public static double GetFactorValue( this IFactorIdProperty FactorId, DateTime date )
	{
		return DbZeus.Db.Factors.GetFactorValue( ( int ) FactorId.FactorId, date );
	}
}
