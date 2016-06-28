using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading.Tasks;
using Common;
using Microsoft.Win32;
//monitor top hardware consuming processes
namespace Jake.Service
{
	public class ProcessMonitor:AbstractModule
	{
		private const string RegPath = "HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\PerfProc\\Performance";
		private const string RegKey = "Disable Performance Counters";
		private bool _completed = true;
		public ProcessMonitor(SrvMonSummary summary):base(summary)
		{
		}
		private void OnTimer()
		{
			if (!_completed) return;
			_completed = false;
			try
			{
				if ((int)Registry.GetValue(RegPath, RegKey, 1) != 0)
				{
					Registry.SetValue(RegPath, RegKey, 0);
				}
				using (var searcher = new ManagementObjectSearcher("root\\CIMV2",
						"SELECT * FROM Win32_PerfFormattedData_PerfProc_Process"))
				{
					var molist = searcher.Get().Cast<ManagementBaseObject>().ToList();
					var topRamProc = (from mo in molist
									  where (mo["Name"].ToString() != "_Total")
									  orderby Convert.ToUInt64(mo["WorkingSet"]) descending
									  select new Proc
									  {
										  ProcName = mo["Name"].ToString(),
										  Pid = Convert.ToInt32(mo["IDProcess"]),
										  ProcMemory = (int?)(Convert.ToUInt64(mo["WorkingSet"]) / (1024 * 1024)),
										  TimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
									  }).Take(_summary.Config.TopRamProcesses).ToArray();
					if (!topRamProc.Any())
					{
						"Top RAM processes empty!".WriteLog(EventLogEntryType.Error, 12);
					}
					else
					{
						_summary.TopRamProcesses = topRamProc;
					}
				}
				using (var searcher = new ManagementObjectSearcher("root\\CIMV2",
						"SELECT * FROM Win32_PerfFormattedData_PerfProc_Process"))
				{
					var molist = searcher.Get().Cast<ManagementBaseObject>().ToList();
					var topCpuProc = (from mo in molist
									  where ((mo["Name"].ToString() != "Idle") && (mo["Name"].ToString() != "_Total"))
									  orderby Convert.ToInt32(mo["PercentProcessorTime"]) descending
									  select new Proc
									  {
										  ProcName = mo["Name"].ToString(),
										  Pid = Convert.ToInt32(mo["IDProcess"]),
										  ProcCpu = Convert.ToInt32(mo["PercentProcessorTime"]),
										  TimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
									  }).Take(_summary.Config.TopCpuProcesses).ToArray();
					if (!topCpuProc.Any())
					{
						"Top CPU processes empty!".WriteLog(EventLogEntryType.Error, 12);
					}
					else
					{
						_summary.TopCpuProcesses = topCpuProc;
					}
				}
				_completed = true;
			}
			catch (ManagementException ex)
			{
				(ex.ErrorInformation + "\n" + ex.Message).WriteLog(EventLogEntryType.Error, 12);
			}
			catch (Exception ex)
			{
				(ex.Message + ex.InnerException).WriteLog(EventLogEntryType.Error, 12);
			}
		}
		public override async void OnTimerAsync(object sender, EventArgs args)
		{
			await Task.Run(() => OnTimer());
		}
	}
}
