using System;
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
		public string UserId;
		[DataMember]
		public string MonitoredServices;
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
	}
}