using RiskConsult.Extensions;
using RiskConsult.Maths;
using System.Globalization;

namespace RiskConsult._Tests;

public class HistoricalSimulation : List<HistoricalSimulationZeusRow>
{
	private static readonly string[] _wrongScenarios = [ "Name", "20051018_0", "20051116_0", "20130927_0" ];

	public DateTime EndDate => this.Max( v => v.Date );
	public DateTime StartDate => this.Min( v => v.Date );

	public HistoricalSimulation() : base()
	{
	}

	public void Add( DateTime date, double startValue, double endValue, int index )
		=> Add( new HistoricalSimulationZeusRow( date, startValue, endValue, index ) );

	public object[,] GetCompoundedReturns( DateTime dteStart = default, DateTime dteEnd = default, int period = 1 )
		=> GetCompoundedReturns( dteStart, dteEnd, [ period ] );

	public object[,] GetCompoundedReturns( DateTime dteStart = default, DateTime dteEnd = default, int[]? periods = null )
	{
		// Valido parametros opcionales
		periods ??= [ 1 ];
		if ( periods.Length == 0 )
		{
			throw new Exception( "Any period setted." );
		}

		if ( dteStart == default )
		{
			dteStart = StartDate;
		}

		if ( dteEnd == default )
		{
			dteEnd = EndDate;
		}

		// Filtro rango de datos de interés y construyo matriz resultado
		HistoricalSimulationZeusRow[] scenarios =
		[
			.. this
						.Where( v => v.Date >= dteStart && v.Date <= dteEnd )
						.OrderBy( v => v.Date )
,
		];

		// Recorro cada escenario de la matriz original desde los índices que encontré
		var mRes = new object[ scenarios.Length, periods.Length + 2 ];
		var ixRes = 0;
		foreach ( HistoricalSimulationZeusRow scenario in scenarios )
		{
			// Asigno valores por periodo
			double product = 1;
			var scenariosCount = 1;
			var ixPeriod = 2;
			foreach ( var periodSize in periods )
			{
				// Si el tamaño del periodo excede el número de escenarios lo omito
				if ( ixRes + 1 < periodSize )
				{
					break;
				}

				for ( var days = scenariosCount; days <= periodSize; days++ )
				{
					var ixCompounded = ixRes - days + 1;
					product *= scenarios[ ixCompounded ].EndValue / scenarios[ ixCompounded ].StartValue;
				}

				scenariosCount = periodSize + 1;
				mRes[ ixRes, ixPeriod++ ] = product - 1;
			}

			//Asigno valores fijos
			mRes[ ixRes, 0 ] = ixRes + 1;
			mRes[ ixRes++, 1 ] = scenario.Date;
		}

		return mRes;
	}

	public double GetCVaR( double confidence, int compoundedPeriod = 1, int latestData = -1 )
	{
		if ( confidence is < 0 or > 1 )
		{
			throw new ArgumentOutOfRangeException( nameof( confidence ) );
		}

		if ( latestData == -1 || latestData > Count )
		{
			latestData = Count;
		}

		// Obtengo sección de datos
		var mData = GetCompoundedReturns( default, default, [ compoundedPeriod ] );
		mData = mData.GetSection( Math.Max( compoundedPeriod - 1, Count - latestData ), mData.GetLength( 0 ), 0, mData.GetLength( 1 ) );
		var intAlpha = Convert.ToInt32( mData.GetLength( 0 ) * ( 1.0 - confidence ) );
		double sumatory = 0;
		var returns = mData.GetColumn( 2 )
			.Select( Convert.ToDouble )
			.OrderBy( v => v )
			.ToArray();

		// Obtengo CVaR que corresponde al nivel de confianza
		for ( var i = 0; i <= intAlpha - 1; i++ )
		{
			sumatory += returns[ i ];
		}

		return sumatory / intAlpha;
	}

	public double GetVaR( double confidence, int compoundedPeriod = 1, int latestData = -1 )
	{
		if ( confidence is < 0 or > 1 )
		{
			throw new ArgumentOutOfRangeException( nameof( confidence ) );
		}

		if ( latestData == -1 || latestData > Count )
		{
			latestData = Count;
		}

		// Obtengo matriz de rendimientos
		var mData = GetCompoundedReturns( default, default, compoundedPeriod );
		mData = mData.GetSection( Math.Max( compoundedPeriod - 1, Count - latestData ), mData.GetLength( 0 ), 0, mData.GetLength( 1 ) );

		// Devuelvo resultado
		return mData.GetColumn( 2 )
			.Select( Convert.ToDouble )
			.OrderBy( v => v )
			.ToArray()
			.Percentile( 1 - confidence );
	}

	public Dictionary<DateTime, double> GetWorstScenarios( int compoundedPeriod = 1, int rollWindow = 66, int worstDates = 10 )
	{
		// Obtengo matriz de rendimientos compuestos
		var mData = GetCompoundedReturns( default, default, compoundedPeriod );

		// Obtengo diccionario ordenada de fechas y rendimientos
		DateTime[] dates = mData.GetColumn( 1 ).GetRange( compoundedPeriod - 1 ).Cast<DateTime>().ToArray();
		var returns = mData.GetColumn( 2 ).GetRange( compoundedPeriod - 1 ).Cast<double>().ToArray();
		var mSort = dates.Zip( returns, ( k, v ) => new { k, v } )
			.ToDictionary( x => x.k, x => x.v )
			.OrderBy( x => x.Value )
			.ToDictionary( x => x.Key, x => x.Value );

		// Defino diccionario resultado
		var mRes = new Dictionary<DateTime, double>();
		for ( var i = 0; i < worstDates; i++ )
		{
			// Agrego el primer valor
			KeyValuePair<DateTime, double> firstPair = mSort.First();
			mRes.Add( firstPair.Key, firstPair.Value );

			// Obtengo fechas mínimas y máximas del rollwindow
			var ixDate = Array.IndexOf( dates, firstPair.Key );
			DateTime dteMin = dates[ Math.Max( 0, ixDate - rollWindow + 1 ) ];
			DateTime dteMax = dates[ Math.Min( dates.Length - 1, ixDate + rollWindow - 1 ) ];

			// Filtro
			mSort = mSort
				.Where( p => p.Key < dteMin || p.Key > dteMax )
				.ToDictionary( x => x.Key, x => x.Value );
		}

		return mRes;
	}

	public object[,] GetWorstScenariosMatrix( int compoundedPeriod = 1, int rollWindow = 66, int worstDates = 10 )
	{
		// Obtengo matriz de rendimientos compuestos
		var mData = GetCompoundedReturns( default, default, compoundedPeriod );

		// Obtengo matriz ordenada de fechas y rendimientos
		var mSort = mData.GetColumns( [ 1, 4 ] ).ToArrayFromColumns().GetSection( compoundedPeriod - 1, mData.GetLength( 0 ), 0, mData.GetLength( 1 ) );

		// Defino matriz resultado
		var mRes = new object[ worstDates, 2 ];
		for ( int intRes = 0, loopTo = mRes.GetLength( 0 ) - 1; intRes <= loopTo; intRes++ )
		{
			// Asigno valores mínimos
			var dteSelected = DateTime.Parse( mSort[ 0, 0 ].ToString() ?? string.Empty );
			mRes[ intRes, 0 ] = dteSelected;
			mRes[ intRes, 1 ] = Math.Abs( ( double ) mSort[ 0, 1 ] );

			// Obtengo fechas mínimas y máximas
			var intSelectedDate = mData.GetColumn( 1 ).ToList().IndexOf( dteSelected.ToString() );
			var dteMin = DateTime.Parse( mData[ Math.Max( 0, intSelectedDate - rollWindow + 1 ), 1 ].ToString() ?? string.Empty );
			var dteMax = DateTime.Parse( mData[ Math.Min( mData.GetLength( 0 ) - 1, intSelectedDate + rollWindow - 1 ), 1 ].ToString() ?? string.Empty );

			// Genero matriz temporal de transición
			var rows = new List<List<object>>();
			for ( int intSort = 0, loopTo1 = mSort.GetLength( 0 ) - 1; intSort <= loopTo1; intSort++ )
			{
				_ = DateTime.TryParse( mSort[ intSort, 0 ].ToString(), out DateTime dteCurrent );
				if ( dteCurrent < dteMin || dteCurrent > dteMax )
				{
					rows.Add(
					[
						dteCurrent,
						(double) mSort[ intSort, 1 ]
					] );
				}
			}

			mSort = rows.ToArrayFromRows();
		}

		return mRes;
	}

	public void LoadFromClipboard()
	{
		var values = Utilities.Utilities.GetArrayFromClipboard( Environment.NewLine, "\t" ) ?? throw new Exception( "Any valid array on clipboard" );
		var index = 1;
		var rows = values.GetLength( 0 );
		for ( var i = 0; i < rows; i++ )
		{
			if ( _wrongScenarios.Contains( values[ i, 1 ] ) )
			{
				continue;
			}

			var dateValue = values[ i, 1 ][ ..8 ];
			var date = DateTime.ParseExact( dateValue, "yyyyMMdd", CultureInfo.InvariantCulture );
			var startValue = Convert.ToDouble( values[ i, 2 ] );
			var finalValue = Convert.ToDouble( values[ i, 3 ] );
			Add( date, startValue, finalValue, index++ );
		}
	}
}
