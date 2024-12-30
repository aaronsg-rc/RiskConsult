using RiskConsult.Core;
using RiskConsult.Data;
using RiskConsult.Enumerators;
using RiskConsult.Interop;
using RiskConsult.Zeus.System;
using Portfolio = RiskConsult.Zeus.System.Portfolio;
using Range = Microsoft.Office.Interop.Excel.Range;

namespace RiskConsult._Tests;

public static class Unclasified
{
	public static void ExportFactorsTabsToRange( string filesDirectory, string portfolio, Range rangeToExport, ZeusExportableTabs tabType, string fileNameModifier = "" )
	{
		try
		{
			// Asigno identificador de archivos
			Console.WriteLine( tabType.ToString() );
			fileNameModifier = $"{( string.IsNullOrEmpty( fileNameModifier ) ? "" : "_" )}{fileNameModifier}.xls";

			// Exporto Curva
			var fileName = $"{portfolio}_{tabType}_Curve{fileNameModifier}";
			var arrTabContent = ExcelExtensions.GetArrayFromWorksheet( Path.Combine( filesDirectory, fileName ), initialColumn: 2 );
			arrTabContent.ExportToRange( ( Range ) rangeToExport.Cells[ 1, 1 ].Offset( ( object ) 0, ( object ) 0 ) );

			// Exporto Spread
			fileName = $"{portfolio}_{tabType}_Spread{fileNameModifier}";
			arrTabContent = ExcelExtensions.GetArrayFromWorksheet( Path.Combine( filesDirectory, fileName ), initialColumn: 2 );
			arrTabContent.ExportToRange( ( Range ) rangeToExport.Cells[ 1, 1 ].Offset( ( object ) 61, ( object ) 0 ) );

			// Exporto Style
			fileName = $"{portfolio}_{tabType}_Style{fileNameModifier}";
			arrTabContent = ExcelExtensions.GetArrayFromWorksheet( Path.Combine( filesDirectory, fileName ), initialColumn: 2 );
			arrTabContent.ExportToRange( ( Range ) rangeToExport.Cells[ 1, 1 ].Offset( ( object ) 71, ( object ) 0 ) );

			// Exporto Industry
			fileName = $"{portfolio}_{tabType}_Industry{fileNameModifier}";
			arrTabContent = ExcelExtensions.GetArrayFromWorksheet( Path.Combine( filesDirectory, fileName ), initialColumn: 2 );
			arrTabContent.ExportToRange( ( Range ) rangeToExport.Cells[ 1, 1 ].Offset( ( object ) 77, ( object ) 0 ) );

			// Exporto Index
			fileName = $"{portfolio}_{tabType}_Index{fileNameModifier}";
			arrTabContent = ExcelExtensions.GetArrayFromWorksheet( Path.Combine( filesDirectory, fileName ), initialColumn: 2 );
			arrTabContent.ExportToRange( ( Range ) rangeToExport.Cells[ 1, 1 ].Offset( ( object ) 89, ( object ) 0 ) );
		}
		catch ( Exception ex )
		{
			throw new Exception( $"ExportFactorsTabsToRange - {ex.Message}" );
		}
	}

	/// <summary> Obtiene la lista de exposiciones del portafolio </summary>
	/// <param name="portfolio"> Portafolio del que se quieren las exposiciones </param>
	/// <param name="factors"> Factores de los que se quiere validar si tiene exposiciones </param>
	public static List<IFactorValue> GetExposures( this Portfolio portfolio, IEnumerable<IFactor> factors )
	{
		var exposures = new List<IFactorValue>();
		foreach ( IFactor factor in factors )
		{
			var exposure = Convert.ToDouble( portfolio.GetPortfolioAnalytic( $"EXP_{factor.Name}" ) );
			if ( exposure != 0 )
			{
				var value = factor.FactorTypeId == FactorTypeId.FT_INDUSTRY ? exposure / 100 : exposure;
				var factExp = new FactorValue() { Factor = factor, Value = value };
				exposures.Add( factExp );
			}
		}

		return exposures;
	}

	/// <summary> Obtiene la lista de exposiciones del portafolio </summary>
	/// <param name="portfolio"> Portafolio del que se quieren las exposiciones </param>
	public static List<IFactorValue> GetExposures( this Portfolio portfolio )
		=> portfolio.GetExposures( DbZeus.Db.Factors );

	/// <summary> Obtiene la lista de exposiciones del instrumento </summary>
	/// ///
	/// <param name="ticker"> Identificador del instrumento del que se quieren las exposiciones </param>
	public static IEnumerable<IFactorValue> GetHoldingExposures( this Portfolio portfolio, string ticker )
		=> GetHoldingExposures( portfolio, ticker, DbZeus.Db.Factors );

	/// <summary> Obtiene la lista de exposiciones del instrumento </summary>
	/// <param name="ticker"> Identificador del instrumento del que se quieren las exposiciones </param>
	/// <param name="factors"> Factores de los que se quiere validar si tiene exposiciones </param>
	public static List<IFactorValue> GetHoldingExposures( this Portfolio portfolio, string ticker, IEnumerable<IFactor> factors )
	{
		var exposures = new List<IFactorValue>();
		foreach ( IFactor factor in factors )
		{
			var exposure = ( double ) ( portfolio.GetHoldingAnalytic( ticker, $"EXP_{factor.Name}" ) ?? 0 );
			if ( exposure != 0 )
			{
				var value = factor.FactorTypeId == FactorTypeId.FT_INDUSTRY ? exposure / 100 : exposure;
				var factExp = new FactorValue() { Factor = factor, Value = value };
				exposures.Add( factExp );
			}
		}

		return exposures;
	}

	public static List<T> StringToList<T>( string listString, char separator = ',' )
	{
		if ( string.IsNullOrEmpty( listString ) )
		{
			return [];
		}

		var list = new List<T>();
		var arrStr = listString.Split( separator );
		foreach ( var word in arrStr )
		{
			var value = ( T ) Convert.ChangeType( word, typeof( T ) );
			list.Add( value );
		}

		return list;
	}
}
