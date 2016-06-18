using System;
using System.Threading.Tasks;
using System.ServiceModel;
using Common;

namespace Bubblegum.Service.Code
{
	[ServiceContract(Namespace = "http://bubblegum.laberko.net")]
	public interface IServer
	{
		//was client configuration changed on server?
		[OperationContract]
		Task<bool> GetConfigChangedAsync(Guid serverId);

		//check user password
		[OperationContract]
		Task<bool> AuthAsync(string userName, string password);

		//get client configuration from server
		[OperationContract]
		Task<SrvMonParams> GetConfigAsync(SrvMonParams jakeParams, string password);

		//send summary info from client to server
		[OperationContract]
		Task SendDataAsync(SrvMonSummary summary, string password);
	}
}