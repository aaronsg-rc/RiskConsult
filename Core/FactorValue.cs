using RiskConsult.Data.Interfaces;

namespace RiskConsult.Core;

/// <summary> Permite almacenar el factor de riesgo y al valor que le corresponde </summary>
public interface IFactorValue : IFactorProperty, IValueProperty<double>
{
}

/// <summary> Clase que permite almacenar el factor y la exposición al factor de riesgo </summary>
/// <remarks> Constructor a partir del factor y valor </remarks>
/// <param name="factor"> Factor al que se tiene exposición </param>
/// <param name="value"> Valor de la exposición </param>
public class FactorValue : IFactorValue
{
	public IFactor Factor { get; set; } = new Factor();
	public double Value { get; set; }

	/// <summary> Cadena que representa el valor y la descripción del factor </summary>
	/// <returns> </returns>
	public override string ToString() => $"{Value:F6} [{Factor.Description}]";
}
