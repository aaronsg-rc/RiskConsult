using RiskConsult.Extensions;
using System.Numerics;

namespace RiskConsult.Maths;

public static class ArrayOperations
{
	/// <summary> Aplica el operador de adición (+) a cada elemento de una matriz con los de otra, las matrices deben tener las mismas dimenciones </summary>
	public static T[,] Addition<T>( this T[,] values, T[,] others ) where T : IAdditionOperators<T, T, T>
	{
		ArgumentNullException.ThrowIfNull( values );
		ArgumentNullException.ThrowIfNull( others );

		var rows = values.GetLength( 0 );
		var cols = values.GetLength( 1 );
		if ( rows != others.GetLength( 0 ) || cols != others.GetLength( 1 ) )
		{
			throw new Exception( "Arrays must have same dimensions size" );
		}

		// Realizo suma
		var results = new T[ rows, cols ];
		for ( var i = 0; i < rows; i++ )
		{
			for ( var j = 0; j < cols; j++ )
			{
				results[ i, j ] = values[ i, j ] + others[ i, j ];
			}
		}

		return results;
	}

	/// <summary> Convierte una array 2D de <typeparamref name="T" /> a double, a menos que ya lo sea </summary>
	public static double[,] AsDoubles<T>( this T[,] values ) where T : struct
	{
		if ( values is double[,] results )
		{
			return results;
		}

		var rows = values.GetLength( 0 );
		var cols = values.GetLength( 1 );
		var lbRows = values.GetLowerBound( 0 );
		var lbCols = values.GetLowerBound( 1 );
		results = new double[ rows, cols ];

		for ( var i = 0; i < rows; i++ )
		{
			for ( var j = 0; j < cols; j++ )
			{
				results[ i, j ] = Convert.ToDouble( values[ i + lbRows, j + lbCols ] );
			}
		}

		return results;
	}

	/// <summary> Computes the Pearson Product-Moment Correlation matrix. </summary>
	/// <param name="values"> Array of sample data vectors. </param>
	/// <returns> The Pearson product-moment correlation matrix. </returns>
	public static double[,] GetCorrelationMatrix( this double[,] values )
	{
		// Defino dimensiones de mi matriz de correlación
		var size = values.GetLength( 1 );
		var results = new double[ size, size ];

		// Recorro cada posicion de mi matriz destino y obtengo mi coeficiente de correlación entre vector i contra vector j
		for ( var i = 0; i < size; i++ )
		{
			for ( var j = i; j < size; j++ )
			{
				var correlation = i == j ? 1 : Statistics.Correlation( values.GetColumn( i ), values.GetColumn( j ) );
				results[ i, j ] = correlation;
				results[ j, i ] = correlation;
			}
		}

		return results;
	}

	/// <summary> Obtengo la matriz de covarianzas para los valores contenidos en la matriz </summary>
	/// <param name="values"> Matrix de la cual se quiere extraer la matriz de covarianzas </param>
	public static double[,] GetCovarianceMatrix( this double[,] values )
	{
		// Defino dimensiones de mi matriz de covarianzas
		var size = values.GetLength( 1 );
		var results = new double[ size, size ];

		// Recorro cada posicion de mi matriz destino
		for ( var i = 0; i < size; i++ )
		{
			for ( var j = i; j < size; j++ )
			{
				var covariance = Statistics.Covariance( values.GetColumn( i ), values.GetColumn( j ) );
				results[ i, j ] = covariance;
				results[ j, i ] = covariance;
			}
		}

		return results;
	}

	/// <summary> Obtengo la varianza de mi matriz dado un vector de ponderaciones/exposiciones, la matriz debe ser una matriz de rendimientos </summary>
	/// <param name="mReturns"> Matriz de rendimientos </param>
	/// <param name="vWeights"> Vector de ponderaciones/exposiciones </param>
	public static double GetVariance( this double[,] mReturns, IEnumerable<double> vWeights )
	{
		// Obtengo como matriz mi vector de ponderaciones y su transpuesta
		var mWeights = vWeights.ToColumnArray();
		var mTransposedWeights = vWeights.ToRowArray();

		// Obtengo matriz de Covarianzas
		var mCov = mReturns.GetCovarianceMatrix();

		// Realizo la operación
		var matrixResult = mTransposedWeights
			.Multiply( mCov )
			.Multiply( mWeights );

		return matrixResult[ 0, 0 ];
	}

	/// <summary> Realiza operación aritmética de una multiplicación de una matriz por otra matriz </summary>
	public static double[,] Multiply( this double[,] matrixA, double[,] matrixB )
	{
		var rowsA = matrixA.GetLength( 0 );
		var colsA = matrixA.GetLength( 1 );
		var rowsB = matrixB.GetLength( 0 );
		var colsB = matrixB.GetLength( 1 );

		if ( colsA != rowsB )
		{
			throw new ArgumentException( "Matrices cannot be multiplied. Columns of matrix A must equal rows of matrix B." );
		}

		var result = new double[ rowsA, colsB ];
		for ( var i = 0; i < rowsA; i++ )
		{
			for ( var j = 0; j < colsB; j++ )
			{
				double sum = default;
				for ( var k = 0; k < colsA; k++ )
				{
					sum += matrixA[ i, k ] * matrixB[ k, j ];
				}

				result[ i, j ] = sum;
			}
		}

		return result;
	}

	/// <summary> Realiza operación aritmética de una multiplicación de una matriz por un escalar </summary>
	public static T[,] Multiply<T>( this T[,] values, T scalar ) where T : IMultiplyOperators<T, T, T>
	{
		var rows = values.GetLength( 0 );
		var cols = values.GetLength( 1 );
		var results = new T[ rows, cols ];
		for ( var i = 0; i < rows; i++ )
		{
			for ( var j = 0; j < cols; j++ )
			{
				results[ i, j ] = values[ i, j ] * scalar;
			}
		}

		return results;
	}
}
