using Microsoft.Data.SqlClient;
using RiskConsult.Enumerators;

namespace RiskConsult.Utilities.PortfolioComparer;

public interface IPortfolioProvider
{
	Portfolio GetPortfolio( DateTime date, string portfolio );
}

public class PortfolioDbProvider( string connString, PriceSourceId sourceID = PriceSourceId.PiP_MD ) : IPortfolioProvider
{
	public Portfolio GetPortfolio( DateTime date, string name )
	{
		var portfolio = new Portfolio()
		{
			Date = date,
			Name = name,
		};

		using var conn = new SqlConnection( connString );
		conn.Open();

		using SqlCommand cmd = conn.CreateCommand();
		cmd.CommandText =
			"SELECT H.txtHoldingDescription, P.dblAmount, Q.dblQuote " +
			"FROM tblPortfolio P " +
			"LEFT JOIN tblDATA_Quotes Q ON Q.dteDate = P.dteDate AND Q.intID = P.intID " +
			"LEFT JOIN tblMAP_Holdings H ON H.intholdingId = P.intID " +
			"WHERE P.dteDate = @date " +
			"AND P.txtPortfolioID = @name " +
			"AND Q.intSourceID = @sourceId";
		cmd.Parameters.AddWithValue( "@date", date );
		cmd.Parameters.AddWithValue( "@name", name );
		cmd.Parameters.AddWithValue( "@sourceId", sourceID );

		using SqlDataReader reader = cmd.ExecuteReader();
		while ( reader.Read() )
		{
			var holding = new Holding()
			{
				Date = date,
				Name = reader.GetString( 0 ),
				Amount = reader.GetDouble( 1 ),
				Price = reader.GetDouble( 2 )
			};

			portfolio.Holdings.Add( holding );
		}

		return portfolio;
	}
}
