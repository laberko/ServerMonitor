using System.ServiceModel;

namespace Bubblegum.Service.Code
{
	[ServiceBehavior(Namespace = "http://bubblegum.laberko.net", InstanceContextMode = InstanceContextMode.PerSession)]
	public partial class Server : IServer
	{
	}
}