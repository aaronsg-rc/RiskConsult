﻿namespace RiskConsult.Performance.Providers;

public interface IDateReturnValueProvider
{
	/// <summary> Rendimiento en la unidad base en la fecha proporcionada </summary>
	double GetReturnValue( DateTime date );
}
