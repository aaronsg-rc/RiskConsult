using RiskConsult.Data.Interfaces;

namespace RiskConsult.Data.Entities;

public interface IMapStringEntity : IDescriptionProperty, IGroupIdProperty<string>, IIdProperty, INameProperty
{ }

/// <summary> tblMAP_Strings ( intID, txtGroupID, txtName, txtDescription ) </summary>
public class MapStringEntity : IMapStringEntity
{
	/// <summary> txtDescription </summary>
	public string Description { get; set; } = string.Empty;

	/// <summary> txtGroupID </summary>
	public string GroupId { get; set; } = string.Empty;

	/// <summary> intID </summary>
	public int Id { get; set; } = -1;

	/// <summary> txtName </summary>
	public string Name { get; set; } = string.Empty;

	public override string ToString() => string.Join( '|', Id, GroupId, Name, Description );
}
