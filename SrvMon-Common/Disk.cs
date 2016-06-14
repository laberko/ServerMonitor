using System.Runtime.Serialization;
//disk info class
namespace Common
{
	[DataContract(Namespace = "http://laberko.net")]
	public class Disk
	{
		[DataMember]
		public char? DriveLetter;
		[DataMember]
		public string DriveLabel;
		[DataMember]
		public int DriveSize;
		[DataMember]
		public int DriveFree;
		[DataMember]
		public string TimeStamp;
	}
}
