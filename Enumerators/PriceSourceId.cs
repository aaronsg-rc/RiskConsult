namespace RiskConsult.Enumerators;

public enum PriceSourceId
{
	Invalid = -1,
	PiP_MD = 0,
	PiP_24h = 1,
	Yield = 2,
	Spread = 3,
	Volatility = 4,
	User = 100,
	Valmer_MD = 101,
	Valmer_24h = 102,
	Custom = 999
}
