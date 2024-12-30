namespace RiskConsult.Performance.Providers;

public interface IReturnProvider
{
	/// <summary> Conjunto de datos atribuibles al rendimiento en la fecha proporcionada </summary>
	IReturnData GetReturn( DateTime date );
}

public abstract class ReturnProvider : IReturnProvider, IDateInitialValueProvider, IDateFinalValueProvider, IDateReturnPercentProvider, IDateReturnValueProvider, IDateValueProvider
{
	private readonly Dictionary<DateTime, IReturnData> _returns = [];

	public double GetFinalValue( DateTime date ) => GetReturn( date ).FinalValue;

	public double GetInitialValue( DateTime date ) => GetReturn( date ).InitialValue;

	public IReturnData GetReturn( DateTime date )
	{
		if ( _returns.TryGetValue( date, out IReturnData? returnData ) )
		{
			return returnData;
		}
		else
		{
			return _returns[ date ] = CalculateReturn( date );
		}
	}

	public double GetReturnPercent( DateTime date ) => GetReturn( date ).ReturnPercent;

	public double GetReturnValue( DateTime date ) => GetReturn( date ).ReturnValue;

	public double GetValue( DateTime date ) => GetReturn( date ).InitialValue;

	protected abstract IReturnData CalculateReturn( DateTime date );
}
