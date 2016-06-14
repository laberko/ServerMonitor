using System.Runtime.Serialization;

namespace Common
//service state class
{
	[DataContract(Namespace = "http://laberko.net")]
	public class ServiceState
	{
		[DataMember]
		public string Name;
		[DataMember]
		public string DisplayName;
		[DataMember]
		public string State;
		[DataMember]
		public string TimeStamp;
		//check if state changed
		public bool IsChanged(ServiceState oldState)
		{
			return oldState != null && (Name == oldState.Name) && (State != oldState.State);
		}
	}
}
