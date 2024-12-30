namespace RiskConsult.Utilities.PortfolioComparer;

public interface IPortfolio
{
	DateTime Date { get; }
	string Name { get; }
	double Value { get; }
	IHoldingCollection Holdings { get; }
}

public class Portfolio : IPortfolio
{
	public DateTime Date { get; set; }
	public string Name { get; set; } = string.Empty;
	public double Value => Holdings.Sum( hold => hold.Value );
	public IHoldingCollection Holdings { get; set; } = new HoldingCollection();
}
