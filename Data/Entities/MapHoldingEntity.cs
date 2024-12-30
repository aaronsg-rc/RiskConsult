using RiskConsult.Data.Interfaces;

namespace RiskConsult.Data.Entities;

public interface IMapHoldingEntity : IDescriptionProperty, IFinalDateProperty, IInitialDateProperty, IHoldingIdProperty, INameProperty
{ }

/// <summary> tblMAP_Holdings ( dteStart, dteEnd, intholdingId, txtHoldingName, txtHoldingDescription ) </summary>
public class MapHoldingEntity : IMapHoldingEntity
{
	/// <summary> txtHoldingDescription </summary>
	public string Description { get; set; } = string.Empty;

	/// <summary> dteEnd </summary>
	public DateTime FinalDate { get; set; }

	/// <summary> intholdingId </summary>
	public int HoldingId { get; set; }

	/// <summary> dteStart </summary>
	public DateTime InitialDate { get; set; }

	/// <summary> txtHoldingName </summary>
	public string Name { get; set; } = string.Empty;

	public override string ToString() => string.Join( '|', InitialDate, FinalDate, HoldingId, Name, Description );
}
