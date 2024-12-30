using RiskConsult.Data.Interfaces;
using RiskConsult.Enumerators;
using RiskConsult.Extensions;

namespace RiskConsult.Core;

public interface IPortfolio : IDateProperty, ICurrencyIdProperty, IPortfolioIdProperty, IPriceSourceIdProperty, IValueProperty<double>, IIdProperty
{
	List<IHolding> Holdings { get; set; }

	IHolding? this[ int id ] { get; }

	void AddHolding( IHolding holding );

	IPortfolio Clone();
}

/// <summary> Constructor del portafolio que evalua los instrumentos a la fecha en que se abre </summary>
public class Portfolio : IPortfolio
{
	/// <summary> Moneda en que está valuado el portafolio </summary>
	public CurrencyId CurrencyId { get; set; } = CurrencyId.Invalid;
	/// <summary> Fecha del portafolio </summary>
	public DateTime Date { get; set; } = DateTime.MinValue;
	/// <summary> Lista de instrumentos del portafolio </summary>
	public List<IHolding> Holdings { get; set; } = [];
	public int Id { get; set; } = -1;
	/// <summary> Nombre del portafolio </summary>
	public string PortfolioId { get; set; } = string.Empty;
	/// <summary> Tipo de precio con el que se evalua el portafolio </summary>
	public PriceSourceId PriceSourceId { get; set; } = PriceSourceId.Invalid;
	/// <summary> Valuación del portafolio </summary>
	public double Value { get => Holdings.Sum( h => h.Value ); set => SetValue( value ); }

	/// <summary> Devuelve la referencia al primer instrumento que coincida con el identificador </summary>
	/// <param name="stringID"> Identificador del instrumento del tipo Description/Ticker/Ticker2/Isin </param>
	public IHolding? this[ IHoldingIdProperty holdingId ] => this[ holdingId.HoldingId ];

	public IHolding? this[ int holdingId ] => Holdings.FirstOrDefault( h => h.HoldingId == holdingId );

	/// <summary> Agrega un instrumento al portafolio, si ya existe suma el número de títulos </summary>
	/// <param name="holding"> Referencia a instrumento a agregar </param>
	public void AddHolding( IHolding holding )
	{
		IHolding? myHolding = Holdings.FirstOrDefault( h => h.HoldingId == holding.HoldingId );
		if ( myHolding == null )
		{
			Holdings.Add( holding );
		}
		else
		{
			myHolding.Amount += holding.Amount;
		}
	}

	/// <summary> Agrega un instrumento a la composición del portafolio, si ya existe suma el número de títulos </summary>
	/// <param name="holdingId"> Implementación del ID del instrumento </param>
	/// <param name="amount"> Número de títulos </param>
	/// <param name="csZeus"> Cadena de conexión a Zeus </param>
	/// <param name="dbUser"> Cadena de conexión a User </param>
	public void AddHolding( IHoldingIdProperty holdingId, double amount )
		=> AddHolding( holdingId.HoldingId, amount );

	/// <summary> Agrega un instrumento a la composición del portafolio, si ya existe suma el número de títulos </summary>
	/// <param name="holdingId"> ID del instrumento </param>
	/// <param name="amount"> Número de títulos </param>
	public void AddHolding( int holdingId, double amount )
	{
		IHolding? myHolding = Holdings.FirstOrDefault( h => h.HoldingId == holdingId );
		if ( myHolding == null )
		{
			IHoldingTerms terms = holdingId.GetHoldingTerms() ?? throw new Exception( $"Invalid id {holdingId}" );
			var hold = new Holding
			{
				Amount = amount,
				Terms = terms,
				HoldingId = holdingId
			};

			hold.LoadPrice( Date, PriceSourceId, CurrencyId );
			Holdings.Add( hold );
		}
		else
		{
			myHolding.Amount += amount;
		}
	}

	public IPortfolio Clone()
	{
		var portfolio = new Portfolio()
		{
			Date = Date,
			CurrencyId = CurrencyId,
			PriceSourceId = PriceSourceId,
			PortfolioId = PortfolioId,
			Holdings = []
		};

		foreach ( IHolding holding in Holdings )
		{
			portfolio.Holdings.Add( holding.Clone() );
		}

		return portfolio;
	}

	private void SetValue( double value )
	{
		var currValue = Value;
		var dicWeights = Holdings.ToDictionary( h => h.HoldingId, h => h.Value / currValue );
		this.SetAmountsByWeights( dicWeights, value );
	}
}
