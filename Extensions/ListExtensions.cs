namespace RiskConsult.Extensions;

public static class ListExtensions
{
	/// <summary> Convierte una lista de <typeparamref name="TNum" /> en doubles, a menos que ya lo sea </summary>
	public static IList<double> AsDoubles<TNum>( this IList<TNum> values ) where TNum : IConvertible => values is IList<double> list ? list : values.Select( v => v.ToDouble( null ) ).ToArray();
}
