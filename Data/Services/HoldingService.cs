using RiskConsult.Core;
using RiskConsult.Data.Entities;
using RiskConsult.Data.Repositories;
using RiskConsult.Enumerators;
using System.Collections.Concurrent;
using System.Reflection;

namespace RiskConsult.Data.Services;

public interface IHoldingService
{
	IReadOnlyDictionary<TypeId, IHoldingType> HoldingTypes { get; }

	IEnumerable<IHoldingTerms> GetAll();

	IHoldingTerms? GetHoldingTerms( string holdingId, HoldingIdType idType );

	IHoldingTerms GetHoldingTerms( int holdingId );

	List<IHoldingTerms> GetHoldingTermsBySubType( SubTypeId subTypeId );
}

internal class HoldingService( ITcHoldingRepository tcHoldingRepository, ITcIntegerRepository tcIntegerRepository ) : IHoldingService
{
	private const int _tcTypeGroup = 0;
	private readonly ConcurrentDictionary<int, IHoldingTerms> _terms = [];
	private readonly object _typesLock = new();
	private bool _allLoaded = false;
	private Dictionary<TypeId, IHoldingType>? _types;
	public IReadOnlyDictionary<TypeId, IHoldingType> HoldingTypes => _types ??= GetHoldingTypes( tcIntegerRepository );

	public IEnumerable<IHoldingTerms> GetAll()
	{
		if ( _allLoaded )
		{
			return _terms.Values;
		}

		_terms.Clear();
		tcHoldingRepository
		   .GetAll<TcHoldingEntity>()
		   .AsParallel()
		   .ForAll( e => CreateAndAddHoldingTerms( e ) );

		_allLoaded = true;
		return _terms.Values;
	}

	public IHoldingTerms GetHoldingTerms( int holdingId )
	{
		if ( _terms.TryGetValue( holdingId, out IHoldingTerms? terms ) )
		{
			return terms;
		}

		ITcHoldingEntity? tycs = tcHoldingRepository.GetTcHoldingEntity( holdingId.ToString(), HoldingIdType.HoldingId );
		return tycs == null ? throw new InvalidOperationException( $"Invalid holding ID {holdingId}" ) : CreateAndAddHoldingTerms( tycs );
	}

	public IHoldingTerms? GetHoldingTerms( string holdingId, HoldingIdType idType )
	{
		if ( idType is HoldingIdType.Invalid )
		{
			return null;
		}

		IHoldingTerms? terms =
			idType is HoldingIdType.HoldingId ? _terms.GetValueOrDefault( Convert.ToInt32( holdingId ) ) :
			idType is HoldingIdType.Description ? _terms.Values.FirstOrDefault( e => e.Description.Equals( holdingId, StringComparison.InvariantCultureIgnoreCase ) ) :
			idType is HoldingIdType.Ticker ? _terms.Values.FirstOrDefault( e => e.Ticker.Equals( holdingId, StringComparison.InvariantCultureIgnoreCase ) ) :
			idType is HoldingIdType.Ticker2 ? _terms.Values.FirstOrDefault( e => e.Ticker2.Equals( holdingId, StringComparison.InvariantCultureIgnoreCase ) ) :
			 _terms.Values.FirstOrDefault( e => e.Isin.Equals( holdingId, StringComparison.InvariantCultureIgnoreCase ) );

		if ( terms != null )
		{
			return terms;
		}

		ITcHoldingEntity? tycs = tcHoldingRepository.GetTcHoldingEntity( holdingId, idType );
		if ( tycs == null )
		{
			return null;
		}

		return CreateAndAddHoldingTerms( tycs );
	}

	public List<IHoldingTerms> GetHoldingTermsBySubType( SubTypeId subTypeId )
	{
		var termsList = new List<IHoldingTerms>();
		ITcHoldingEntity[] entities = tcHoldingRepository.GetTcHoldingEntitiesBySubType( ( int ) subTypeId );
		foreach ( ITcHoldingEntity entity in entities )
		{
			if ( !_terms.TryGetValue( entity.HoldingId, out IHoldingTerms? terms ) )
			{
				terms = CreateAndAddHoldingTerms( entity );
			}

			termsList.Add( terms );
		}

		return termsList;
	}

	/// <summary> Crea términos y agrega a cache a partir de base de datos </summary>
	private IHoldingTerms CreateAndAddHoldingTerms( ITcHoldingEntity tycs )
	{
		var terms = new HoldingTerms
		{
			Description = tycs.Description,
			HoldingId = tycs.HoldingId,
			Ticker = tycs.Ticker,
			Isin = tycs.Isin ?? string.Empty,
			Ticker2 = tycs.Ticker2 ?? string.Empty,
			TypeId = ( TypeId ) tycs.TypeId,
			TermStructureId = tycs.TermStructureId == null ? TermStructureId.Invalid : ( TermStructureId ) tycs.TermStructureId,
			ModuleId = tycs.ModuleId == null ? ModuleId.Invalid : ( ModuleId ) tycs.ModuleId,
			PeriodId = tycs.PeriodId == null ? PeriodId.Invalid : ( PeriodId ) tycs.PeriodId,
			SubTypeId = tycs.SubTypeId == null ? SubTypeId.Invalid : ( SubTypeId ) tycs.SubTypeId,
			ClassId = tycs.ClassId == null ? ClassId.Invalid : ( ClassId ) tycs.ClassId,
			CountryId = tycs.CountryId == null ? CountryId.Invalid : ( CountryId ) tycs.CountryId,
			CurrencyId = tycs.CurrencyId == null ? CurrencyId.Invalid : ( CurrencyId ) tycs.CurrencyId,
			Issue = tycs.Issue == null ? DateTime.MinValue : ( DateTime ) tycs.Issue,
			Maturity = tycs.Maturity == null ? DateTime.MinValue : ( DateTime ) tycs.Maturity,
			CouponRate = tycs.CouponRate == null ? 0.0 : ( double ) tycs.CouponRate,
			Nominal = tycs.Nominal == null ? double.NaN : ( double ) tycs.Nominal,
			Strike = tycs.Strike == null ? double.NaN : ( double ) tycs.Strike,
			UnderlyingId = tycs.UnderlyingId == null ? -1 : ( int ) tycs.UnderlyingId,
			WeekDayAdjust = tycs.WeekDayAdjust == null ? 1 : ( int ) tycs.WeekDayAdjust,
			PayFrequency = tycs.PayFrequency == null ? -1 : ( int ) tycs.PayFrequency,
			LotSize = tycs.LotSize == null ? 1 : ( int ) tycs.LotSize,
			PayDay = tycs.PayDay == null ? -1 : ( int ) tycs.PayDay,
		};

		IHoldingType? type = HoldingTypes.GetValueOrDefault( terms.TypeId );
		if ( type != null )
		{
			terms.TermStructureId = terms.TermStructureId == TermStructureId.Invalid ? type.TermStructure : terms.TermStructureId;
			terms.CurrencyId = terms.CurrencyId == CurrencyId.Invalid ? type.CurrencyId : terms.CurrencyId;
			terms.SubTypeId = terms.SubTypeId == SubTypeId.Invalid ? type.SubType : terms.SubTypeId;
			terms.ModuleId = terms.ModuleId == ModuleId.Invalid ? type.ModuleId : terms.ModuleId;
			terms.PeriodId = terms.PeriodId == PeriodId.Invalid ? type.PayPeriodId : terms.PeriodId;
			terms.ClassId = terms.ClassId == ClassId.Invalid ? type.ClassId : terms.ClassId;
			terms.UnderlyingId = terms.UnderlyingId == -1 ? type.Underlying : terms.UnderlyingId;

			if ( terms.TypeId is TypeId.Udibonos or TypeId.Pagare_UDI or TypeId.Private_UDI or
									TypeId.Fixed_Pagare_UDI or TypeId.Fixed_CEDE_UDI or TypeId.Zero_UDI or TypeId.Futuro_UDI )
			{
				terms.CurrencyId = CurrencyId.UDI;
			}
		}

		return _terms[ tycs.HoldingId ] = terms;
	}

	private Dictionary<TypeId, IHoldingType> GetHoldingTypes( ITcIntegerRepository tcIntegerRepository )
	{
		lock ( _typesLock )
		{
			if ( _types != null )
			{
				return _types;
			}

			var holdingTypes = new Dictionary<TypeId, IHoldingType>();
			ITcIntegerEntity[] typeEntities = tcIntegerRepository.GetTcIntegerGroup( _tcTypeGroup );
			PropertyInfo[] properties = typeof( HoldingType ).GetProperties();

			foreach ( TypeId type in Enum.GetValues<TypeId>() )
			{
				if ( type == TypeId.Invalid )
				{
					continue;
				}

				IEnumerable<ITcIntegerEntity> entities = typeEntities.Where( e => ( TypeId ) e.Id == type );
				var holdType = new HoldingType();

				foreach ( PropertyInfo prop in properties )
				{
					ITcIntegerEntity? value = entities.FirstOrDefault( e => e.Parameter.Equals( prop.Name, StringComparison.InvariantCultureIgnoreCase ) );
					if ( value != null )
					{
						prop.SetValue( holdType, value.Value );
					}
				}

				holdType.TypeId = type;
				holdingTypes.Add( type, holdType );
			}

			_types = holdingTypes;

			return _types;
		}
	}
}
