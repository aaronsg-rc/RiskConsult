namespace RiskConsult.Maths;

/// <summary> Extension methods to return basic statistics on set of data. </summary>
public static class Statistics
{
	/// <summary> Computes the Pearson Product-Moment Correlation coefficient. </summary>
	/// <param name="dataA"> Sample data A. </param>
	/// <param name="dataB"> Sample data B. </param>
	/// <returns> The Pearson product-moment correlation coefficient. </returns>
	public static double Correlation( this IEnumerable<double> dataA, IEnumerable<double> dataB )
	{
		return StreamingStatistics.Correlation( dataA, dataB );
	}

	/// <summary>
	/// Estimates the unbiased population covariance from the provided samples. On a dataset of size N will use an N-1 normalizer (Bessel's
	/// correction). Returns NaN if data has less than two entries or if any entry is NaN.
	/// </summary>
	/// <param name="samples1"> A subset of samples, sampled from the full population. </param>
	/// <param name="samples2"> A subset of samples, sampled from the full population. </param>
	public static double Covariance( this IEnumerable<double> samples1, IEnumerable<double> samples2 )
	{
		if ( samples1 is double[] samples3 && samples2 is double[] samples4 )
		{
			return ArrayStatistics.Covariance( samples3, samples4 );
		}
		else
		{
			return StreamingStatistics.Covariance( samples1, samples2 );
		}
	}

	/// <summary> Calculates the entropy of a stream of double values in bits. Returns NaN if any of the values in the stream are NaN. </summary>
	/// <param name="data"> The data sample sequence. </param>
	public static double Entropy( this IEnumerable<double> data )
	{
		return StreamingStatistics.Entropy( data );
	}

	/// <summary> Evaluates the geometric mean. Returns NaN if data is empty or if any entry is NaN. </summary>
	/// <param name="data"> The data to calculate the geometric mean of. </param>
	/// <returns> The geometric mean of the sample. </returns>
	public static double GeometricMean( this IEnumerable<double> data )
	{
		return data is not double[] data2 ? StreamingStatistics.GeometricMean( data ) : ArrayStatistics.GeometricMean( data2 );
	}

	/// <summary> Evaluates the harmonic mean. Returns NaN if data is empty or if any entry is NaN. </summary>
	/// <param name="data"> The data to calculate the harmonic mean of. </param>
	/// <returns> The harmonic mean of the sample. </returns>
	public static double HarmonicMean( this IEnumerable<double> data )
	{
		return data is not double[] data2 ? StreamingStatistics.HarmonicMean( data ) : ArrayStatistics.HarmonicMean( data2 );
	}

	/// <summary> Returns the maximum value in the sample data. Returns NaN if data is empty or if any entry is NaN. </summary>
	/// <param name="data"> The sample data. </param>
	/// <returns> The maximum value in the sample data. </returns>
	public static double Maximum( this IEnumerable<double> data )
	{
		return data is not double[] data2 ? StreamingStatistics.Maximum( data ) : ArrayStatistics.Maximum( data2 );
	}

	/// <summary> Returns the maximum absolute value in the sample data. Returns NaN if data is empty or if any entry is NaN. </summary>
	/// <param name="data"> The sample data. </param>
	/// <returns> The maximum value in the sample data. </returns>
	public static double MaximumAbsolute( this IEnumerable<double> data )
	{
		return data is not double[] data2 ? StreamingStatistics.MaximumAbsolute( data ) : ArrayStatistics.MaximumAbsolute( data2 );
	}

	/// <summary> Evaluates the sample mean, an estimate of the population mean. Returns NaN if data is empty or if any entry is NaN. </summary>
	/// <param name="data"> The data to calculate the mean of. </param>
	/// <returns> The mean of the sample. </returns>
	public static double Mean( this IEnumerable<double> data )
	{
		return data is not double[] data2 ? StreamingStatistics.Mean( data ) : ArrayStatistics.Mean( data2 );
	}

	/// <summary>
	/// Estimates the sample mean and the unbiased population standard deviation from the provided samples. On a dataset of size N will use an N-1
	/// normalizer (Bessel's correction). Returns NaN for mean if data is empty or if any entry is NaN and NaN for standard deviation if data has less
	/// than two entries or if any entry is NaN.
	/// </summary>
	/// <param name="samples"> The data to calculate the mean of. </param>
	/// <returns> The mean of the sample. </returns>
	public static (double Mean, double StandardDeviation) MeanStandardDeviation( this IEnumerable<double> samples )
	{
		if ( samples is double[] samples2 )
		{
			return ArrayStatistics.MeanStandardDeviation( samples2 );
		}
		else
		{
			return StreamingStatistics.MeanStandardDeviation( samples );
		}
	}

	/// <summary> Estimates the sample mean and the unbiased population variance from the provided samples. On a dataset of size N will use an N-1
	/// normalizer (Bessel's correction). Returns NaN for mean if data is empty or if any entry is NaN and NaN for variance if data has less than two
	/// entries or if any entry is NaN. </summary> <param name="samples"><The data to calculate the mean of./param> <returns>The mean of the sample.</returns>
	public static (double Mean, double Variance) MeanVariance( this IEnumerable<double> samples )
	{
		if ( samples is double[] samples2 )
		{
			return ArrayStatistics.MeanVariance( samples2 );
		}
		else
		{
			return StreamingStatistics.MeanVariance( samples );
		}
	}

	/// <summary> Estimates the sample median from the provided samples (R8). </summary>
	public static double Median( this IEnumerable<double> values )
	{
		var sortedValues = values.OrderBy( v => v ).ToList();
		var count = sortedValues.Count;
		return count == 0
			? throw new InvalidOperationException( "No se pueden calcular la mediana de una colección vacía." )
			: count % 2 == 1 ? sortedValues[ count / 2 ] : ( sortedValues[ ( count / 2 ) - 1 ] + sortedValues[ count / 2 ] ) / 2.0;
	}

	/// <summary> Returns the minimum value in the sample data. Returns NaN if data is empty or if any entry is NaN. </summary>
	/// <param name="data"> The sample data. </param>
	/// <returns> The minimum value in the sample data. </returns>
	public static double Minimum( this IEnumerable<double> data ) => data is not double[] data2 ? StreamingStatistics.Minimum( data ) : ArrayStatistics.Minimum( data2 );

	/// <summary> Returns the minimum absolute value in the sample data. Returns NaN if data is empty or if any entry is NaN. </summary>
	/// <param name="data"> The sample data. </param>
	/// <returns> The minimum value in the sample data. </returns>
	public static double MinimumAbsolute( this IEnumerable<double> data ) => data is not double[] data2 ? StreamingStatistics.MinimumAbsolute( data ) : ArrayStatistics.MinimumAbsolute( data2 );

	/// <summary> Obtiene el producto punto de la multiplicacion de dos vectores enumerables del mismo tamaño </summary>
	public static double Multiply( this IList<double> values, IList<double> others )
	{
		if ( values.Count != others.Count )
		{
			throw new ArgumentException( "Vectors must be of the same size" );
		}

		var sum = 0.0;
		for ( var i = 0; i < values.Count; i++ )
		{
			sum += values[ i ] * others[ i ];
		}

		return sum;
	}

	public static double Percentile( this IEnumerable<double> values, double percentile )
	{
		var sortedValues = values.OrderBy( v => v ).ToList();
		var N = sortedValues.Count;
		if ( N == 0 )
		{
			return double.NaN;
		}

		var n = ( ( N - 1 ) * percentile ) + 1;
		if ( n == 1d )
		{
			return sortedValues[ 0 ];
		}
		else if ( n == N )
		{
			return sortedValues[ N - 1 ];
		}
		else
		{
			var k = ( int ) n;
			var d = n - k;
			return sortedValues[ k - 1 ] + ( d * ( sortedValues[ k ] - sortedValues[ k - 1 ] ) );
		}
	}

	/// <summary>
	/// Evaluates the population covariance from the provided full populations. On a dataset of size N will use an N normalizer and would thus be
	/// biased if applied to a subset. Returns NaN if data is empty or if any entry is NaN.
	/// </summary>
	/// <param name="population1"> The full population data. </param>
	/// <param name="population2"> The full population data. </param>
	public static double PopulationCovariance( this IEnumerable<double> population1, IEnumerable<double> population2 )
	{
		return population1 is not double[] population3 || population2 is not double[] population4
			? StreamingStatistics.PopulationCovariance( population1, population2 )
			: ArrayStatistics.PopulationCovariance( population3, population4 );
	}

	/// <summary>
	/// Evaluates the standard deviation from the provided full population. On a dataset of size N will use an N normalizer and would thus be biased
	/// if applied to a subset. Returns NaN if data is empty or if any entry is NaN.
	/// </summary>
	/// <param name="population"> The full population data. </param>
	public static double PopulationStandardDeviation( this IEnumerable<double> population )
	{
		return population is not double[] population2
			? StreamingStatistics.PopulationStandardDeviation( population )
			: ArrayStatistics.PopulationStandardDeviation( population2 );
	}

	/// <summary>
	/// Evaluates the variance from the provided full population. On a dataset of size N will use an N normalizer and would thus be biased if applied
	/// to a subset. Returns NaN if data is empty or if any entry is NaN.
	/// </summary>
	/// <param name="population"> The full population data. </param>
	public static double PopulationVariance( this IEnumerable<double> population )
	{
		return population is not double[] population2
			? StreamingStatistics.PopulationVariance( population )
			: ArrayStatistics.PopulationVariance( population2 );
	}

	/// <summary> Evaluates the root mean square (RMS) also known as quadratic mean. Returns NaN if data is empty or if any entry is NaN. </summary>
	/// <param name="data"> The data to calculate the RMS of. </param>
	public static double RootMeanSquare( this IEnumerable<double> data ) => data is not double[] data2 ? StreamingStatistics.RootMeanSquare( data ) : ArrayStatistics.RootMeanSquare( data2 );

	/// <summary>
	/// Estimates the unbiased population standard deviation from the provided samples. On a dataset of size N will use an N-1 normalizer (Bessel's
	/// correction). Returns NaN if data has less than two entries or if any entry is NaN.
	/// </summary>
	/// <param name="samples"> A subset of samples, sampled from the full population. </param>
	public static double StandardDeviation( this IEnumerable<double> samples )
	{
		return samples is not double[] samples2
			? StreamingStatistics.StandardDeviation( samples )
			: ArrayStatistics.StandardDeviation( samples2 );
	}

	/// <summary>
	/// Estimates the unbiased population variance from the provided samples. On a dataset of size N will use an N-1 normalizer (Bessel's correction).
	/// Returns NaN if data has less than two entries or if any entry is NaN.
	/// </summary>
	/// <param name="samples"> A subset of samples, sampled from the full population. </param>
	public static double Variance( this IEnumerable<double> samples )
	{
		if ( samples is double[] samples2 )
		{
			return ArrayStatistics.Variance( samples2 );
		}
		else
		{
			return StreamingStatistics.Variance( samples );
		}
	}

	/// <summary> Computes the Weighted Pearson Product-Moment Correlation coefficient. </summary>
	/// <param name="dataA"> Sample data A. </param>
	/// <param name="dataB"> Sample data B. </param>
	/// <param name="weights"> Corresponding weights of data. </param>
	/// <returns> The Weighted Pearson product-moment correlation coefficient. </returns>
	public static double WeightedCorrelation( IEnumerable<double> dataA, IEnumerable<double> dataB, IEnumerable<double> weights )
	{
		var num = 0.0;
		var num2 = 0.0;
		var num3 = 0.0;
		var num4 = 0.0;
		var num5 = 0.0;
		var num6 = 0.0;
		using ( IEnumerator<double> enumerator = dataA.GetEnumerator() )
		{
			using IEnumerator<double> enumerator2 = dataB.GetEnumerator();
			using IEnumerator<double> enumerator3 = weights.GetEnumerator();
			while ( enumerator.MoveNext() )
			{
				if ( !enumerator2.MoveNext() )
				{
					throw new ArgumentOutOfRangeException( nameof( dataB ), "The array arguments must have the same length." );
				}

				if ( !enumerator3.MoveNext() )
				{
					throw new ArgumentOutOfRangeException( nameof( weights ), "The array arguments must have the same length." );
				}

				var current = enumerator.Current;
				var current2 = enumerator2.Current;
				var current3 = enumerator3.Current;
				var num7 = num5 + current3;
				var num8 = current - num;
				var num9 = num8 * current3 / num7;
				num += num9;
				num3 += num5 * num8 * num9;
				var num10 = current2 - num2;
				var num11 = num10 * current3 / num7;
				num2 += num11;
				num4 += num5 * num10 * num11;
				num6 += num8 * num10 * current3 * ( num5 / num7 );
				num5 = num7;
			}

			if ( enumerator2.MoveNext() )
			{
				throw new ArgumentOutOfRangeException( nameof( dataB ), "The array arguments must have the same length." );
			}

			if ( enumerator3.MoveNext() )
			{
				throw new ArgumentOutOfRangeException( nameof( weights ), "The array arguments must have the same length." );
			}
		}

		return num6 / System.Math.Sqrt( num3 * num4 );
	}
}
