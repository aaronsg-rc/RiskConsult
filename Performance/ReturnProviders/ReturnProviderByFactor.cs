using RiskConsult.Core;
using RiskConsult.Enumerators;
using RiskConsult.Extensions;

namespace RiskConsult.Performance.ReturnProviders;

public class ReturnProviderByFactor( IFactor factor ) : ReturnProvider
{
	protected override IReturnData CalculateReturn( DateTime date )
	{
		DateTime prevDate = date.GetBusinessDateAdd( DateUnit.Day, -1 );
		var initialValue = factor.GetFactorValue( prevDate );
		var finalValue = factor.GetFactorValue( date );
		return new ReturnData( date,
			initialValue,
			finalValue,
			factor.GetFactorReturn( date ),
			finalValue - initialValue );
	}
}
