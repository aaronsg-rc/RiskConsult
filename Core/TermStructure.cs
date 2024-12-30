using RiskConsult.Data.Interfaces;
using RiskConsult.Enumerators;
using RiskConsult.Maths;

namespace RiskConsult.Core;

public interface ITermStructure : ITermStructureIdProperty, IDateProperty, INameProperty, IDescriptionProperty
{
	int[] Terms { get; }
	double[] Values { get; }

	double GetTermValue( int term );
}

public class TermStructure : ITermStructure
{
	public DateTime Date { get; set; } = DateTime.MinValue;
	public string Description { get; set; } = string.Empty;
	public string Name { get; set; } = string.Empty;
	public int[] Terms { get; set; } = [];
	public TermStructureId TermStructureId { get; set; } = TermStructureId.Invalid;
	public double[] Values { get; set; } = [];

	public double GetTermValue( int term ) => Calculator.LinearInterpolation( term, Terms, Values );
}
