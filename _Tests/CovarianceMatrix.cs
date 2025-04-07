using MathNet.Numerics.Statistics;
using RiskConsult.Extensions;
using System.Data;

namespace RiskConsult._Tests;

public class CovarianceMatrix
{
	public double this[ int row, int col ] => Values[ row, col ];
	public readonly int DataCount;
	public readonly double Lambda;
	public readonly double[,] Values;

	private CovarianceMatrix( double[,] covariances, double lambda, int dataCount )
	{
		Lambda = lambda;
		Values = covariances;
		DataCount = dataCount;
	}

	public static (double[], double[]) CleanOutliers( IEnumerable<double> values, IEnumerable<double> others, int threshold = 5 )
	{
		// Filtro
		var x = values.ToArray();
		var y = others.ToArray();
		if ( x.Length != y.Length )
		{
			throw new ArgumentException( "Vectors size must be the same" );
		}

		// Cálculos para filtro
		var stdDev = values.PopulationStandardDeviation();
		var stdDevAvg = GetMeanStdDevAvg( values );
		var median = values.Median();

		//calculos x
		var x_average = x.Average();
		var x_std = x.PopulationStandardDeviation();
		var x_ex = threshold * ( stdDevAvg + median );// stdDev + median;

		//cálculos y
		var y_average = y.Average();
		var y_std = y.PopulationStandardDeviation();
		var y_ex = x_ex;

		//filtro
		for ( var i = 0; i < x.Length; i++ )
		{
			if ( Math.Abs( x[ i ] - ( threshold * median ) ) >= x_ex ||
				Math.Abs( y[ i ] - ( threshold * median ) ) >= y_ex )
			{
				x[ i ] = double.NaN;
				y[ i ] = double.NaN;
			}
		}

		return (
			x.Where( v => !double.IsNaN( v ) ).ToArray(),
			y.Where( v => !double.IsNaN( v ) ).ToArray());
	}

	public static CovarianceMatrix FromReturns( double[,] returns, double lambda )
	{
		var size = returns.GetLength( 1 );
		var results = new double[ size, size ];
		for ( var i = 0; i < size; i++ )
		{
			for ( var j = i; j < size; j++ )
			{
				var covariance = GetExponentialAttenuatedCovariance( returns.GetColumn( i ), returns.GetColumn( j ), lambda );
				results[ i, j ] = covariance;
				results[ j, i ] = covariance;
			}
		}

		return new CovarianceMatrix( results, lambda, returns.GetLength( 0 ) );
	}

	public static CovarianceMatrix FromReturns( double[,] returns ) => FromReturns( returns, 1 );

	public static double GetExponentialAttenuatedCovariance( IEnumerable<double> values, IEnumerable<double> others, double lambda )
	{
		(var x, var y) = CleanOutliers( values, others, 5 );

		//return lambdaCovariance;
		var n = x.Length;
		var x_average = x.Average();
		var y_average = y.Average();
		var weightedSum = 0.0;
		var sumWeights = 0.0;
		for ( var i = 0; i < n; i++ )
		{
			var attenuation = Math.Pow( lambda, n - i - 1 );
			weightedSum += attenuation * ( x[ i ] - x_average ) * ( y[ i ] - y_average );
			sumWeights += attenuation;
		}

		return weightedSum / sumWeights;
	}

	public static double GetMeanStdDevAvg( IEnumerable<double> values )
	{
		var average = values.Average();
		return values.Select( v => Math.Abs( v - average ) ).Sum() / values.Count();
	}

	public static double MedianAbsoluteDeviation( IEnumerable<double> values )
	{
		var median = values.Median();
		return values.Select( v => Math.Abs( v - median ) ).Sum() / values.Count();
	}
}
