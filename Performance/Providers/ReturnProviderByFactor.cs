using RiskConsult.Core;
using RiskConsult.Enumerators;
using RiskConsult.Extensions;

namespace RiskConsult.Performance.Providers;

public class ReturnProviderByFactor( IFactor factor ) : ReturnProvider
{
	protected override IReturnData CalculateReturn( DateTime date )
	{
		DateTime prevDate = date.GetBusinessDateAdd( DateUnit.Day, -1 );
		var prevValue = factor.GetFactorValue( prevDate );
		var value = factor.GetFactorValue( date );
		return new ReturnData( date,
			prevValue,
			value,
			factor.GetFactorReturn( date ),
			value - prevValue );
	}
}
