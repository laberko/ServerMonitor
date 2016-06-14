using System.Runtime.Serialization;
//event log item
namespace Common
{
	[DataContract(Namespace = "http://laberko.net")]
	public class EventCount
	{
		[DataMember]
		public long Code;
		[DataMember]
		public string Source;
		[DataMember]
		public string Text;
		[DataMember]
		public int Count;
		[DataMember]
		public string LastTimeStamp;
	}
}
