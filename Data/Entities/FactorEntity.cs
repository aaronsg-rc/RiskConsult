using RiskConsult.Data.Interfaces;
using RiskConsult.Enumerators;

namespace RiskConsult.Data.Entities;

public interface IFactorEntity : IDateProperty, IFactorIdProperty, IValueProperty<double>
{ }

/// <summary> [ tblDATA_Factors, tblDATA_FactorReturns, tblDATA_FactorsCumulative ] ( dteDate, intID, dblValue ) </summary>
public class FactorEntity : IFactorEntity
{
	/// <summary> dteDate </summary>
	public DateTime Date { get; set; }

	/// <summary> intID </summary>
	public FactorId FactorId { get; set; }

	/// <summary> dblValue </summary>
	public double Value { get; set; }

	public override string ToString() => string.Join( '|', Date, FactorId, Value );
}
