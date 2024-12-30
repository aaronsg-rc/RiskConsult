namespace RiskConsult.Zeus.System;

/// <summary> Parámetros necesarios para poder ejecutar un backtesting </summary>
public sealed class BacktestSettings
{
	public string Analytic { get; set; }
	public DateTime EndDate { get; set; }
	public string FilePath { get; set; }
	public string Portfolio { get; set; }
	public DateTime StartDate { get; set; }

	public BacktestSettings()
	{
		StartDate = DateTime.MinValue;
		EndDate = DateTime.MinValue;
		Portfolio = string.Empty;
		Analytic = string.Empty;
		FilePath = string.Empty;
	}

	public BacktestSettings( DateTime startDate, DateTime endDate, string portfolio, string analytic, string filePath )
	{
		StartDate = startDate;
		EndDate = endDate;
		Portfolio = portfolio;
		Analytic = analytic;
		FilePath = filePath;
	}
}