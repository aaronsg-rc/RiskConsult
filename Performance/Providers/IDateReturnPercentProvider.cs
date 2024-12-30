﻿namespace RiskConsult.Performance.Providers;

public interface IDateReturnPercentProvider
{
	/// <summary> Rendimiento porcentual en la fecha proporcionada </summary>
	double GetReturnPercent( DateTime date );
}
