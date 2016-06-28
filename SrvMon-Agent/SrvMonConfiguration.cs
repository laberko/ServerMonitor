using System;
using System.Diagnostics;
using System.Net.Mail;
using System.Timers;
using Common;
//server monitor configuration actions
namespace Jake.Service
{
	public class SrvMonConfiguration:AbstractModule
	{
		private readonly WcfClient _client;
		private readonly Timer _timer;
		private bool _configurationNeeded;
		public SrvMonConfiguration(SrvMonSummary summary, WcfClient client, Timer timer):base(summary)
		{
			_client = client;
			_timer = timer;
			_summary.Config = LoadConfig();
		}
		private SrvMonParams LoadConfig()
		{
			var jakeParams = new SrvMonParams();
			_configurationNeeded = false;
			try
			{
				if ("ServerId".GetRegString() != null)
				{
					if ("ServerId".GetRegString() == "")
					{
						jakeParams.ServerId = Guid.NewGuid();
						jakeParams.ServerId.ToString().SetRegString("ServerId");
					}
					else
						jakeParams.ServerId = Guid.Parse("ServerId".GetRegString());
				}
				else
					_configurationNeeded = true;
				if ("HostName".GetRegString() != null)
				{
					if ("HostName".GetRegString() == "")
					{
						jakeParams.HostName = Environment.MachineName;
						jakeParams.HostName.SetRegString("HostName");
					}
					else
						jakeParams.HostName = "HostName".GetRegString();
				}
				else
					_configurationNeeded = true;
				if ("UserId".GetRegString() != null)
					jakeParams.UserId = (new MailAddress ("UserId".GetRegString())).ToString();
				else
					_configurationNeeded = true;
				if ("MonitoredServices".GetRegString() != null)
					jakeParams.MonitoredServices = "MonitoredServices".GetRegString();
				else
					_configurationNeeded = true;
				if ("TopRamProcesses".GetRegString() != null)
					jakeParams.TopRamProcesses = int.Parse("TopRamProcesses".GetRegString());
				else
					_configurationNeeded = true;
				if ("TopCpuProcesses".GetRegString() != null)
					jakeParams.TopCpuProcesses = int.Parse("TopCpuProcesses".GetRegString());
				else
					_configurationNeeded = true;
				if ("HwMonTimeSpan".GetRegString() != null)
					jakeParams.HwMonTimeSpan = TimeSpan.Parse("HwMonTimeSpan".GetRegString()).ToString();
				else
					_configurationNeeded = true;
				if ("EvMonTimeSpan".GetRegString() != null)
					jakeParams.EvMonTimeSpan = TimeSpan.Parse("EvMonTimeSpan".GetRegString()).ToString();
				else
					_configurationNeeded = true;
				if ("ServiceTimer".GetRegString() != null)
					jakeParams.ServiceTimer = int.Parse("ServiceTimer".GetRegString());
				else
					_configurationNeeded = true;
				_timer.Interval = jakeParams.ServiceTimer*1000;
				return jakeParams;
			}
			catch (Exception ex)
			{
				(ex.Message + ex.InnerException).WriteLog(EventLogEntryType.Error, 14);
				"Run Confugurator!".WriteLog(EventLogEntryType.Error, 14);
				_configurationNeeded = true;
				return null;
			}
		}
		public override async void OnTimerAsync(object sender, EventArgs args)
		{
				if ((_summary.Config != null) && (await _client.ConfigChanged(_summary.Config.ServerId)))
				{
					//get config from server and save to registry
					(await _client.GetServerConfig(_summary.Config)).SaveConfig();
				}
				//load config from registry
				_summary.Config = LoadConfig();
				if (!_configurationNeeded)
					return;
				//configuration incomplete
				_summary.Config = null;
				"Run Confugurator!".WriteLog(EventLogEntryType.Error, 14);
		}
	}
}