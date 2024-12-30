using RiskConsult.Data.Interfaces;

namespace RiskConsult.Data.Entities;

public interface IDateEntity : IDateProperty
{ }

public class DateEntity : IDateEntity
{
	public DateTime Date { get; set; }
}
