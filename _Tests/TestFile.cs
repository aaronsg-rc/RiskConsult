using System.Data;
using System.Reflection;

namespace RiskConsult._Tests;

public interface IDataArrayProvider
{
	object[] GetDataArray();
}

public interface IDataCsvProvider
{
	string GetCsvLine();
}

public interface IDataProvider : IDataRowProvider, IDataArrayProvider, IDataCsvProvider
{
	public object GetColumnValue( string name );
}

public interface IDataRowProvider
{
	public DataRow GetDataRow( DataTable table );
}

public interface IHeadersProvider
{
	IEnumerable<string> GetHeaders();
}

public interface ITableColumnMapping
{
	public Dictionary<PropertyInfo, string> ColumnMappings
	{
		get;
	}
	public string TableName
	{
		get;
	}
}
