﻿using RiskConsult.Data.Entities;
using RiskConsult.Data.Interfaces;
using System.Data;

namespace RiskConsult.Data.Repositories;

public interface IFactorRepository : IRepository<IFactorEntity>
{
	IFactorEntity[] GetFactorEntities( DateTime date );
}

public interface IFactorReturnRepository : IFactorRepository
{ }

public interface IFactorValueRepository : IFactorRepository
{ }

public interface IFactorCumulativeRepository : IFactorRepository
{ }

internal class FactorRepository( IUnitOfWork unitOfWork, string tableName ) : DbRepository<IFactorEntity>( unitOfWork ), IFactorReturnRepository, IFactorValueRepository, IFactorCumulativeRepository
{
	private readonly Dictionary<DateTime, FactorEntity[]> _cache = [];

	public override IPropertyMap[] Properties { get; } =
	[
		new PropertyMap<FactorEntity>( nameof( FactorEntity.Date ), "dteDate", 0, true ),
		new PropertyMap<FactorEntity>( nameof( FactorEntity.FactorId ), "intID", 1, true ),
		new PropertyMap<FactorEntity>( nameof( FactorEntity.Value ), "dblValue", 2, false )
	];
	public override string TableName { get; } = tableName;

	/// <summary> Limpia datos almacenados en el cache </summary>
	public void ClearCache() => _cache.Clear();

	/// <summary> Obtiene las entidades de los factores para la fecha que se defina </summary>
	/// <param name="date"> Fecha de los factores </param>
	public IFactorEntity[] GetFactorEntities( DateTime date )
	{
		if ( _cache.TryGetValue( date, out FactorEntity[]? entities ) )
		{
			return entities;
		}

		using IDbCommand command = UnitOfWork.CreateCommand();
		command.CommandText = $"SELECT * FROM {TableName} WHERE dteDate = @date";

		IDbDataParameter dateParam = command.CreateParameter();
		dateParam.ParameterName = "@date";
		dateParam.Value = date;
		command.Parameters.Add( dateParam );

		return _cache[ date ] = UnitOfWork.GetCommandEntities<FactorEntity>( command, Properties );
	}
}
