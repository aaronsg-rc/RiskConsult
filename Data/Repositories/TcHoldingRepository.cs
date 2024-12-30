using RiskConsult.Data.Entities;

using RiskConsult.Data.Interfaces;
using RiskConsult.Enumerators;
using System.Data;

namespace RiskConsult.Data.Repositories;

public interface ITcHoldingRepository : IRepository<ITcHoldingEntity>
{
	ITcHoldingEntity[] GetTcHoldingEntitiesBySubType( int subTypeId );

	ITcHoldingEntity? GetTcHoldingEntity( string holdingId, HoldingIdType idType );
}

internal class TcHoldingRepository( IUnitOfWork unitOfWork ) : DbRepository<ITcHoldingEntity>( unitOfWork ), ITcHoldingRepository
{
	public override IPropertyMap[] Properties { get; } =
	[
		new PropertyMap<TcHoldingEntity>(nameof(TcHoldingEntity.HoldingId ), "intholdingId", 0, true ),
		new PropertyMap<TcHoldingEntity>(nameof(TcHoldingEntity.TypeId ), "intTypeId", 2, false ),
		new PropertyMap<TcHoldingEntity>(nameof(TcHoldingEntity.SubTypeId ), "intSubTypeId", 3, false ),
		new PropertyMap<TcHoldingEntity>(nameof(TcHoldingEntity.PayDay ), "intPaymentDay", 7, false ),
		new PropertyMap<TcHoldingEntity>(nameof(TcHoldingEntity.PayFrequency ), "intPaymentFrequency", 8, false ),
		new PropertyMap<TcHoldingEntity>(nameof(TcHoldingEntity.PeriodId ), "intPaymentPeriodId", 9, false ),
		new PropertyMap<TcHoldingEntity>(nameof(TcHoldingEntity.WeekDayAdjust ), "intWkDayAdj", 10, false ),
		new PropertyMap<TcHoldingEntity>(nameof(TcHoldingEntity.Issue ), "dteIssue", 12, false ),
		new PropertyMap<TcHoldingEntity>(nameof(TcHoldingEntity.Maturity ), "dteMaturity", 13, false ),
		new PropertyMap<TcHoldingEntity>(nameof(TcHoldingEntity.CouponRate ), "dblCouponRate", 14, false ),
		new PropertyMap<TcHoldingEntity>(nameof(TcHoldingEntity.Nominal ), "dblNominal", 15, false ),
		new PropertyMap<TcHoldingEntity>(nameof(TcHoldingEntity.ClassId ), "intClassId", 26, false ),
		new PropertyMap<TcHoldingEntity>(nameof(TcHoldingEntity.CountryId ), "intCountryId", 31, false ),
		new PropertyMap<TcHoldingEntity>(nameof(TcHoldingEntity.CurrencyId ), "intCurrencyId", 32, false ),
		new PropertyMap<TcHoldingEntity>(nameof(TcHoldingEntity.LotSize ), "intLotSizeID", 41, false ),
		new PropertyMap<TcHoldingEntity>(nameof(TcHoldingEntity.ModuleId ), "intModuleId", 42, false ),
		new PropertyMap<TcHoldingEntity>(nameof(TcHoldingEntity.TermStructureId ), "intTermStructureId", 50, false ),
		new PropertyMap<TcHoldingEntity>(nameof(TcHoldingEntity.UnderlyingId ), "intUnderlyingID", 52, false ),
		new PropertyMap<TcHoldingEntity>(nameof(TcHoldingEntity.Ticker ), "txtTicker", 56, false ),
		new PropertyMap<TcHoldingEntity>(nameof(TcHoldingEntity.Strike ), "dblStrike", 57, false ),
		new PropertyMap<TcHoldingEntity>(nameof(TcHoldingEntity.Description ), "txtDescription", 58, false ),
		new PropertyMap<TcHoldingEntity>(nameof(TcHoldingEntity.Isin ), "txtISIN", 64, false ),
		new PropertyMap<TcHoldingEntity>(nameof(TcHoldingEntity.Ticker2 ), "txtTicker2", 68, false )
	];
	public override string TableName { get; } = "tblTC_Holdings";

	public ITcHoldingEntity[] GetTcHoldingEntitiesBySubType( int subTypeId )
	{
		using IDbCommand command = UnitOfWork.CreateCommand();
		command.CommandText = $"SELECT {string.Join( ',', Properties.Select( p => p.ColumnName ) )} FROM {TableName} WHERE intSubTypeID = {subTypeId}";

		return UnitOfWork.GetCommandEntities<TcHoldingEntity>( command, Properties );
	}

	public ITcHoldingEntity? GetTcHoldingEntity( string holdingId, HoldingIdType idType )
	{
		if ( idType == HoldingIdType.Invalid )
		{
			return null;
		}

		var fieldName =
			idType is HoldingIdType.HoldingId ? "intHoldingId" :
			idType is HoldingIdType.Description ? "txtDescription" :
			idType == HoldingIdType.Ticker ? "txtTicker" :
			idType == HoldingIdType.Ticker2 ? "txtTicker2" :
			"txtISIN";

		using IDbCommand command = UnitOfWork.CreateCommand();
		command.CommandText = $"SELECT {string.Join( ',', Properties.Select( p => p.ColumnName ) )} FROM {TableName} WHERE {fieldName} = @id";
		IDbDataParameter param = command.CreateParameter();
		param.ParameterName = "@id";
		param.Value = holdingId;
		command.Parameters.Add( param );

		return UnitOfWork.GetCommandEntity<TcHoldingEntity>( command, Properties );
	}
}
