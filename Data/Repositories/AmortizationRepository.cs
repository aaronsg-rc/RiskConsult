using RiskConsult.Data.Entities;
using RiskConsult.Data.Interfaces;
using System.Data;

namespace RiskConsult.Data.Repositories;

public interface IAmortizationRepository : IRepository<IAmortizationEntity>
{
	IAmortizationEntity[] GetAmortizationEntities( int holdingId );
}

/// <summary> Clase encargada de obtener entidades de amortizaciones </summary>
internal class AmortizationRepository( IUnitOfWork unitOfWork ) : DbRepository<IAmortizationEntity>( unitOfWork ), IAmortizationRepository
{
	public override IPropertyMap[] Properties { get; } =
	[
		new PropertyMap<AmortizationEntity>( nameof(AmortizationEntity.HoldingId ), "intID", 0, true ),
		new PropertyMap<AmortizationEntity>( nameof(AmortizationEntity.Date ), "dteDate", 1, true ),
		new PropertyMap<AmortizationEntity>( nameof(AmortizationEntity.Value ), "dblAmortization", 4, false )
	];

	public override string TableName { get; } = "tblDATA_CashFlows";

	/// <summary> Obtiene la lista de entidades de amortización de un instrumento </summary>
	/// <returns> Lista de entidades de amortización </returns>
	public IAmortizationEntity[] GetAmortizationEntities( int holdingId )
	{
		using IDbCommand command = UnitOfWork.CreateCommand();
		command.CommandText = $"SELECT intID, dteDate, dblAmortization FROM {TableName} WHERE intID = {holdingId}";

		return UnitOfWork.GetCommandEntities<AmortizationEntity>( command, Properties );
	}
}
