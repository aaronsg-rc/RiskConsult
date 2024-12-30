namespace RiskConsult.Data;

public interface ICsvReaderOptions
{
	char Delimiter { get; set; }
	bool HasHeaders { get; set; }
}

public class CsvReaderOptions : ICsvReaderOptions
{
	public char Delimiter { get; set; } = ',';
	public bool HasHeaders { get; set; } = false;
}
