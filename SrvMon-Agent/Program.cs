using System.ServiceProcess;
//server monitor root class
namespace Jake.Service
{
	static class Program
	{
		static void Main()
		{
			var servicesToRun = new ServiceBase[]
			{
				new SrvMonWatcher()
			};
			ServiceBase.Run(servicesToRun);
		}
	}
}
