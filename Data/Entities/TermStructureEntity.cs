using RiskConsult.Data.Interfaces;
using RiskConsult.Enumerators;

namespace RiskConsult.Data.Entities;

public interface ITermStructureEntity : IDateProperty, ITermProperty, ITermStructureIdProperty, IValueProperty<double>
{ }

/// <summary> tblDATA_TermStructure ( dteDate, intTermStructureId, intTerm, dblValue ) </summary>
public class TermStructureEntity : ITermStructureEntity
{
	/// <summary> dteDate </summary>
	public DateTime Date { get; set; }

	/// <summary> intTerm </summary>
	public int Term { get; set; }

	/// <summary> intTermStructureId </summary>
	public TermStructureId TermStructureId { get; set; } = TermStructureId.Invalid;

	/// <summary> dblValue </summary>
	public double Value { get; set; }

	public override string ToString() => string.Join( '|', Date, TermStructureId, Term, Value );
}
