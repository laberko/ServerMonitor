using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Common;
using Microsoft.VisualBasic.Devices;
//RAM and CPU monitoring class
namespace Jake.Service
{
	public class HardwareMonitor:AbstractModule
	{
		private PerformanceCounter _pc = new PerformanceCounter();
		public HardwareMonitor(SrvMonSummary summary):base(summary)
		{
			_pc.CategoryName = "Processor";
			_pc.CounterName = "% Processor Time";
			_pc.InstanceName = "_Total";
		}
		public override void OnTimer(object sender, EventArgs args)
		{
			var ci = new ComputerInfo();
			_summary.RamFree = (int) (ci.AvailablePhysicalMemory/(1024*1024));
			_summary.RamTotal = (int) (ci.TotalPhysicalMemory/(1024*1024));
			_summary.CpuLoad = (int) _pc.NextValue();
		}
		public override async void OnTimerAsync(object sender, EventArgs args)
		{
			await Task.Run(() => OnTimer(sender, args));
		}
	}
}
