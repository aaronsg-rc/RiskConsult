namespace RiskConsult.Utilities.PortfolioComparer;

public interface IHolding
{
	string Name { get; set; }
	double Amount { get; set; }
	double Price { get; set; }
	double Value { get; }
	DateTime Date { get; set; }
}

public class Holding : IHolding
{
	public string Name { get; set; } = string.Empty;
	public double Amount { get; set; }
	public double Price { get; set; }
	public double Value => Price * Amount;
	public DateTime Date { get; set; }
}
