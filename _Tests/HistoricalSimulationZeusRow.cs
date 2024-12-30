namespace RiskConsult._Tests;

public class HistoricalSimulationZeusRow( DateTime date, double startValue, double endValue, int index )
{
	public readonly DateTime Date = date;
	public readonly double EndValue = endValue;
	public readonly int Index = index;
	public readonly double Return = ( endValue / startValue ) - 1;
	public readonly double StartValue = startValue;

	public override string ToString() => $"{Date.ToShortDateString()}|{Return:P4}";
}
