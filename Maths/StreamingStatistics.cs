using RiskConsult.Extensions;
using System.Numerics;

namespace RiskConsult.Maths;

/// <summary>
/// Statistics operating on an IEnumerable in a single pass, without keeping the full data in memory. Can be used in a streaming way, e.g. on large
/// datasets not fitting into memory.
/// </summary>
public static class StreamingStatistics
{
	/// <inheritdoc cref="Correlation(IEnumerable{double}, IEnumerable{double})" />
	public static double Correlation<T, U>( IEnumerable<T> dataA, IEnumerable<U> dataB )
		where T : struct, INumberBase<T>
		where U : struct, INumberBase<U>
		=> Correlation( dataA.AsDoubles(), dataB.AsDoubles() );

	/// <summary> Computes the Pearson Product-Moment Correlation coefficient. </summary>
	/// <param name="dataA"> Sample data A. </param>
	/// <param name="dataB"> Sample data B. </param>
	/// <returns> The Pearson product-moment correlation coefficient. </returns>
	public static double Correlation( IEnumerable<double> dataA, IEnumerable<double> dataB )
	{
		var num = 0;
		var num2 = 0.0;
		var num3 = 0.0;
		var num4 = 0.0;
		var num5 = 0.0;
		var num6 = 0.0;
		using ( IEnumerator<double> enumerator = dataA.GetEnumerator() )
		{
			using IEnumerator<double> enumerator2 = dataB.GetEnumerator();
			while ( enumerator.MoveNext() )
			{
				if ( !enumerator2.MoveNext() )
				{
					throw new ArgumentOutOfRangeException( nameof( dataB ), "The array arguments must have the same length." );
				}

				var current = enumerator.Current;
				var current2 = enumerator2.Current;
				var num7 = current - num3;
				var num8 = num7 / ++num;
				var num9 = current2 - num4;
				var num10 = num9 / num;
				num3 += num8;
				num4 += num10;
				num5 += num8 * num7 * ( num - 1 );
				num6 += num10 * num9 * ( num - 1 );
				num2 += num7 * num9 * ( num - 1 ) / num;
			}

			if ( enumerator2.MoveNext() )
			{
				throw new ArgumentOutOfRangeException( nameof( dataA ), "The array arguments must have the same length." );
			}
		}

		return num2 / Math.Sqrt( num5 * num6 );
	}

	/// <summary>
	/// Estimates the unbiased population covariance from the provided two sample enumerable sequences, in a single pass without memoization. On a
	/// dataset of size N will use an N-1 normalizer (Bessel's correction).
	/// </summary>
	/// <param name="samples1"> First sample stream. </param>
	/// <param name="samples2"> Second sample stream. </param>
	/// <returns> Returns NaN if data has less than two entries or if any entry is NaN. </returns>
	public static double Covariance( IEnumerable<double> samples1, IEnumerable<double> samples2 )
	{
		var num = 0;
		var num2 = 0.0;
		var num3 = 0.0;
		var num4 = 0.0;
		using ( IEnumerator<double> enumerator = samples1.GetEnumerator() )
		{
			using IEnumerator<double> enumerator2 = samples2.GetEnumerator();
			while ( enumerator.MoveNext() )
			{
				if ( !enumerator2.MoveNext() )
				{
					throw new ArgumentException( "All vectors must have the same dimensionality." );
				}

				var num5 = num3;
				num++;
				num2 += ( enumerator.Current - num2 ) / num;
				num3 += ( enumerator2.Current - num3 ) / num;
				num4 += ( enumerator.Current - num2 ) * ( enumerator2.Current - num5 );
			}

			if ( enumerator2.MoveNext() )
			{
				throw new ArgumentException( "All vectors must have the same dimensionality." );
			}
		}

		return num <= 1 ? double.NaN : num4 / ( num - 1 );
	}

	/// <inheritdoc cref="Covariance(IEnumerable{double}, IEnumerable{double})" />
	public static double Covariance<T, U>( IEnumerable<T> samples1, IEnumerable<U> samples2 )
		where T : struct, INumberBase<T>
		where U : struct, INumberBase<U>
		=> Covariance( samples1.AsDoubles(), samples2.AsDoubles() );

	/// <summary> Calculates the entropy of a stream of double values. </summary>
	/// <param name="stream"> The input stream to evaluate. </param>
	/// <returns> Returns NaN if any of the values in the stream are NaN.. </returns>
	public static double Entropy( IEnumerable<double> stream )
	{
		Dictionary<double, double> dictionary = [];
		var num = 0;
		foreach ( var item in stream )
		{
			if ( double.IsNaN( item ) )
			{
				return double.NaN;
			}

			if ( dictionary.TryGetValue( item, out var value ) )
			{
				value = dictionary[ item ] = value + 1.0;
			}
			else
			{
				dictionary.Add( item, 1.0 );
			}

			num++;
		}

		var num3 = 0.0;
		foreach ( KeyValuePair<double, double> item2 in dictionary )
		{
			var num4 = item2.Value / num;
			num3 += num4 * System.Math.Log( num4, 2.0 );
		}

		return 0.0 - num3;
	}

	/// <inheritdoc cref="Entropy(IEnumerable{double})" />
	public static double Entropy<T>( IEnumerable<T> stream )
		where T : struct, INumberBase<T>
		=> Entropy( stream.AsDoubles() );

	/// <summary> Evaluates the geometric mean of the enumerable, in a single pass without memoization. </summary>
	/// <param name="stream"> Sample stream, no sorting is assumed. </param>
	/// <returns> Returns NaN if data is empty or any entry is NaN. </returns>
	public static double GeometricMean( IEnumerable<double> stream )
	{
		var num = 0uL;
		var num2 = 0.0;
		foreach ( var item in stream )
		{
			num2 += System.Math.Log( item );
			num++;
		}

		return num == 0 ? double.NaN : System.Math.Exp( num2 / num );
	}

	/// <inheritdoc cref="GeometricMean(IEnumerable{double})" />
	public static double GeometricMean<T>( IEnumerable<T> stream )
		where T : struct, INumberBase<T>
		=> GeometricMean( stream.AsDoubles() );

	/// <summary> Evaluates the harmonic mean of the enumerable, in a single pass without memoization. </summary>
	/// <param name="stream"> Sample stream, no sorting is assumed. </param>
	/// <returns> Returns NaN if data is empty or any entry is NaN. </returns>
	public static double HarmonicMean( IEnumerable<double> stream )
	{
		var num = 0uL;
		var num2 = 0.0;
		foreach ( var item in stream )
		{
			num2 += 1.0 / item;
			num++;
		}

		return num == 0 ? double.NaN : num / num2;
	}

	/// <inheritdoc cref="HarmonicMean(IEnumerable{double})" />
	public static double HarmonicMean<T>( IEnumerable<T> stream )
		where T : struct, INumberBase<T>
		=> HarmonicMean( stream.AsDoubles() );

	/// <summary> Returns the largest value from the enumerable, in a single pass without memoization. </summary>
	/// <param name="stream"> Sample stream, no sorting is assumed. </param>
	/// <returns> Returns NaN if data is empty or any entry is NaN. </returns>
	public static double Maximum( IEnumerable<double> stream )
	{
		var num = double.NegativeInfinity;
		var flag = false;
		foreach ( var item in stream )
		{
			if ( item > num || double.IsNaN( item ) )
			{
				num = item;
			}

			flag = true;
		}

		return !flag ? double.NaN : num;
	}

	/// <inheritdoc cref="Maximum(IEnumerable{double})" />
	public static double Maximum<T>( IEnumerable<T> stream )
		where T : struct, INumberBase<T>
		=> Maximum( stream.AsDoubles() );

	/// <summary> Returns the largest absolute value from the enumerable, in a single pass without memoization. </summary>
	/// <param name="stream"> Sample stream, no sorting is assumed. </param>
	/// <returns> Returns NaN if data is empty or any entry is NaN. </returns>
	public static double MaximumAbsolute( IEnumerable<double> stream )
	{
		var num = 0.0;
		var flag = false;
		foreach ( var item in stream )
		{
			if ( System.Math.Abs( item ) > num || double.IsNaN( item ) )
			{
				num = System.Math.Abs( item );
			}

			flag = true;
		}

		return !flag ? double.NaN : num;
	}

	/// <inheritdoc cref="MaximumAbsolute(IEnumerable{double})" />
	public static double MaximumAbsolute<T>( IEnumerable<T> stream )
		where T : struct, INumberBase<T>
		=> MaximumAbsolute( stream.AsDoubles() );

	/// <summary> Estimates the arithmetic sample mean from the enumerable, in a single pass without memoization. </summary>
	/// <param name="stream"> Sample stream, no sorting is assumed. </param>
	/// <returns> Returns NaN if data is empty or any entry is NaN. </returns>
	public static double Mean( IEnumerable<double> stream )
	{
		var num = 0.0;
		var num2 = 0uL;
		var flag = false;
		foreach ( var item in stream )
		{
			num += ( item - num ) / ++num2;
			flag = true;
		}

		return !flag ? double.NaN : num;
	}

	/// <inheritdoc cref="Mean(IEnumerable{double})" />
	public static double Mean<T>( IEnumerable<T> stream )
		where T : struct, INumberBase<T>
		=> Mean( stream.AsDoubles() );

	/// <summary>
	/// Estimates the arithmetic sample mean and the unbiased population standard deviation from the provided samples as enumerable sequence, in a
	/// single pass without memoization. On a dataset of size N will use an N-1 normalizer (Bessel's correction).
	/// </summary>
	/// <param name="samples"> Sample stream, no sorting is assumed. </param>
	/// <returns>
	/// Returns NaN for mean if data is empty or any entry is NaN, and NaN for standard deviation if data has less than two entries or if any entry is NaN.
	/// </returns>
	public static (double Mean, double StandardDeviation) MeanStandardDeviation( IEnumerable<double> samples )
	{
		(double, double) tuple = MeanVariance( samples );
		return (tuple.Item1, System.Math.Sqrt( tuple.Item2 ));
	}

	/// <inheritdoc cref="MeanStandardDeviation(IEnumerable{double})" />
	public static (double Mean, double StandardDeviation) MeanStandardDeviation<T>( IEnumerable<T> samples )
		where T : struct, INumberBase<T>
		=> MeanStandardDeviation( samples.AsDoubles() );

	/// <summary>
	/// Estimates the arithmetic sample mean and the unbiased population variance from the provided samples as enumerable sequence, in a single pass
	/// without memoization. On a dataset of size N will use an N-1 normalizer (Bessel's correction).
	/// </summary>
	/// <param name="samples"> Sample stream, no sorting is assumed. </param>
	/// <returns>
	/// Returns NaN for mean if data is empty or any entry is NaN, and NaN for variance if data has less than two entries or if any entry is NaN.
	/// </returns>
	public static (double Mean, double Variance) MeanVariance( IEnumerable<double> samples )
	{
		var num = 0.0;
		var num2 = 0.0;
		var num3 = 0.0;
		var num4 = 0uL;
		using ( IEnumerator<double> enumerator = samples.GetEnumerator() )
		{
			if ( enumerator.MoveNext() )
			{
				num4++;
				num3 = num = enumerator.Current;
			}

			while ( enumerator.MoveNext() )
			{
				num4++;
				var current = enumerator.Current;
				num3 += current;
				var num5 = ( num4 * current ) - num3;
				num2 += num5 * num5 / ( num4 * ( num4 - 1 ) );
				num += ( current - num ) / num4;
			}
		}

		return (num4 != 0 ? num : double.NaN, num4 > 1 ? num2 / ( num4 - 1 ) : double.NaN);
	}

	/// <inheritdoc cref="MeanVariance(IEnumerable{double})" />
	public static (double Mean, double Variance) MeanVariance<T>( IEnumerable<T> samples )
		where T : struct, INumberBase<T>
		=> MeanVariance( samples.AsDoubles() );

	/// <summary> Returns the smallest value from the enumerable, in a single pass without memoization. </summary>
	/// <param name="stream"> Sample stream, no sorting is assumed. </param>
	/// <returns> Returns NaN if data is empty or if any entry is NaN. </returns>
	public static double Minimum( IEnumerable<double> stream )
	{
		var num = double.PositiveInfinity;
		var flag = false;
		foreach ( var item in stream )
		{
			if ( item < num || double.IsNaN( item ) )
			{
				num = item;
			}

			flag = true;
		}

		return !flag ? double.NaN : num;
	}

	/// <inheritdoc cref="Minimum(IEnumerable{double})" />
	public static double Minimum<T>( IEnumerable<T> stream )
		where T : struct, INumberBase<T>
		=> Minimum( stream.AsDoubles() );

	/// <summary> Returns the smallest absolute value from the enumerable, in a single pass without memoization. </summary>
	/// <param name="stream"> Sample stream, no sorting is assumed. </param>
	/// <returns> Returns NaN if data is empty or if any entry is NaN. </returns>
	public static double MinimumAbsolute( IEnumerable<double> stream )
	{
		var num = double.PositiveInfinity;
		var flag = false;
		foreach ( var item in stream )
		{
			if ( System.Math.Abs( item ) < num || double.IsNaN( item ) )
			{
				num = System.Math.Abs( item );
			}

			flag = true;
		}

		return !flag ? double.NaN : num;
	}

	/// <inheritdoc cref="MinimumAbsolute(IEnumerable{double})" />
	public static double MinimumAbsolute<T>( IEnumerable<T> stream )
		where T : struct, INumberBase<T>
		=> MinimumAbsolute( stream.AsDoubles() );

	/// <summary>
	/// Evaluates the population covariance from the full population provided as two enumerable sequences, in a single pass without memoization. On a
	/// dataset of size N will use an N normalizer and would thus be biased if applied to a subset.
	/// </summary>
	/// <param name="population1"> First population stream. </param>
	/// <param name="population2"> Second population stream. </param>
	/// <returns> Returns NaN if data is empty or if any entry is NaN. </returns>
	public static double PopulationCovariance( IEnumerable<double> population1, IEnumerable<double> population2 )
	{
		var num = 0;
		var num2 = 0.0;
		var num3 = 0.0;
		var num4 = 0.0;
		using ( IEnumerator<double> enumerator = population1.GetEnumerator() )
		{
			using IEnumerator<double> enumerator2 = population2.GetEnumerator();
			while ( enumerator.MoveNext() )
			{
				if ( !enumerator2.MoveNext() )
				{
					throw new ArgumentException( "All vectors must have the same dimensionality." );
				}

				var num5 = num3;
				num++;
				num2 += ( enumerator.Current - num2 ) / num;
				num3 += ( enumerator2.Current - num3 ) / num;
				num4 += ( enumerator.Current - num2 ) * ( enumerator2.Current - num5 );
			}

			if ( enumerator2.MoveNext() )
			{
				throw new ArgumentException( "All vectors must have the same dimensionality." );
			}
		}

		return num4 / num;
	}

	/// <inheritdoc cref="PopulationCovariance(IEnumerable{double},IEnumerable{double})" />
	public static double PopulationCovariance<T, U>( IEnumerable<T> population1, IEnumerable<U> population2 )
		where T : struct, INumberBase<T>
		where U : struct, INumberBase<U>
		=> PopulationCovariance( population1.AsDoubles(), population2.AsDoubles() );

	/// <summary>
	/// Evaluates the population standard deviation from the full population provided as enumerable sequence, in a single pass without memoization. On
	/// a dataset of size N will use an N normalizer and would thus be biased if applied to a subset.
	/// </summary>
	/// <param name="population"> Sample stream, no sorting is assumed. </param>
	/// <returns> Returns NaN if data is empty or if any entry is NaN. </returns>
	public static double PopulationStandardDeviation( IEnumerable<double> population ) => Math.Sqrt( PopulationVariance( population ) );

	/// <inheritdoc cref="PopulationStandardDeviation(IEnumerable{double})" />
	public static double PopulationStandardDeviation<T>( IEnumerable<T> population )
		where T : struct, INumberBase<T>
		=> PopulationStandardDeviation( population.AsDoubles() );

	/// <summary>
	/// Evaluates the population variance from the full population provided as enumerable sequence, in a single pass without memoization. On a dataset
	/// of size N will use an N normalizer and would thus be biased if applied to a subset.
	/// </summary>
	/// <param name="population"> Sample stream, no sorting is assumed. </param>
	/// <returns> Returns NaN if data is empty or if any entry is NaN. </returns>
	public static double PopulationVariance( IEnumerable<double> population )
	{
		var num = 0.0;
		var num2 = 0.0;
		var num3 = 0uL;
		using ( IEnumerator<double> enumerator = population.GetEnumerator() )
		{
			if ( enumerator.MoveNext() )
			{
				num3++;
				num2 = enumerator.Current;
			}

			while ( enumerator.MoveNext() )
			{
				num3++;
				var current = enumerator.Current;
				num2 += current;
				var num4 = ( num3 * current ) - num2;
				num += num4 * num4 / ( num3 * ( num3 - 1 ) );
			}
		}

		return num / num3;
	}

	/// <inheritdoc cref="PopulationVariance(IEnumerable{double})" />
	public static double PopulationVariance<T>( IEnumerable<T> population )
		where T : struct, INumberBase<T>
		=> PopulationVariance( population.AsDoubles() );

	/// <summary> Estimates the root mean square (RMS) also known as quadratic mean from the enumerable, in a single pass without memoization. </summary>
	/// <param name="stream"> Sample stream, no sorting is assumed. </param>
	/// <returns> Returns NaN if data is empty or any entry is NaN. </returns>
	public static double RootMeanSquare( IEnumerable<double> stream )
	{
		var num = 0.0;
		var num2 = 0uL;
		var flag = false;
		foreach ( var item in stream )
		{
			num += ( ( item * item ) - num ) / ++num2;
			flag = true;
		}

		return !flag ? double.NaN : System.Math.Sqrt( num );
	}

	/// <inheritdoc cref="RootMeanSquare(IEnumerable{double})" />
	public static double RootMeanSquare<T>( IEnumerable<T> stream )
		where T : struct, INumberBase<T>
		=> RootMeanSquare( stream.AsDoubles() );

	/// <summary>
	/// Estimates the unbiased population standard deviation from the provided samples as enumerable sequence, in a single pass without memoization.
	/// On a dataset of size N will use an N-1 normalizer (Bessel's correction).
	/// </summary>
	/// <param name="samples"> Sample stream, no sorting is assumed. </param>
	/// <returns> Returns NaN if data has less than two entries or if any entry is NaN. </returns>
	public static double StandardDeviation( IEnumerable<double> samples ) => System.Math.Sqrt( Variance( samples ) );

	/// <inheritdoc cref="StandardDeviation(IEnumerable{double})" />
	public static double StandardDeviation<T>( IEnumerable<T> samples )
		where T : struct, INumberBase<T>
		=> StandardDeviation( samples.AsDoubles() );

	/// <summary>
	/// Estimates the unbiased population variance from the provided samples as enumerable sequence, in a single pass without memoization. On a
	/// dataset of size N will use an N-1 normalizer (Bessel's correction).
	/// </summary>
	/// <param name="samples"> Sample stream, no sorting is assumed. </param>
	/// <returns> Returns NaN if data has less than two entries or if any entry is NaN. </returns>
	public static double Variance( IEnumerable<double> samples )
	{
		var num = 0.0;
		var num2 = 0.0;
		var num3 = 0uL;
		using ( IEnumerator<double> enumerator = samples.GetEnumerator() )
		{
			if ( enumerator.MoveNext() )
			{
				num3++;
				num2 = enumerator.Current;
			}

			while ( enumerator.MoveNext() )
			{
				num3++;
				var current = enumerator.Current;
				num2 += current;
				var num4 = ( num3 * current ) - num2;
				num += num4 * num4 / ( num3 * ( num3 - 1 ) );
			}
		}

		return num3 <= 1 ? double.NaN : num / ( num3 - 1 );
	}

	/// <inheritdoc cref="Variance(IEnumerable{double})" />
	public static double Variance<T>( IEnumerable<T> samples )
		where T : struct, INumberBase<T>
		=> Variance( samples.AsDoubles() );
}
