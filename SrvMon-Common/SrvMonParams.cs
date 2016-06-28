using System;
using System.Diagnostics;
using System.Runtime.Serialization;
//service monitor parameters
namespace Common
{
	[DataContract(Namespace = "http://laberko.net")]
	public class SrvMonParams
	{
		//default parameters
		[DataMember]
		public Guid ServerId;
		[DataMember]
		public string HostName;
		[DataMember]
		public string UserId = "";
		[DataMember]
		public string MonitoredServices = "";
		[DataMember]
		public int TopRamProcesses = 5;
		[DataMember]
		public int TopCpuProcesses = 5;
		[DataMember]
		public int ServiceTimer = 60;										//interval for monitoring tasks in seconds
		[DataMember]
		public string HwMonTimeSpan = new TimeSpan(1, 0, 0).ToString();     //timespan for hardware history
		[DataMember]
		public string EvMonTimeSpan = new TimeSpan(6, 0, 0).ToString();     //timespan for event statistics
		public SrvMonParams()
		{
			ServerId = Guid.NewGuid();
			HostName = Environment.MachineName;
		}
		public void SaveConfig()
		{
			try
			{
				Environment.MachineName.SetRegString("HostName");
				ServerId.ToString().SetRegString("ServerId");
				UserId.SetRegString("UserId");
				MonitoredServices.SetRegString("MonitoredServices");
				TopRamProcesses.ToString().SetRegString("TopRamProcesses");
				TopCpuProcesses.ToString().SetRegString("TopCpuProcesses");
				HwMonTimeSpan.SetRegString("HwMonTimeSpan");
				EvMonTimeSpan.SetRegString("EvMonTimeSpan");
				ServiceTimer.ToString().SetRegString("ServiceTimer");
				"Bubblegum".SetRegString("ServiceLogin");
				"CandyKingdom".SetRegString("ServicePassword");
				"Updated configuration in registry!".WriteLog(EventLogEntryType.Warning, 22);
			}
			catch (NullReferenceException ex)
			{
				(ex.Message + ex.InnerException).WriteLog(EventLogEntryType.Error, 22);
			}
		}
	}
}