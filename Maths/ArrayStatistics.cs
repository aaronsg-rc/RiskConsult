using RiskConsult.Extensions;
using System.Numerics;

namespace RiskConsult.Maths;

/// <summary>
/// Statistics operating on arrays assumed to be unsorted. WARNING: Methods with the Inplace-suffix may modify the data array by reordering its entries.
/// </summary>
public static class ArrayStatistics
{
	/// <summary>
	/// Estimates the unbiased population covariance from the provided two sample arrays. On a dataset of size N will use an N-1 normalizer (Bessel's
	/// correction). Returns NaN if data has less than two entries or if any entry is NaN.
	/// </summary>
	/// <param name="samples1"> First sample array. </param>
	/// <param name="samples2"> Second sample array. </param>
	public static double Covariance( double[] samples1, double[] samples2 )
	{
		if ( samples1.Length != samples2.Length )
		{
			throw new ArgumentException( "All vectors must have the same dimensionality." );
		}

		if ( samples1.Length <= 1 )
		{
			return double.NaN;
		}

		var num = Mean( samples1 );
		var num2 = Mean( samples2 );
		var num3 = 0.0;
		for ( var i = 0; i < samples1.Length; i++ )
		{
			num3 += ( samples1[ i ] - num ) * ( samples2[ i ] - num2 );
		}

		return num3 / ( samples1.Length - 1 );
	}

	/// <inheritdoc cref="Covariance(double[], double[])" />
	public static double Covariance<T, U>( T[] samples1, U[] samples2 )
		where T : struct, INumberBase<T>
		where U : struct, INumberBase<U>
		=> Covariance( samples1.AsDoublesArray(), samples2.AsDoublesArray() );

	/// <summary> Evaluates the geometric mean of the unsorted data array. Returns NaN if data is empty or any entry is NaN. </summary>
	/// <param name="data"> Sample array, no sorting is assumed. </param>
	public static double GeometricMean( double[] data )
	{
		if ( data.Length == 0 )
		{
			return double.NaN;
		}

		var num = 0.0;
		for ( var i = 0; i < data.Length; i++ )
		{
			num += System.Math.Log( data[ i ] );
		}

		return System.Math.Exp( num / data.Length );
	}

	/// <inheritdoc cref="GeometricMean(double[])" />
	public static double GeometricMean<T>( T[] data )
		where T : struct, INumberBase<T>
		=> GeometricMean( data.AsDoublesArray() );

	/// <summary> Evaluates the harmonic mean of the unsorted data array. Returns NaN if data is empty or any entry is NaN. </summary>
	/// <param name="data"> Sample array, no sorting is assumed. </param>
	public static double HarmonicMean( double[] data )
	{
		if ( data.Length == 0 )
		{
			return double.NaN;
		}

		var num = 0.0;
		for ( var i = 0; i < data.Length; i++ )
		{
			num += 1.0 / data[ i ];
		}

		return data.Length / num;
	}

	/// <inheritdoc cref="HarmonicMean(double[])" />
	public static double HarmonicMean<T>( T[] data )
		where T : struct, INumberBase<T>
		=> HarmonicMean( data.AsDoublesArray() );

	/// <summary> Returns the largest value from the unsorted data array. Returns NaN if data is empty or any entry is NaN. </summary>
	/// <param name="data"> Sample array, no sorting is assumed. </param>
	public static double Maximum( double[] data )
	{
		if ( data.Length == 0 )
		{
			return double.NaN;
		}

		var num = double.NegativeInfinity;
		for ( var i = 0; i < data.Length; i++ )
		{
			if ( data[ i ] > num || double.IsNaN( data[ i ] ) )
			{
				num = data[ i ];
			}
		}

		return num;
	}

	/// <inheritdoc cref="Maximum(double[])" />
	public static double Maximum<T>( T[] data )
		where T : struct, INumberBase<T>
		=> Maximum( data.AsDoublesArray() );

	/// <summary> Returns the largest absolute value from the unsorted data array. Returns NaN if data is empty or any entry is NaN. </summary>
	/// <param name="data"> Sample array, no sorting is assumed. </param>
	public static double MaximumAbsolute( double[] data )
	{
		if ( data.Length == 0 )
		{
			return double.NaN;
		}

		var num = 0.0;
		for ( var i = 0; i < data.Length; i++ )
		{
			if ( System.Math.Abs( data[ i ] ) > num || double.IsNaN( data[ i ] ) )
			{
				num = System.Math.Abs( data[ i ] );
			}
		}

		return num;
	}

	/// <inheritdoc cref="MaximumAbsolute(double[])" />
	public static double MaximumAbsolute<T>( T[] data )
		where T : struct, INumberBase<T>
		=> MaximumAbsolute( data.AsDoublesArray() );

	/// <summary> Estimates the arithmetic sample mean from the unsorted data array. Returns NaN if data is empty or any entry is NaN. </summary>
	/// <param name="data"> Sample array, no sorting is assumed. </param>
	public static double Mean( double[] data )
	{
		if ( data.Length == 0 )
		{
			return double.NaN;
		}

		var num = 0.0;
		var num2 = 0uL;
		for ( var i = 0; i < data.Length; i++ )
		{
			num += ( data[ i ] - num ) / ++num2;
		}

		return num;
	}

	/// <inheritdoc cref="Mean(double[])" />
	public static double Mean<T>( T[] data )
		where T : struct, INumberBase<T>
		=> Mean( data.AsDoublesArray() );

	/// <summary>
	/// Estimates the arithmetic sample mean and the unbiased population standard deviation from the provided samples as unsorted array. On a dataset
	/// of size N will use an N-1 normalizer (Bessel's correction). Returns NaN for mean if data is empty or any entry is NaN and NaN for standard
	/// deviation if data has less than two entries or if any entry is NaN.
	/// </summary>
	/// <param name="samples"> Sample array, no sorting is assumed. </param>
	public static (double Mean, double StandardDeviation) MeanStandardDeviation( double[] samples ) => (Mean( samples ), StandardDeviation( samples ));

	/// <inheritdoc cref="MeanStandardDeviation(double[])" />
	public static (double Mean, double StandardDeviation) MeanStandardDeviation<T>( T[] samples )
		where T : struct, INumberBase<T>
		=> MeanStandardDeviation( samples.AsDoublesArray() );

	/// <summary>
	/// Estimates the arithmetic sample mean and the unbiased population variance from the provided samples as unsorted array. On a dataset of size N
	/// will use an N-1 normalizer (Bessel's correction). Returns NaN for mean if data is empty or any entry is NaN and NaN for variance if data has
	/// less than two entries or if any entry is NaN.
	/// </summary>
	/// <param name="samples"> Sample array, no sorting is assumed. </param>
	public static (double Mean, double Variance) MeanVariance( double[] samples ) => (Mean( samples ), Variance( samples ));

	/// <inheritdoc cref="MeanVariance(double[])" />
	public static (double Mean, double Variance) MeanVariance<T>( T[] samples )
		where T : struct, INumberBase<T>
		=> MeanVariance( samples.AsDoublesArray() );

	/// <summary> Returns the smallest value from the unsorted data array. Returns NaN if data is empty or any entry is NaN. </summary>
	/// <param name="data"> Sample array, no sorting is assumed. </param>
	public static double Minimum( double[] data )
	{
		if ( data.Length == 0 )
		{
			return double.NaN;
		}

		var num = double.PositiveInfinity;
		for ( var i = 0; i < data.Length; i++ )
		{
			if ( data[ i ] < num || double.IsNaN( data[ i ] ) )
			{
				num = data[ i ];
			}
		}

		return num;
	}

	/// <inheritdoc cref="Minimum(double[])" />
	public static double Minimum<T>( T[] data )
		where T : struct, INumberBase<T>
		=> Minimum( data.AsDoublesArray() );

	/// <summary> Returns the smallest absolute value from the unsorted data array. Returns NaN if data is empty or any entry is NaN. </summary>
	/// <param name="data"> Sample array, no sorting is assumed. </param>
	public static double MinimumAbsolute( double[] data )
	{
		if ( data.Length == 0 )
		{
			return double.NaN;
		}

		var num = double.PositiveInfinity;
		for ( var i = 0; i < data.Length; i++ )
		{
			if ( System.Math.Abs( data[ i ] ) < num || double.IsNaN( data[ i ] ) )
			{
				num = System.Math.Abs( data[ i ] );
			}
		}

		return num;
	}

	/// <inheritdoc cref="MinimumAbsolute(double[])" />
	public static double MinimumAbsolute<T>( T[] data )
		where T : struct, INumberBase<T>
		=> MinimumAbsolute( data.AsDoublesArray() );

	/// <summary>
	/// Evaluates the population covariance from the full population provided as two arrays. On a dataset of size N will use an N normalizer and would
	/// thus be biased if applied to a subset. Returns NaN if data is empty or if any entry is NaN.
	/// </summary>
	/// <param name="population1"> First population array. </param>
	/// <param name="population2"> Second population array. </param>
	public static double PopulationCovariance( double[] population1, double[] population2 )
	{
		if ( population1.Length != population2.Length )
		{
			throw new ArgumentException( "All vectors must have the same dimensionality." );
		}

		if ( population1.Length == 0 )
		{
			return double.NaN;
		}

		var num = Mean( population1 );
		var num2 = Mean( population2 );
		var num3 = 0.0;
		for ( var i = 0; i < population1.Length; i++ )
		{
			num3 += ( population1[ i ] - num ) * ( population2[ i ] - num2 );
		}

		return num3 / population1.Length;
	}

	/// <inheritdoc cref="PopulationCovariance(double[], double[])" />
	public static double PopulationCovariance<T, U>( T[] population1, U[] population2 )
		where T : struct, INumberBase<T>
		where U : struct, INumberBase<U>
		=> PopulationCovariance( population1.AsDoublesArray(), population2.AsDoublesArray() );

	/// <summary>
	/// Evaluates the population standard deviation from the full population provided as unsorted array. On a dataset of size N will use an N
	/// normalizer and would thus be biased if applied to a subset. Returns NaN if data is empty or if any entry is NaN.
	/// </summary>
	/// <param name="population"> Sample array, no sorting is assumed. </param>
	public static double PopulationStandardDeviation( double[] population ) => Math.Sqrt( PopulationVariance( population ) );

	/// <inheritdoc cref="PopulationStandardDeviation(double[])" />
	public static double PopulationStandardDeviation<T>( T[] population )
		where T : struct, INumberBase<T>
		=> PopulationStandardDeviation( population.AsDoublesArray() );

	/// <summary>
	/// Evaluates the population variance from the full population provided as unsorted array. On a dataset of size N will use an N normalizer and
	/// would thus be biased if applied to a subset. Returns NaN if data is empty or if any entry is NaN.
	/// </summary>
	/// <param name="population"> Sample array, no sorting is assumed. </param>
	public static double PopulationVariance( double[] population )
	{
		if ( population.Length == 0 )
		{
			return double.NaN;
		}

		var num = 0.0;
		var num2 = population[ 0 ];
		for ( var i = 1; i < population.Length; i++ )
		{
			num2 += population[ i ];
			var num3 = ( ( i + 1 ) * population[ i ] ) - num2;
			num += num3 * num3 / ( ( i + 1.0 ) * i );
		}

		return num / population.Length;
	}

	/// <inheritdoc cref="PopulationVariance(double[])" />
	public static double PopulationVariance<T>( T[] population )
		where T : struct, INumberBase<T>
		=> PopulationVariance( population.AsDoublesArray() );

	/// <summary>
	/// Estimates the root mean square (RMS) also known as quadratic mean from the unsorted data array. Returns NaN if data is empty or any entry is NaN.
	/// </summary>
	/// <param name="data"> Sample array, no sorting is assumed. </param>
	public static double RootMeanSquare( double[] data )
	{
		if ( data.Length == 0 )
		{
			return double.NaN;
		}

		var num = 0.0;
		var num2 = 0uL;
		for ( var i = 0; i < data.Length; i++ )
		{
			num += ( ( data[ i ] * data[ i ] ) - num ) / ++num2;
		}

		return Math.Sqrt( num );
	}

	/// <inheritdoc cref="RootMeanSquare(double[])" />
	public static double RootMeanSquare<T>( T[] data )
		where T : struct, INumberBase<T>
		=> RootMeanSquare( data.AsDoublesArray() );

	/// <summary>
	/// Estimates the unbiased population standard deviation from the provided samples as unsorted array. On a dataset of size N will use an N-1
	/// normalizer (Bessel's correction). Returns NaN if data has less than two entries or if any entry is NaN.
	/// </summary>
	/// <param name="samples"> Sample array, no sorting is assumed. </param>
	public static double StandardDeviation( double[] samples ) => Math.Sqrt( Variance( samples ) );

	/// <inheritdoc cref="StandardDeviation(double[])" />
	public static double StandardDeviation<T>( T[] samples )
		where T : struct, INumberBase<T>
		=> StandardDeviation( samples.AsDoublesArray() );

	/// <summary>
	/// Estimates the unbiased population variance from the provided samples as unsorted array. On a dataset of size N will use an N-1 normalizer
	/// (Bessel's correction). Returns NaN if data has less than two entries or if any entry is NaN.
	/// </summary>
	/// <param name="samples"> Sample array, no sorting is assumed. </param>
	public static double Variance( double[] samples )
	{
		if ( samples.Length <= 1 )
		{
			return double.NaN;
		}

		var num = 0.0;
		var num2 = samples[ 0 ];
		for ( var i = 1; i < samples.Length; i++ )
		{
			num2 += samples[ i ];
			var num3 = ( ( i + 1 ) * samples[ i ] ) - num2;
			num += num3 * num3 / ( ( i + 1.0 ) * i );
		}

		return num / ( samples.Length - 1 );
	}

	/// <inheritdoc cref="Variance(double[])" />
	public static double Variance<T>( T[] samples ) where T : struct, INumberBase<T>
	{
		return Variance( samples.AsDoublesArray() );
	}
}
