using System;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Threading.Tasks;
using Microsoft.Win32;

//some extension methods
namespace Common
{
	public static class Extension
	{
		public static string GetRegString(this string key)
		{
			try
			{
				var regKey = Registry.LocalMachine.OpenSubKey("Software").OpenSubKey("CandyKingdom");
				return regKey?.GetValue(key) != null ? regKey.GetValue(key).ToString() : null;
			}
			catch (NullReferenceException ex)
			{
				(ex.Message + ex.InnerException).WriteLog(EventLogEntryType.Error, 21);
				return null;
			}
			catch (UnauthorizedAccessException ex)
			{
				(ex.Message + ex.InnerException).WriteLog(EventLogEntryType.Error, 21);
				return null;
			}
		}
		public static void SetRegString(this string value, string key)
		{
			try
			{
				var regKey = Registry.LocalMachine.OpenSubKey("Software").OpenSubKey("CandyKingdom", true);
				regKey?.SetValue(key, value);
			}
			catch (NullReferenceException ex)
			{
				(ex.Message + ex.InnerException).WriteLog(EventLogEntryType.Error, 21);
			}
			catch (UnauthorizedAccessException ex)
			{
				(ex.Message + ex.InnerException).WriteLog(EventLogEntryType.Error, 21);
			}
		}
		public static async void WriteLog(this string log, EventLogEntryType type, int id)
		{
			const string logPath = "C:\\ProgramData\\SrvMon\\";
			const string logFile = "Error.log";
			await Task.Run(() =>
			{
				try
				{
					using (var jakeEventLog = new EventLog())
					{
						jakeEventLog.Source = "ServerMonitor";
						jakeEventLog.Log = "SrvMonLog";
						jakeEventLog.WriteEntry(log, type, id);
					}
				}
				catch (SecurityException ex)
				{
					using (var outFile = new StreamWriter(Path.Combine(logPath, logFile), true))
					{
						outFile.WriteLineAsync(ex.Message+ex.InnerException);
					}
					EventLog.CreateEventSource("ServerMonitor", "SrvMonLog");
				}
				catch (ObjectDisposedException ex)
				{
					using (var outFile = new StreamWriter(Path.Combine(logPath, logFile), true))
					{
						outFile.WriteLineAsync(ex.Message+ex.InnerException);
					}
				}
			});
		}
	}
}
