using RiskConsult.Performance.ReturnProviders;

namespace RiskConsult.Performance;

public class Performance
{
	public DateTime FinalDate { get; private set; }

	public DateTime InitialDate { get; private set; }

	public double ReturnPercent { get; private set; }

	public List<IReturnData> Returns { get; private set; } = [];

	public double ReturnValue { get; private set; }

	public void Calculate( IEnumerable<DateTime> period, IReturnProvider returnProvider )
	{
		var returnPercent = 1.0;
		var returnValue = 0.0;
		var returns = new List<IReturnData>();
		DateTime initialDate = DateTime.MaxValue;
		DateTime finalDate = DateTime.MinValue;
		foreach ( DateTime date in period )
		{
			IReturnData dateReturn = returnProvider.GetReturn( date );
			returnPercent *= 1 + dateReturn.ReturnPercent;
			returnValue += dateReturn.ReturnValue;

			if ( date < initialDate )
			{
				initialDate = date;
			}

			if ( date > finalDate )
			{
				finalDate = date;
			}

			returns.Add( dateReturn );
		}

		ReturnPercent = returnPercent - 1;
		ReturnValue = returnValue;
		InitialDate = initialDate;
		FinalDate = finalDate;
		Returns = returns;
	}
}
