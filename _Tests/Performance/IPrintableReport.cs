namespace RiskConsult._Tests.Performance;

public interface IPrintableReport
{
	static abstract string GetReportHeaders();

	string GetReportLine();

	void PrintReport( string filePath );

	abstract string ToString();
}
