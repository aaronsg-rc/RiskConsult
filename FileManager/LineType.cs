namespace RiskConsult.FileManager;

/// <summary> Tipos de línea que se pueden encontrar dentro del archivo ini </summary>
public enum LineType
{
	Comment,
	Section,
	Param,
	Invalid
}