using System.Runtime.Versioning;

namespace AgeLib.Library;

[SupportedOSPlatform("windows")]
internal static class LibraryLog
{
    private static readonly object SyncRoot = new();
    private static readonly string LogPath = Path.Combine(
        Path.GetDirectoryName(Environment.ProcessPath) ?? Environment.CurrentDirectory,
        "agelib-library.log");

    internal static void Write(string message)
    {
        var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [Library] {message}";
        NativeMethods.OutputDebugString(line + Environment.NewLine);

        try
        {
            lock (SyncRoot)
            {
                File.AppendAllText(LogPath, line + Environment.NewLine);
            }
        }
        catch (Exception exception)
        {
            NativeMethods.OutputDebugString($"[Library] Could not write log file: {exception}\r\n");
        }
    }
}