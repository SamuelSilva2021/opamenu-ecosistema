using System;
using System.Runtime.InteropServices;

namespace OpaMenu.Desktop.Services.Implementation.Printing;

internal static class WindowsRawPrinter
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    private sealed class DOCINFO
    {
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pDocName = "OpaMenu";

        [MarshalAs(UnmanagedType.LPWStr)]
        public string pOutputFile = string.Empty;

        [MarshalAs(UnmanagedType.LPWStr)]
        public string pDataType = "RAW";
    }

    [DllImport("winspool.Drv", EntryPoint = "OpenPrinterW", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool OpenPrinter(string pPrinterName, out IntPtr phPrinter, IntPtr pDefault);

    [DllImport("winspool.Drv", SetLastError = true)]
    private static extern bool ClosePrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", EntryPoint = "StartDocPrinterW", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern int StartDocPrinter(IntPtr hPrinter, int level, [In] DOCINFO di);

    [DllImport("winspool.Drv", SetLastError = true)]
    private static extern bool EndDocPrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", SetLastError = true)]
    private static extern bool StartPagePrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", SetLastError = true)]
    private static extern bool EndPagePrinter(IntPtr hPrinter);

    [DllImport("winspool.Drv", SetLastError = true)]
    private static extern bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, int dwCount, out int dwWritten);

    public static bool SendBytesToPrinter(string printerName, byte[] bytes)
    {
        if (!OpenPrinter(printerName, out var hPrinter, IntPtr.Zero))
            return false;

        try
        {
            var docInfo = new DOCINFO();
            var jobId = StartDocPrinter(hPrinter, 1, docInfo);
            if (jobId <= 0)
                return false;

            try
            {
                if (!StartPagePrinter(hPrinter))
                    return false;

                var unmanagedBytes = Marshal.AllocCoTaskMem(bytes.Length);
                try
                {
                    Marshal.Copy(bytes, 0, unmanagedBytes, bytes.Length);
                    var ok = WritePrinter(hPrinter, unmanagedBytes, bytes.Length, out var written);
                    return ok && written == bytes.Length;
                }
                finally
                {
                    Marshal.FreeCoTaskMem(unmanagedBytes);
                    EndPagePrinter(hPrinter);
                }
            }
            finally
            {
                EndDocPrinter(hPrinter);
            }
        }
        finally
        {
            ClosePrinter(hPrinter);
        }
    }
}

