using RiskConsult.Data.Interfaces;
using RiskConsult.Enumerators;

namespace RiskConsult.Core;

public interface IFactor : IFactorIdProperty, INameProperty, IDescriptionProperty, ITermProperty, ITermStructureIdProperty, IFactorTypeIdProperty
{
}

public class Factor : IFactor
{
	public string Description { get; set; } = string.Empty;
	public FactorId FactorId { get; set; } = FactorId.Invalid;
	public FactorTypeId FactorTypeId { get; set; } = FactorTypeId.Invalid;
	public string Name { get; set; } = string.Empty;
	public int Term { get; set; } = -1;
	public TermStructureId TermStructureId { get; set; } = TermStructureId.Invalid;

	public override string ToString() => $"{FactorId}|{Description}";
}
