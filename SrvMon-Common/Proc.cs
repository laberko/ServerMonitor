using System.Runtime.Serialization;
//process info class
namespace Common
{
	[DataContract(Namespace = "http://laberko.net")]
	public class Proc
	{
		[DataMember]
		public string ProcName;
		[DataMember]
		public int Pid;
		[DataMember]
		public int? ProcMemory;
		[DataMember]
		public int? ProcCpu;
		[DataMember]
		public string TimeStamp;
	}
}
