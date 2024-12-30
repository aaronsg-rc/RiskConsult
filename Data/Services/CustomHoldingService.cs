using Microsoft.Extensions.DependencyInjection;
using RiskConsult.Core;
using RiskConsult.Data.Entities;
using RiskConsult.Data.Repositories;
using RiskConsult.Enumerators;

namespace RiskConsult.Data.Services;

public interface ICustomHoldingService
{
	IEnumerable<IHoldingTerms> GetAll();

	IHoldingTerms GetHoldingTerms( int holdingId );

	IHoldingTerms? GetHoldingTerms( string holdingId, HoldingIdType idType );
}

internal class CustomHoldingService( IServiceProvider serviceProvider ) : ICustomHoldingService
{
	private readonly Dictionary<int, IHoldingTerms> _cache = [];
	private readonly ICustomHoldingRepository<DateTime> _dateRepository = serviceProvider.GetRequiredService<ICustomHoldingRepository<DateTime>>();
	private readonly ICustomHoldingRepository<double> _floatRepository = serviceProvider.GetRequiredService<ICustomHoldingRepository<double>>();
	private readonly ICustomHoldingRepository<int> _integerRepository = serviceProvider.GetRequiredService<ICustomHoldingRepository<int>>();
	private readonly IMapHoldingRepository _mapRepository = serviceProvider.GetRequiredService<IMapHoldingRepository>();

	public IEnumerable<IHoldingTerms> GetAll()
	{
		MapHoldingEntity[] entities = _mapRepository.GetAll<MapHoldingEntity>();
		foreach ( MapHoldingEntity entity in entities )
		{
			IHoldingTerms? terms = GetHoldingTerms( entity.HoldingId );
			if ( terms == null )
			{
				continue;
			}

			yield return terms;
		}
	}

	public IHoldingTerms GetHoldingTerms( int holdingId )
	{
		if ( _cache.TryGetValue( holdingId, out IHoldingTerms? terms ) )
		{
			return terms;
		}

		return CreateAndAddCustomHoldingTerms( holdingId.ToString(), HoldingIdType.HoldingId );
	}

	public IHoldingTerms? GetHoldingTerms( string holdingId, HoldingIdType idType )
	{
		if ( idType is HoldingIdType.Invalid )
		{
			return null;
		}

		IHoldingTerms? terms =
			idType == HoldingIdType.HoldingId ? _cache.GetValueOrDefault( Convert.ToInt32( holdingId ) ) :
			idType == HoldingIdType.Ticker ? _cache.Values.FirstOrDefault( e => e.Ticker.Equals( holdingId, StringComparison.InvariantCultureIgnoreCase ) ) :
			_cache.Values.FirstOrDefault( e => e.Ticker.Equals( holdingId, StringComparison.InvariantCultureIgnoreCase ) );

		if ( terms != null )
		{
			return terms;
		}

		return CreateAndAddCustomHoldingTerms( holdingId, idType );
	}

	/// <summary> Obtiene los términos y condiciones de un instrumento dado un id </summary>
	/// <param name="holdingId"> id del instrumento </param>
	private IHoldingTerms CreateAndAddCustomHoldingTerms( string holdingId, HoldingIdType idType )
	{
		IMapHoldingEntity map = _mapRepository.GetHoldingEntity( holdingId, idType ) ??
			throw new ArgumentException( $"Invalid HoldingId {holdingId}", nameof( holdingId ) );
		ICustomHoldingEntity<int>[] ints = _integerRepository.GetCustomHoldingEntities( map.HoldingId );
		ICustomHoldingEntity<double>[] floats = _floatRepository.GetCustomHoldingEntities( map.HoldingId );
		ICustomHoldingEntity<DateTime>[] dates = _dateRepository.GetCustomHoldingEntities( map.HoldingId );
		var terms = new HoldingTerms
		{
			// Asigno tabla map
			HoldingId = map.HoldingId,
			Description = map.Description,
			Ticker = map.Name,

			// Asigno tabla Integer
			TypeId = ( TypeId ) (
				ints.FirstOrDefault( i => i.Parameter == "Type" )?.Value ??
				floats.First( e => e.Parameter == "Type" ).Value ),

			// Asigno tabla Date
			Issue = dates.FirstOrDefault( d => d.Parameter == "Issue" )?.Value ?? DateTime.MinValue,
			Maturity = dates.FirstOrDefault( d => d.Parameter == "Maturity" )?.Value ?? DateTime.MaxValue,

			// Asigno tabla Float
			ClassId = ( ClassId ) ( floats.FirstOrDefault( f => f.Parameter == "ClassId" )?.Value ?? -1 ),
			CurrencyId = ( CurrencyId ) ( floats.FirstOrDefault( f => f.Parameter == "CurrencyId" )?.Value ?? -1 ),
			ModuleId = ( ModuleId ) ( floats.FirstOrDefault( f => f.Parameter == "ModuleId" )?.Value ?? -1 ),
			PayFrequency = ( int ) ( floats.FirstOrDefault( f => f.Parameter == "PayFreq" )?.Value ?? -1 ),
			PeriodId = ( PeriodId ) ( floats.FirstOrDefault( f => f.Parameter == "PayPeriodId" )?.Value ?? -1 ),
			SubTypeId = ( SubTypeId ) ( floats.FirstOrDefault( f => f.Parameter == "SubType" )?.Value ?? -1 ),
			TermStructureId = ( TermStructureId ) ( floats.FirstOrDefault( f => f.Parameter == "TermStructure" )?.Value ?? -1 ),
			UnderlyingId = ( int ) ( floats.FirstOrDefault( f => f.Parameter == "Underlying" )?.Value ?? -1 ),
			CouponRate = floats.FirstOrDefault( f => f.Parameter == "CouponRate" )?.Value ?? 0,
			LotSize = ( int ) ( floats.FirstOrDefault( f => f.Parameter == "LotSize" )?.Value ?? 1 ),
			Nominal = floats.FirstOrDefault( f => f.Parameter == "Nominal" )?.Value ?? 0,
			Strike = floats.FirstOrDefault( f => f.Parameter == "Strike" )?.Value ?? -1,
			Ticker2 = string.Empty,
			Isin = string.Empty,
			PayDay = -1,
			WeekDayAdjust = 1,
		};

		IHoldingType type = DbZeus.Db.Holdings.HoldingTypes[ terms.TypeId ];
		terms.Nominal = terms.Nominal == 0 ? type.Nominal : terms.Nominal;
		terms.ClassId = terms.ClassId == ClassId.Invalid ? type.ClassId : terms.ClassId;
		terms.CurrencyId = terms.CurrencyId == CurrencyId.Invalid ? type.CurrencyId : terms.CurrencyId;
		terms.ModuleId = terms.ModuleId == ModuleId.Invalid ? type.ModuleId : terms.ModuleId;
		terms.PeriodId = terms.PeriodId == PeriodId.Invalid ? type.PayPeriodId : terms.PeriodId;
		terms.SubTypeId = terms.SubTypeId == SubTypeId.Invalid ? type.SubType : terms.SubTypeId;
		terms.TermStructureId = terms.TermStructureId == TermStructureId.Invalid ? type.TermStructure : terms.TermStructureId;
		terms.UnderlyingId = terms.UnderlyingId == -1 ? type.Underlying : terms.UnderlyingId;
		if ( terms.TypeId is TypeId.Udibonos or TypeId.Pagare_UDI or TypeId.Private_UDI or TypeId.Fixed_Pagare_UDI or
			TypeId.Fixed_CEDE_UDI or TypeId.Zero_UDI or TypeId.Futuro_UDI )
		{
			terms.CurrencyId = CurrencyId.UDI;
		}

		return _cache[ map.HoldingId ] = terms;
	}
}
