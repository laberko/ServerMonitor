using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Timers;
using Common;

namespace Jake.Service
{
	public partial class SrvMonWatcher : ServiceBase
	{
		public SrvMonWatcher()
		{
			InitializeComponent();
			var timer = new Timer { Interval = double.Parse("ServiceTimer".GetRegString())*1000 };
			var summary = new SrvMonSummary();
			var client = new Client(summary);
			var config = new SrvMonConfiguration(summary, client, timer);
			var hwMonitor = new HardwareMonitor(summary);
			var diskMonitor = new DiskMonitor(summary);
			var serviceMonitor = new ServiceMonitor(summary);
			var processMonitor = new ProcessMonitor(summary);
			var evMonitor = new EventMonitor(summary);
			timer.Elapsed += OnTimer;
			timer.Elapsed += config.OnTimer;
			timer.Elapsed += hwMonitor.OnTimerAsync;
			timer.Elapsed += diskMonitor.OnTimerAsync;
			timer.Elapsed += serviceMonitor.OnTimerAsync;
			timer.Elapsed += processMonitor.OnTimerAsync;
			timer.Elapsed += evMonitor.OnTimerAsync;
			timer.Elapsed += summary.OnTimer;
			timer.Elapsed += client.OnTimerAsync;
			timer.Start();
		}
		protected override void OnStart(string[] args)
		{
			"Server Monitor Service Started!".WriteLog(EventLogEntryType.Information, 0);
		}
		protected override void OnStop()
		{
			"Server Monitor Service Stopped!".WriteLog(EventLogEntryType.Warning, 0);
		}
		private static void OnTimer(object sender, ElapsedEventArgs args)
		{
		}
	}
}
