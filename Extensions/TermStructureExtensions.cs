using RiskConsult.Core;
using RiskConsult.Data;
using RiskConsult.Data.Interfaces;
using RiskConsult.Enumerators;

namespace RiskConsult.Extensions;

public static class TermStructureExtensions
{
	public static ITermStructure GetTermStructure( this TermStructureId termStructureId, DateTime date )
	{
		return DbZeus.Db.TermStructures.GetTermStructure( date, termStructureId );
	}

	public static ITermStructure GetTermStructure( this ITermStructureIdProperty termStructureId, DateTime date )
	{
		return DbZeus.Db.TermStructures.GetTermStructure( date, termStructureId.TermStructureId );
	}

	public static TermStructureId GetTermStructureId( this TypeId type )
	{
		return type switch
		{
			TypeId.Cetes => TermStructureId.MXN,
			TypeId.Bondes => TermStructureId.MXN,
			TypeId.Udibonos => TermStructureId.UDI,
			TypeId.Bonos => TermStructureId.MXN,
			TypeId.Zero_EUR => TermStructureId.EUR,
			TypeId.Zero_MXN => TermStructureId.MXN,
			TypeId.Pagare_UDI => TermStructureId.UDI,
			TypeId.Pagare_MXN => TermStructureId.MXN,
			TypeId.Private_UDI => TermStructureId.UDI,
			TypeId.Floater_MXN => TermStructureId.MXN,
			TypeId.Fixed_USD => TermStructureId.LIB,
			TypeId.Floater_SPD_BPA => TermStructureId.TIIE,
			TypeId.Ipabonos => TermStructureId.MXN,
			TypeId.Brems => TermStructureId.MXN,
			TypeId.Fixed_Pagare_UDI => TermStructureId.UDI,
			TypeId.Fixed_CEDE_UDI => TermStructureId.UDI,
			TypeId.Fixed_CEDE_MXN => TermStructureId.MXN,
			TypeId.CD_Dual => TermStructureId.TIIE,
			TypeId.CD_Tiie => TermStructureId.TIIE,
			TypeId.Fixed_MXN => TermStructureId.MXN,
			TypeId.Floater_CEDE_MXN => TermStructureId.MXN,
			TypeId.Floater_USD => TermStructureId.LIB,
			TypeId.Zero_USD => TermStructureId.LIB,
			TypeId.Repo => TermStructureId.MXN,
			TypeId.IpabonosTrimestrales => TermStructureId.MXN,
			TypeId.BondesLp => TermStructureId.MXN,
			TypeId.BondesLt => TermStructureId.MXN,
			TypeId.BondesLs => TermStructureId.MXN,
			TypeId.PapelBancarioAAA => TermStructureId.TIIE,
			TypeId.PapelBancarioP8 => TermStructureId.TIIE,
			TypeId.PapelBancarioP0 => TermStructureId.TIIE,
			TypeId.CETES_CTI => TermStructureId.MXN,
			TypeId.FraTiie28D => TermStructureId.TIIE,
			TypeId.FraCete91D => TermStructureId.MXN,
			TypeId.Zero_UDI => TermStructureId.UDI,
			TypeId.Fixed_USD_48H => TermStructureId.LIB,
			TypeId.Zero_USD48H => TermStructureId.LIB,
			TypeId.Floater_TIIE => TermStructureId.TIIE,
			TypeId.Fixed_TIIE => TermStructureId.TIIE,
			TypeId.Zero_TIIE => TermStructureId.TIIE,
			TypeId.Fixed_UMS => TermStructureId.UMS,
			TypeId.Floater_UMS => TermStructureId.UMS,
			TypeId.Zero_UMS => TermStructureId.UMS,
			TypeId.Futuro_Cetes91 => TermStructureId.MXN,
			TypeId.Futuro_TIIE28 => TermStructureId.TIIE,
			TypeId.Futuro_MXN => TermStructureId.MXN,
			TypeId.Futuro_EUR => TermStructureId.EUR,
			TypeId.Futuro_LIB_30 => TermStructureId.LIB,
			TypeId.Futuro_LIB_91 => TermStructureId.LIB,
			TypeId.Futuro_EUR_YEN => TermStructureId.EUR,
			TypeId.UsTreasury5Y => TermStructureId.TRS,
			TypeId.UsTreasury10Y => TermStructureId.TRS,
			TypeId.UsTreasury30Y => TermStructureId.TRS,
			TypeId.Fixed_EUR => TermStructureId.EUR,
			TypeId.Fixed_YEN => TermStructureId.LIB,
			TypeId.Floater_EUR => TermStructureId.EUR,
			TypeId.UsTreasury2Y => TermStructureId.TRS,
			TypeId.Fixed_AUD => TermStructureId.LIB,
			TypeId.Fixed_BRL => TermStructureId.LIB,
			TypeId.Zero_AUD => TermStructureId.LIB,
			TypeId.Zero_YEN => TermStructureId.LIB,
			TypeId.Fondeo_MXN => TermStructureId.MXN,
			_ => TermStructureId.Invalid
		};
	}
}
