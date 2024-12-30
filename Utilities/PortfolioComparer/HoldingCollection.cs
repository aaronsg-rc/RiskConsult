namespace RiskConsult.Utilities.PortfolioComparer;

public interface IHoldingCollection : IList<IHolding>
{
	abstract new void Add( IHolding holding );

	IHolding? GetHolding( string name );

	void Remove( string name );
}

public class HoldingCollection : List<IHolding>, IHoldingCollection
{
	public IHolding? GetHolding( string name ) => this.FirstOrDefault( x => x.Name == name );

	public void Remove( string name )
	{
		IHolding? holding = GetHolding( name );
		if ( holding != null )
		{
			Remove( holding );
		}
	}
}
