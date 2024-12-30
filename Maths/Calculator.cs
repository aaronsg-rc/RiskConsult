using RiskConsult.Extensions;
using System.Numerics;

namespace RiskConsult.Maths;

public static class Calculator
{
	public static double CalculateBeta( double[] indexReturns, double[] stockReturns )
	{
		var indexVariance = indexReturns.Variance();
		var covariance = indexReturns.Covariance( stockReturns );
		var beta = covariance / indexVariance;

		return beta;
	}

	/// <summary> Obtiene el valor de la interpolación lineal de 2 sets de datos </summary>
	/// <param name="x_values"> Valor X del que se quiere obtener la Y </param>
	/// <param name="x_values"> Set se valores X [Array,Vector,Matrix] </param>
	/// <param name="y_values"> Set se valores Y [Array,Vector,Matrix] </param>
	/// <returns> Valor Y interpolado linealmente </returns>
	public static double LinearInterpolation<TNum1, TNum2, TNum3>( TNum1 x, IEnumerable<TNum2> x_values, IEnumerable<TNum3> y_values )
		where TNum1 : struct, INumberBase<TNum1>
		where TNum2 : struct, INumberBase<TNum2>
		where TNum3 : struct, INumberBase<TNum3>
		=> LinearInterpolation( Convert.ToDouble( x ), x_values.AsDoublesArray(), y_values.AsDoublesArray() );

	/// <summary> Obtiene el valor de la interpolación lineal de 2 sets de datos </summary>
	/// <param name="x"> Valor X del que se quiere obtener la Y </param>
	/// <param name="x_values"> Set se valores X [Array,Vector,Matrix] </param>
	/// <param name="y_values"> Set se valores Y [Array,Vector,Matrix] </param>
	/// <returns> Valor Y interpolado linealmente </returns>
	public static double LinearInterpolation( double x, IList<double> x_values, IList<double> y_values )
	{
		if ( x_values.Count != y_values.Count )
		{
			throw new ArgumentException( "Los enumerables xValues y yValues deben tener la misma longitud." );
		}

		// Interpolación
		for ( var i = 0; i < x_values.Count - 1; i++ )
		{
			if ( x == x_values[ i ] )
			{
				return y_values[ i ];
			}
			else if ( x >= x_values[ i ] && x <= x_values[ i + 1 ] )
			{
				return LinearInterpolation( x, x_values, y_values, i, i + 1 );
			}
		}

		// Extrapolación
		return x < x_values[ 0 ]
			? LinearInterpolation( x, x_values, y_values, 0, 1 )
			: LinearInterpolation( x, x_values, y_values, x_values.Count - 2, x_values.Count - 1 );
	}

	private static double LinearInterpolation( double x, IList<double> xList, IList<double> yList, int ixLB, int ixUB )
		=> yList[ ixLB ] + ( ( x - xList[ ixLB ] ) * ( yList[ ixUB ] - yList[ ixLB ] ) / ( xList[ ixUB ] - xList[ ixLB ] ) );
}
