using System;
using System.Threading.Tasks;
using System.ServiceModel;
using Common;

namespace Bubblegum.Service.Code
{
	[ServiceContract(Namespace = "http://bubblegum.laberko.net")]
	public interface IServer
	{
		[OperationContract]
		bool GetConfigChanged(Guid serverId);
		[OperationContract]
		SrvMonParams GetConfig(SrvMonParams jakeParams, string password);
		[OperationContract]
		Task SendDataAsync(SrvMonSummary summary, string password);
	}
}