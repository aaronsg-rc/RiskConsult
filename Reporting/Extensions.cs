using System.Text;

namespace RiskConsult.Reporting;

public static class Extensions
{
	public static void PrintToCsvFile<T>( this IEnumerable<T> entities, string filePath ) where T : class, ILineProvider, IHeadersProvider, IReportModel
	{
		if ( !entities.Any() )
		{
			return;
		}

		using var writer = new StreamWriter( filePath, false, Encoding.UTF8 );
		writer.WriteLine( entities.First().GetHeaders() );

		foreach ( T entity in entities )
		{
			writer.WriteLine( entity.GetLine() );
		}
	}
}
