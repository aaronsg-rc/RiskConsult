using MathNet.Numerics.Statistics;
using RiskConsult.Core;
using RiskConsult.Data;
using RiskConsult.Extensions;
using RiskConsult.Maths;

namespace RiskConsult._Tests;

public static class Risk
{
	public static double GetHistoricalVaR( DateTime date, IEnumerable<int> factorsIDs, int scenariosCount, IEnumerable<double> exposures, double alpha )
	{
		IEnumerable<IFactor> factors = factorsIDs.Select( i => i.GetFactor() );
		IEnumerable<DateTime> dates = DbZeus.Db.Dates.GetBusinessLastDaysFrom( date, scenariosCount );
		var factorReturns = factors.GetFactorsReturnsMatrix( dates );
		return GetHistoricalVaR( exposures, factorReturns, alpha );
	}

	public static double GetHistoricalVaR( IEnumerable<double> weights, double[,] returns, double alpha )
	{
		var weightedReturns = returns.Multiply( weights.ToColumnArray() );
		return GetHistoricalVaR( weightedReturns.GetColumn( 0 ), alpha );
	}

	public static double GetHistoricalVaR( IEnumerable<double> returns, double alpha )
		=> returns.Quantile( 1 - alpha );

	public static double GetParametricVaR( double[,] returns, IEnumerable<double> weights, double alpha )
		=> GetParametricVaR( returns, weights, alpha, 1, 1 );

	public static double GetParametricVaR( double[,] returns, IEnumerable<double> weights, double alpha, double lambda )
		=> GetParametricVaR( returns, weights, alpha, lambda, 1 );

	/// <summary> Calcula el valor en riesgo relativo </summary>
	/// <param name="returns"> Matriz de rendimientos [Marix, Arr1D, Arr2D, Vector] </param>
	/// <param name="weights"> Vector de ponderaciones [Marix, Arr1D, Arr2D, Vector] </param>
	/// <param name="alpha"> Nivel de confianza entre 0 y 1 </param>
	/// <returns> Valor en riesgo con base 1 </returns>
	public static double GetParametricVaR( double[,] returns, IEnumerable<double> weights, double alpha, double lambda, int timeScale )
	{
		var mCovariances = CovarianceMatrix.FromReturns( returns, lambda );
		var rowWeights = weights.ToRowArray();
		var colWeights = weights.ToColumnArray();
		var variance = rowWeights
			.Multiply( mCovariances.Values )
			.Multiply( colWeights )[ 0, 0 ];

		return GetParametricVaR( Math.Sqrt( variance ), alpha, timeScale );
	}

	public static double GetParametricVaR( DateTime date, IEnumerable<int> factorsIDs, int scenariosCount,
		IEnumerable<double> weights, double alpha )
		=> GetParametricVaR( date, factorsIDs, scenariosCount, weights, alpha, 1, 1 );

	public static double GetParametricVaR( DateTime date, IEnumerable<int> factorsIDs, int scenariosCount,
		IEnumerable<double> weights, double alpha, double lambda )
		=> GetParametricVaR( date, factorsIDs, scenariosCount, weights, alpha, lambda, 1 );

	public static double GetParametricVaR( DateTime date, IEnumerable<int> factorsIDs, int scenariosCount,
		IEnumerable<double> weights, double alpha, double lambda, int timeScale )
	{
		IEnumerable<IFactor> factors = factorsIDs.Select( i => i.GetFactor() );
		IEnumerable<DateTime> dates = DbZeus.Db.Dates.GetBusinessLastDaysFrom( date, scenariosCount );
		var factorReturns = factors.GetFactorsReturnsMatrix( dates );
		return GetParametricVaR( factorReturns, weights, alpha, lambda, timeScale );
	}

	public static double GetParametricVaR( double stdDev, double alpha, int timeScale )
		=> stdDev * GetZScore( alpha ) * Math.Sqrt( timeScale );

	public static double GetZScore( double alpha ) => GetNormalInvCDF( alpha );

	private static double GetNormalInvCDF( double alpha ) => throw new NotImplementedException();
}
