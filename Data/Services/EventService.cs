using RiskConsult.Data.Entities;
using RiskConsult.Data.Interfaces;
using RiskConsult.Data.Repositories;
using RiskConsult.Enumerators;
using RiskConsult.Extensions;
using System.Data;

namespace RiskConsult.Data.Services;

public interface IEventService : ICachedService
{
	Dictionary<EventId, double> GetHoldingEvents( int holdingId, DateTime date );

	double GetPayoutByEvents( int holding, DateTime date, double price, CurrencyId holdingCurrency );
}

internal class EventService( IHoldingEventRepository eventsRepository ) : IEventService
{
	private readonly Dictionary<int, IHoldingEventEntity[]> _cache = [];

	/// <summary> Limpia datos almacenados en el cache </summary>
	public void ClearCache() => _cache.Clear();

	/// <summary> Obtiene las entidades de pago de derechos para un instrumento en un día </summary>
	/// <param name="holdingId"> ID del instrumento </param>
	/// <param name="date"> Fecha de las entidades </param>
	/// <returns> Lista de las entidades encontradas </returns>
	public Dictionary<EventId, double> GetHoldingEvents( int holdingId, DateTime date )
	{
		return GetEntities( holdingId )
			.Where( entity => entity.Date.Equals( date ) )
			.ToDictionary( entity => entity.EventId, entity => entity.Value );
	}

	/// <summary> Obtiene el monto por concepto de eventos que se paga para un instrumento en una fecha </summary>
	/// <param name="events"> Enumerable de eventos de un instrumento para una fecha </param>
	/// <param name="price"> Precio del instrumento en la fecha </param>
	/// <param name="fxValue"> Tipo de cambio en el que se encuentra el precio respecto a la moneda del instrumento </param>
	/// <returns> Monto por concepto de eventos o 0 si no se encuentra nada </returns>
	public double GetPayoutByEvents( int holdingId, DateTime date, double price, CurrencyId holdingCurrency )
	{
		double payout = 0;
		IEnumerable<IHoldingEventEntity> entities = GetEntities( holdingId ).Where( e => e.Date == date );
		foreach ( IHoldingEventEntity entity in entities )
		{
			payout += GetEventValue( entity, price, holdingCurrency );
		}

		return payout;
	}

	/// <summary> Obtiene el valor del evento proporcional al precio considerando el tipo de cambio </summary>
	/// <param name="holdingEvent"> Entidad de evento </param>
	/// <param name="price"> Precio base del instrumento para la fecha del evento sin considerar tipo de cambio </param>
	/// <param name="fxValue"> Tipo de cambio </param>
	private static double GetEventValue( IHoldingEventEntity holdingEvent, double price, CurrencyId holdingCurrency )
	{
		EventId eventType = holdingEvent.EventId;
		if ( eventType is EventId.Dividend or EventId.Refund )
		{
			// NOTA: Lo correcto es que el valor se almacene en base de datos en la moneda base del instrumento y este sea devuelto de forma directa,
			// sin embargo, se almacena el valor en pesos y es devuelto aplicando una conversion monetaria.
			var fxValue = holdingCurrency.ConvertToCurrency( CurrencyId.MXN, holdingEvent.Date );
			return holdingEvent.Value / fxValue;
		}
		else if ( eventType is EventId.Split or EventId.Escision )
		{
			return ( price * holdingEvent.Value ) - price;
		}
		else if ( eventType is EventId.Merge )
		{
			return ( price / holdingEvent.Value ) - price;
		}
		else if ( eventType is EventId.Suscription or EventId.Dividend2 )
		{
			return price * holdingEvent.Value;
		}

		throw new NotImplementedException( $"Not defined event type: {eventType}" );
	}

	private IHoldingEventEntity[] GetEntities( int holdingId )
	{
		if ( _cache.TryGetValue( holdingId, out IHoldingEventEntity[]? entities ) )
		{
			return entities;
		}

		return _cache[ holdingId ] = eventsRepository.GetHoldingEventEntities( holdingId );
	}
}
