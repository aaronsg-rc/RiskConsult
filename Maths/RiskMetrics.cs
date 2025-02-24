using MathNet.Numerics.Distributions;

namespace RiskConsult.Maths;

public static class RiskMetrics
{
	public static double CalculateParametricVarEWMA( double[,] ewmaCovMatrix, IList<double> weights, double confidence, double specificRisk )
	{
		var nRows = ewmaCovMatrix.GetLength( 0 );
		var nCols = ewmaCovMatrix.GetLength( 1 );

		if ( nRows != nCols )
		{
			throw new ArgumentException( "La matriz de covarianzas debe ser cuadrada.", nameof( ewmaCovMatrix ) );
		}

		// Calcular la varianza del portafolio
		double portfolioVariance = 0;
		for ( var i = 0; i < nCols; i++ )
		{
			for ( var j = 0; j < nCols; j++ )
			{
				portfolioVariance += weights[ i ] * weights[ j ] * ewmaCovMatrix[ i, j ];
			}
		}

		// Calcular el VaR ajustado por EWMA
		var specificVariance = Math.Pow( specificRisk, 2 );
		var portfolioStdDev = Math.Sqrt( portfolioVariance + specificVariance );
		var zScore = Normal.InvCDF( 0, 1, confidence );

		return -zScore * portfolioStdDev;
	}

	/// <summary> Calcula el VaR paramétrico EWMA. </summary>
	/// <param name="returns"> Lista de retornos históricos del portafolio o activos. </param>
	/// <param name="weights"> Pesos de los activos en el portafolio. </param>
	/// <param name="lambda"> Parámetro de suavización EWMA (valor típico: 0.94). </param>
	/// <param name="confidence"> Nivel de confianza para el VaR (por ejemplo, 0.95 o 0.99). </param>
	/// <returns> El valor del VaR paramétrico EWMA. </returns>
	public static double CalculateParametricVarEWMA( double[,] returns, double[] weights, double lambda, double confidence, double specificRisk )
	{
		var n = returns.GetLength( 0 ); // Número de activos

		if ( weights.Length != n )
		{
			throw new ArgumentException( "El número de pesos debe coincidir con el número de activos." );
		}

		// Inicializar la matriz de covarianzas ajustada por EWMA
		var ewmaCovMatrix = returns.GetCovarianceMatrix( lambda );

		return CalculateParametricVarEWMA( ewmaCovMatrix, weights, confidence, specificRisk );
	}
}
