namespace RiskConsult._Tests.Performance;

public interface IReturnData
{
	/// <summary> Fecha final del rendimiento </summary>
	DateTime FinalDate { get; }

	/// <summary> Valor final del rendimiento </summary>
	double FinalValue { get; }

	/// <summary> Fecha inicial del rendimiento </summary>
	DateTime InitialDate { get; }

	/// <summary> Valor inicial del rendimiento </summary>
	double InitialValue { get; }

	/// <summary> Rendimiento porcentual entre el valor inicial y el final </summary>
	double Return { get; }
}
