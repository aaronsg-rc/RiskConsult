using RiskConsult.Data.Interfaces;

namespace RiskConsult.Data.Entities;

public interface ICustomHoldingEntity<T> : IDateProperty, IHoldingIdProperty, IParameterProperty, IValueProperty<T>
{ }

/// <summary> tblDTC_Holding_Type ( dteDate, intID, txtParameter, Value ) </summary>
public class CustomHoldingEntity<T> : ICustomHoldingEntity<T>
{
	/// <summary> dteDate </summary>
	public DateTime Date { get; set; }

	/// <summary> intID </summary>
	public int HoldingId { get; set; }

	/// <summary> txtParameter </summary>
	public string Parameter { get; set; } = string.Empty;

	/// <summary> Value </summary>
	public T? Value { get; set; }

	public override string ToString() => string.Join( '|', Date, HoldingId, Parameter, Value );
}
