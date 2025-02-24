using RiskConsult.Core;
using RiskConsult.Enumerators;

namespace RiskConsult.Data.Interfaces;

public interface IAmountProperty
{
	/// <summary> Cantidad </summary>
	double Amount { get; set; }
}

public interface IClassIdProperty
{
	/// <summary> Id de la clase del elemento </summary>
	ClassId ClassId { get; set; }
}

public interface ICodeProperty
{
	/// <summary> Código del elemento </summary>
	string Code { get; set; }
}

public interface ICommentProperty
{
	/// <summary> Comentario del elemento </summary>
	string Comment { get; set; }
}

public interface ICountryIdProperty
{
	/// <summary> Id del país </summary>
	CountryId CountryId { get; set; }
}

public interface ICouponRateProperty
{
	/// <summary> Tasa cupón </summary>
	public double CouponRate { get; set; }
}

public interface ICovMatIdProperty
{
	/// <summary> Id de la matriz de covarianzas </summary>
	int CovMatId { get; set; }
}

public interface ICurrencyIdProperty
{
	/// <summary> Moneda del elemento </summary>
	public CurrencyId CurrencyId { get; set; }
}

public interface IDateProperty
{
	/// <summary> Fecha del elemento </summary>
	DateTime Date { get; set; }
}

public interface IDescriptionProperty
{
	/// <summary> Descripción del elemento </summary>
	string Description { get; set; }
}

public interface IEventIdProperty
{
	/// <summary> Id del tipo de evento corporativo </summary>
	EventId EventId { get; set; }
}

public interface IExposureIdProperty
{
	/// <summary> Id del tipo de exposición </summary>
	int ExposureId { get; set; }
}

public interface IFactorIdProperty
{
	/// <summary> Id del factor de riesgo </summary>
	FactorId FactorId { get; set; }
}

public interface IFactorProperty
{
	/// <summary> Factor del elemento </summary>
	IFactor Factor { get; set; }
}

public interface IFactorSetIdProperty
{
	/// <summary> Id del conjunto de factores al que pertenece el elemento </summary>
	FactorSetId FactorSetId { get; set; }
}

public interface IFactorTypeIdProperty
{
	/// <summary> Id del tipo de factor </summary>
	FactorTypeId FactorTypeId { get; set; }
}

public interface IFinalDateProperty
{
	/// <summary> Fecha final del elemento </summary>
	DateTime FinalDate { get; set; }
}

public interface IGroupIdProperty<T>
{
	/// <summary> Id del grupo al que pertenece </summary>
	T GroupId { get; set; }
}

public interface IHoldingIdProperty
{
	/// <summary> Id del instrumento </summary>
	int HoldingId { get; set; }
}

public interface IHoldingTermsProperty
{
	/// <summary> Términos y condiciones del instrumento </summary>
	IHoldingTerms Terms { get; set; }
}

public interface IIdProperty
{
	/// <summary> Id del elemento </summary>
	int Id { get; set; }
}

public interface IInitialDateProperty
{
	/// <summary> Fecha inicial del elemento </summary>
	DateTime InitialDate { get; set; }
}

public interface IIsinProperty
{
	/// <summary> Identificador Isin del instrumento </summary>
	public string Isin { get; set; }
}

public interface IIssueProperty
{
	/// <summary> Fecha de emisión </summary>
	public DateTime Issue { get; set; }
}

public interface ILotSizeProperty
{
	/// <summary> Tamaño de lote </summary>
	public int LotSize { get; set; }
}

public interface IMaturityProperty
{
	/// <summary> Fecha de vencimiento </summary>
	public DateTime Maturity { get; set; }
}

public interface IModelIdProperty
{
	/// <summary> Id del modelo </summary>
	ModelId ModelId { get; set; }
}

public interface IModuleIdProperty
{
	/// <summary> Id del módulo </summary>
	ModuleId ModuleId { get; set; }
}

public interface INameProperty
{
	/// <summary> Nombre del elemento </summary>
	string Name { get; set; }
}

public interface INominalProperty
{
	/// <summary> Nominal del instrumento </summary>
	public double Nominal { get; set; }
}

public interface IParameterProperty
{
	/// <summary> Parámetro al que pertenece </summary>
	string Parameter { get; set; }
}

public interface IPayDayProperty
{
	/// <summary> Fecha de pago </summary>
	public int PayDay { get; set; }
}
public interface ISpreadProperty
{
	public double Spread { get; set; }
}
public interface IPayFrequencyProperty
{
	/// <summary> Frecuencia de pago </summary>
	public int PayFrequency { get; set; }
}

public interface IPeriodIdProperty
{
	/// <summary> Id del periodo </summary>
	PeriodId PeriodId { get; set; }
}

public interface IPortfolioIdProperty
{
	/// <summary> Id del portafolio </summary>
	string PortfolioId { get; set; }
}

public interface IPriceProperty
{
	/// <summary> Precio del elemento </summary>
	double Price { get; set; }
}

public interface IPriceSourceIdProperty
{
	/// <summary> Id del tipo de precio </summary>
	PriceSourceId PriceSourceId { get; set; }
}

public interface IRatingAgencyIdProperty
{
	/// <summary> Id de la agencia calificadora </summary>
	int RatingAgencyId { get; set; }
}

public interface IRatingIdProperty
{
	/// <summary> Id del tipo de calificación </summary>
	int RatingId { get; set; }
}

public interface ISeverityProperty
{
	/// <summary> Severidad </summary>
	int Severity { get; set; }
}

public interface IShockIdProperty
{
	/// <summary> Id del escenario de shock </summary>
	int ShockId { get; set; }
}

public interface IStrikeProperty
{
	/// <summary> Strike o precio objetivo del derivado </summary>
	public double Strike { get; set; }
}

public interface ISubTypeIdProperty
{
	/// <summary> Id del tipo de valor </summary>
	SubTypeId SubTypeId { get; set; }
}

public interface ITermProperty
{
	/// <summary> Días a los que corresponde el valor </summary>
	int Term { get; set; }
}

public interface ITermStructureIdProperty
{
	/// <summary> Id del tipo de curva </summary>
	TermStructureId TermStructureId { get; set; }
}

public interface ITicker2Property
{
	/// <summary> Identificador del tipo EMISORA_SERIE_TV </summary>
	public string Ticker2 { get; set; }
}

public interface ITickerProperty
{
	/// <summary> Identificador único de PiP </summary>
	public string Ticker { get; set; }
}

public interface ITypeIdProperty
{
	/// <summary> Id del tipo de instrumento </summary>
	TypeId TypeId { get; set; }
}

public interface IUnderlyingIdProperty
{
	/// <summary> Id del instrumento subyacente </summary>
	public int UnderlyingId { get; set; }
}

public interface IValueProperty<T>
{
	/// <summary> Valor del elemento </summary>
	T? Value { get; set; }
}

public interface IWeekDayAdjustProperty
{
	/// <summary> Si la fecha de pago caé en día inhabil, determina si toma el día anterior o siguiente </summary>
	public int WeekDayAdjust { get; set; }
}

public interface IScenarioIdProperty
{
	/// <summary> Id del escenario </summary>
	int ScenarioId { get; set; }
}
