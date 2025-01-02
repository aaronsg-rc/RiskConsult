using RiskConsult.Core;
using RiskConsult.Data;
using RiskConsult.Enumerators;

namespace RiskConsult.Extensions;

public static class PortfolioExtensions
{
	public static IPortfolio GetPortfolio( this string name, DateTime date, PriceSourceId sourceId, CurrencyId fxCurrency )
	{
		return
			DbZeus.Db.User?.Portfolios.GetPortfolio( date, name, sourceId, fxCurrency ) ??
			DbZeus.Db.Portfolios.GetPortfolio( date, name, sourceId, fxCurrency ) ??
			throw new InvalidOperationException( $"Any portfolio available for {name}" );
	}

	public static IPortfolio GetPortfolio( this string name, DateTime date )
	{
		return
			DbZeus.Db.User?.Portfolios.GetPortfolio( date, name ) ??
			DbZeus.Db.Portfolios.GetPortfolio( date, name ) ??
			throw new InvalidOperationException( $"Any portfolio available for {name}" );
	}

	public static void SaveToDatabase( this IPortfolio portfolio )
	{
		ArgumentNullException.ThrowIfNull( DbZeus.Db.User, nameof( DbZeus.Db.User ) );
		DbZeus.Db.User.Portfolios.SavePortfolioToDatabase( portfolio );
	}

	/// <summary> Establece la cantidad de títulos de cada instrumento dentro del portafolio a partir de los títulos </summary>
	/// <param name="dictAmount"> diccionario de IDs y el amount correspondiente </param>
	public static void SetAmounts( this IPortfolio portfolio, Dictionary<int, double> dictAmount )
	{
		foreach ( KeyValuePair<int, double> dicHold in dictAmount )
		{
			IHolding hold = portfolio[ dicHold.Key ] ?? throw new Exception( $"Holding {dicHold.Key} missing" );
			hold.Amount = dicHold.Value;
		}
	}

	/// <summary> Establece la cantidadde títulos de cada instrumento dentro del portafolio a partir de los valores </summary>
	/// <param name="dictValues"> diccionario de IDs y el valores correspondiente </param>
	public static void SetAmountsByValues( this IPortfolio portfolio, Dictionary<int, double> dictValues )
	{
		foreach ( KeyValuePair<int, double> dicHold in dictValues )
		{
			IHolding hold = portfolio[ dicHold.Key ] ?? throw new Exception( $"Holding {dicHold.Key} missing" );
			hold.Amount = dicHold.Value / hold.Price;
		}
	}

	/// <summary> Establece la cantidadde títulos de cada instrumento dentro del portafolio a partir de la ponderación </summary>
	/// <param name="dictWeights"> diccionario de IDs y el Weight correspondiente base 1 </param>
	public static void SetAmountsByWeights( this IPortfolio portfolio, Dictionary<int, double> dictWeights, double portfolioValue )
	{
		foreach ( KeyValuePair<int, double> dicHold in dictWeights )
		{
			IHolding hold = portfolio[ dicHold.Key ] ?? throw new Exception( $"Holding {dicHold.Key} missing" );
			hold.Amount = dicHold.Value * portfolioValue / hold.Price;
		}
	}
}
