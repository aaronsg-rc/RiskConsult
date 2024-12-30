namespace RiskConsult._Tests;

public interface IDataExporter<T>
{
	void ExportData( T data );
}
