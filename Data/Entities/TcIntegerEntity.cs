using RiskConsult.Data.Interfaces;

namespace RiskConsult.Data.Entities;

public interface ITcIntegerEntity : IGroupIdProperty<int>, IIdProperty, IParameterProperty, IValueProperty<int>
{ }

/// <summary> tblTC_Integer ( intGroupID, intId, txtParameter, intValue ) </summary>
public class TcIntegerEntity : ITcIntegerEntity
{
	/// <summary> intGroupID </summary>
	public int GroupId { get; set; }

	/// <summary> intId </summary>
	public int Id { get; set; }

	/// <summary> txtParameter </summary>
	public string Parameter { get; set; } = string.Empty;

	/// <summary> intValue </summary>
	public int Value { get; set; }

	public override string ToString() => string.Join( '|', GroupId, Id, Parameter, Value );
}
