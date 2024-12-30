namespace RiskConsult.Zeus.Files;

public readonly struct CurvesActiveData
{
	public ActiveData EUR { get; }
	public ActiveData LIB { get; }
	public ActiveData MXN { get; }
	public ActiveData SpreadBAA { get; }
	public ActiveData SpreadBDE_LP { get; }
	public ActiveData SpreadBDE_LT { get; }
	public ActiveData SpreadBP8 { get; }
	public ActiveData SpreadBPA_BP { get; }
	public ActiveData SpreadBPL { get; }
	public ActiveData SpreadBPS_BP { get; }
	public ActiveData SpreadBPT_BP { get; }
	public ActiveData SpreadCET_CTI { get; }
	public ActiveData SpreadXA_XA { get; }
	public ActiveData TIIE { get; }
	public ActiveData TRS { get; }
	public ActiveData UDI { get; }
	public ActiveData UMS { get; }

	internal CurvesActiveData( object[,] arr, int ixRow, int ixCol )
	{
		MXN = new ActiveData( arr, ixRow++, ixCol );
		SpreadBP8 = new ActiveData( arr, ixRow++, ixCol );
		SpreadBPL = new ActiveData( arr, ixRow++, ixCol );
		SpreadBAA = new ActiveData( arr, ixRow++, ixCol );
		UDI = new ActiveData( arr, ixRow++, ixCol );
		LIB = new ActiveData( arr, ixRow++, ixCol );
		TRS = new ActiveData( arr, ixRow++, ixCol );
		SpreadBDE_LP = new ActiveData( arr, ixRow++, ixCol );
		SpreadCET_CTI = new ActiveData( arr, ixRow++, ixCol );
		SpreadBPS_BP = new ActiveData( arr, ixRow++, ixCol );
		SpreadBDE_LT = new ActiveData( arr, ixRow++, ixCol );
		SpreadBPA_BP = new ActiveData( arr, ixRow++, ixCol );
		SpreadBPT_BP = new ActiveData( arr, ixRow++, ixCol );
		SpreadXA_XA = new ActiveData( arr, ixRow++, ixCol );
		TIIE = new ActiveData( arr, ixRow++, ixCol );
		UMS = new ActiveData( arr, ixRow++, ixCol );
		EUR = new ActiveData( arr, ixRow++, ixCol );
	}
}