using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Common
//service monitor summary info
{
	[DataContract(Name = "SrvMonSummary", Namespace = "http://laberko.net")]
	public class SrvMonSummary
	{
		private const string LogPath = "C:\\ProgramData\\SrvMon\\";
		[DataMember]
		public string AgentVersion;											//from constructor
		[DataMember]
		public string OnlineTime { get; set; }                              //from OnTimer	
		[DataMember]
		public string UtcTime { get; set; }									//from OnTimer
		[DataMember]
		public int RamTotal { get; set; }                                   //from HardwareMonitor
		[DataMember]
		public int RamFree { get; set; }                                    //from HardwareMonitor
		[DataMember]
		public int CpuLoad { get; set; }                                    //from HardwareMonitor
		[DataMember]
		public SrvMonParams Config { get; set; }                            //from SrvMonConfiguration
		[DataMember]
		public ServiceState[] ServiceStates { get; set; }                   //from ServiceMonitor
		[DataMember]
		public Proc[] TopRamProcesses { get; set; }                         //from ProcessMonitor
		[DataMember]
		public Proc[] TopCpuProcesses { get; set; }                         //from ProcessMonitor
		[DataMember]
		public Disk[] LocalDrives { get; set; }                             //from DiskMonitor
		[DataMember]
		public EventCount[] Events { get; set; }                            //from EventMonitor
		public SrvMonSummary()
		{
			AgentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
		}
		//serialize to xml file
		private void Serialize(string path)
		{
			try
			{
				var settings = new XmlWriterSettings { Indent = true };
				using (var writer = XmlWriter.Create(path, settings))
				{
					var serializer = new DataContractSerializer(typeof(SrvMonSummary));
					serializer.WriteObject(writer, this);
					writer.Flush();
					writer.Close();
				}
			}
			catch (IOException ex)
			{
				(ex.Message + ex.InnerException).WriteLog(EventLogEntryType.Warning, 20);
			}
		}
		public List<string> ValidationErrors()
		{
			var errorList = new List<string>();
			try
			{
				if (RamFree >= RamTotal)
					errorList.Add("Wrong free RAM size.");
				if (CpuLoad > 100)
					errorList.Add("Wrong CPU load.");
				if ((ServiceStates.Length > 20) || (Config.MonitoredServices.Split(null).Distinct().Count() > 20))
					errorList.Add("Too many services.");
				if ((TopCpuProcesses.Length > 10) || (Config.TopCpuProcesses > 10))
					errorList.Add("Too many top processes (CPU).");
				if ((TopRamProcesses.Length > 10) || (Config.TopRamProcesses > 10))
					errorList.Add("Too many top processes (RAM).");
				if (LocalDrives.Length > 20)
					errorList.Add("Too many local drives.");
				if (Events.Length > 20)
					errorList.Add("Too many events.");
				if (Config.ServiceTimer < 30)
					errorList.Add("Wrong data send timer.");
				if ((TimeSpan.Parse(Config.EvMonTimeSpan) > new TimeSpan(23, 0, 0)) ||
					(TimeSpan.Parse(Config.HwMonTimeSpan) > new TimeSpan(6, 0, 0)))
					errorList.Add("Too big timespan.");
			}
			catch (FormatException ex)
			{
				errorList.Add(ex.Message);
				return errorList;
			}
			finally
			{
				if (errorList.Count > 0)
				{
					var sb = new StringBuilder();
					foreach (var error in errorList)
					{
						sb.AppendLine(error);
					}
					sb.ToString().WriteLog(EventLogEntryType.Error, 20);
				}
			}
			return errorList;
		}
		public async void OnTimer(object sender, EventArgs args)
		{
			if (Config == null)
			{
				"Wrong configuration! Run Configurator!".WriteLog(EventLogEntryType.Error, 20);
				return;
			}
			var now = DateTime.Now;
			OnlineTime = now.ToString("yyyy-MM-dd HH:mm:ss");
			UtcTime = now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
			//write to xml file
			await Task.Run(() => Serialize(Path.Combine(LogPath, "Summary.xml")));
			//write services states to Event Log
			var sb = new StringBuilder();
			sb.AppendLine("Current states of services:");
			foreach (var service in ServiceStates)
			{
				sb.AppendFormat("{0} ({1}): {2}\n", service.DisplayName, service.Name, service.State);
			}
			sb.AppendFormat("Server ID: {0}", Config.ServerId);
			sb.ToString().WriteLog(EventLogEntryType.Information, 20);
		}
	}
}
