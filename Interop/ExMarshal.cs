using System.Runtime.InteropServices;

namespace RiskConsult.Interop;

public static class ExMarshal
{
	private const string _ole32 = "ole32.dll";
	private const string _oleaut32 = "oleaut32.dll";

	public static object GetActiveObject( string progID )
	{
		Guid clsid;
		try
		{
			CLSIDFromProgIDEx( progID, out clsid );
		}
		// catch
		catch ( Exception )
		{
			CLSIDFromProgID( progID, out clsid );
		}

		GetActiveObject( ref clsid, IntPtr.Zero, out var obj );
		return obj;
	}

	[DllImport( _ole32, PreserveSig = false )]
#pragma warning disable SYSLIB1054 // Use “LibraryImportAttribute” en lugar de “DllImportAttribute” para generar código de serialización P/Invoke en el tiempo de compilación
	private static extern void CLSIDFromProgID( [MarshalAs( UnmanagedType.LPWStr )] string progId, out Guid clsid );

#pragma warning restore SYSLIB1054 // Use “LibraryImportAttribute” en lugar de “DllImportAttribute” para generar código de serialización P/Invoke en el tiempo de compilación

	[DllImport( _ole32, CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false )]
#pragma warning disable SYSLIB1054 // Use “LibraryImportAttribute” en lugar de “DllImportAttribute” para generar código de serialización P/Invoke en el tiempo de compilación
	private static extern void CLSIDFromProgIDEx( [MarshalAs( UnmanagedType.LPWStr )] string progId, out Guid clsid );

#pragma warning restore SYSLIB1054 // Use “LibraryImportAttribute” en lugar de “DllImportAttribute” para generar código de serialización P/Invoke en el tiempo de compilación

	[DllImport( _oleaut32, PreserveSig = false )]
#pragma warning disable SYSLIB1054 // Use “LibraryImportAttribute” en lugar de “DllImportAttribute” para generar código de serialización P/Invoke en el tiempo de compilación
	private static extern void GetActiveObject( ref Guid rclsid, IntPtr reserved, [MarshalAs( UnmanagedType.Interface )] out object ppunk );
#pragma warning restore SYSLIB1054 // Use “LibraryImportAttribute” en lugar de “DllImportAttribute” para generar código de serialización P/Invoke en el tiempo de compilación
}