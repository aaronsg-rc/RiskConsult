﻿namespace RiskConsult.Performance.Providers;

public interface IDateFinalValueProvider
{
	/// <summary> Valor final en la fecha proporcionada </summary>
	double GetFinalValue( DateTime date );
}