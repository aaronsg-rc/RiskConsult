using System.Reflection;

namespace RiskConsult.Data;

public interface IPropertyMap
{
	string ColumnFormat { get; }
	int ColumnIndex { get; }
	string ColumnName { get; }
	bool IsPrimaryKey { get; }
	PropertyInfo PropertyInfo { get; }
}

public class PropertyMap<T> : IPropertyMap
{
	public string ColumnFormat { get; }
	public int ColumnIndex { get; }
	public string ColumnName { get; }
	public bool IsPrimaryKey { get; }
	public PropertyInfo PropertyInfo { get; }

	public PropertyMap( string propertName, string colName = "", int colIndex = -1, bool isPK = false, string colFormat = "" )
	{
		PropertyInfo = typeof( T ).GetProperty( propertName ) ?? throw new ArgumentException( $"Invalid property name {propertName}" );
		ColumnName = string.IsNullOrEmpty( colName ) ? PropertyInfo.Name : colName;
		ColumnIndex = colIndex;
		ColumnFormat = colFormat;
		IsPrimaryKey = isPK;
	}
}
