namespace RiskConsult.Reporting;

public interface IReportModel : ILineProvider, IHeadersProvider
{
	string ToString();
}
