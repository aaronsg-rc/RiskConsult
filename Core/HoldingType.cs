using RiskConsult.Data.Interfaces;
using RiskConsult.Enumerators;

namespace RiskConsult.Core;

public interface IHoldingType : IClassIdProperty, ICountryIdProperty, ICurrencyIdProperty, IFactorIdProperty, IModuleIdProperty, ITermProperty, ITypeIdProperty
{
	FactorSetId AFactorSet { get; set; }
	int CompoundingPeriod { get; set; }
	int CompoundingType { get; set; }
	int ContractValue { get; set; }
	int Conv { get; set; }
	int DCType { get; set; }
	HTypeId HType { get; set; }
	FactorId IndexID { get; set; }
	int LotSize { get; set; }
	int Nominal { get; set; }
	PeriodId PayPeriodId { get; set; }
	FactorSetId SFactorSet { get; set; }
	TermStructureId STermStructure { get; set; }
	SubTypeId SubType { get; set; }
	TermStructureId TermStructure { get; set; }
	TermStructureId TSForeignID { get; set; }
	int Underlying { get; set; }
	FactorId VolatilityID { get; set; }
}

public class HoldingType : IHoldingType
{
	public FactorSetId AFactorSet { get; set; } = FactorSetId.Invalid;
	public ClassId ClassId { get; set; } = ClassId.Invalid;
	public int CompoundingPeriod { get; set; } = -1;
	public int CompoundingType { get; set; } = -1;
	public int ContractValue { get; set; } = -1;
	public int Conv { get; set; } = -1;
	public CountryId CountryId { get; set; } = CountryId.Invalid;
	public CurrencyId CurrencyId { get; set; } = CurrencyId.Invalid;
	public int DCType { get; set; } = -1;
	public FactorId FactorId { get; set; } = FactorId.Invalid;
	public HTypeId HType { get; set; } = HTypeId.Invalid;
	public FactorId IndexID { get; set; } = FactorId.Invalid;
	public int LotSize { get; set; } = 1;
	public ModuleId ModuleId { get; set; } = ModuleId.Invalid;
	public int Nominal { get; set; } = -1;
	public PeriodId PayPeriodId { get; set; } = PeriodId.Invalid;
	public FactorSetId SFactorSet { get; set; } = FactorSetId.Invalid;
	public TermStructureId STermStructure { get; set; } = TermStructureId.Invalid;
	public SubTypeId SubType { get; set; } = SubTypeId.Invalid;
	public int Term { get; set; } = -1;
	public TermStructureId TermStructure { get; set; } = TermStructureId.Invalid;
	public TermStructureId TSForeignID { get; set; } = TermStructureId.Invalid;
	public TypeId TypeId { get; set; } = TypeId.Invalid;
	public int Underlying { get; set; } = -1;
	public FactorId VolatilityID { get; set; } = FactorId.Invalid;

	public override string ToString() => $"[{( int ) TypeId}] {TypeId}";
}
