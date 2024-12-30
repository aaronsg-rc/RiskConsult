using RiskConsult.Enumerators;

namespace RiskConsult.Zeus.System;

/// <summary> Parámetros necesarios para poder ejecutar un perfat </summary>
public sealed class PerformanceSettings
{
	public string Benchmark { get; set; }
	public string Currency { get; set; }
	public DateTime EndDate { get; set; }
	public ModelId Model { get; set; }
	public string Name { get; set; }
	public string Portfolio { get; set; }
	public DateTime StartDate { get; set; }

	/// <summary> Constructor base </summary>
	public PerformanceSettings()
	{
		Benchmark = "PiPCetes-28d BRUTO";
		Currency = "Mexican Peso";
		Portfolio = string.Empty;
		Name = string.Empty;
		Model = ModelId.Model_07;
		StartDate = DateTime.MinValue;
		EndDate = DateTime.MinValue;
	}

	public PerformanceSettings( string portfolio, DateTime startDate, DateTime endDate ) : this()
	{
		Portfolio = portfolio;
		StartDate = startDate;
		EndDate = endDate;
		Name = GetFixedName( $"{portfolio}_[{Benchmark}]" );
	}

	public PerformanceSettings( string portfolio, DateTime startDate, DateTime endDate, string name )
		: this( portfolio, startDate, endDate )
	{
		Name = GetFixedName( name );
	}

	public PerformanceSettings( string portfolio, DateTime startDate, DateTime endDate, string name, string benchmark )
		: this( portfolio, startDate, endDate, name )
	{
		Benchmark = benchmark;
	}

	public PerformanceSettings( string portfolio, DateTime startDate, DateTime endDate, string name, string benchmark, string currency )
		: this( portfolio, startDate, endDate, name, benchmark )
	{
		Currency = currency;
	}

	public PerformanceSettings( string portfolio, DateTime startDate, DateTime endDate, string name, string benchmark, string currency, ModelId ModelId )
		: this( portfolio, startDate, endDate, name, benchmark, currency )
	{
		Model = ModelId;
	}

	private static string GetFixedName( string name ) => name[ ..Math.Min( 30, name.Length ) ];
}
