using System;
using Common;

//service state change event information class
namespace Jake.Service
{
	public class ServiceStateEventArgs : EventArgs
	{
		public ServiceStateEventArgs(ServiceState service, string oldState)
		{
			Service = service;
			OldState = oldState;
			Moment = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
		}
		public ServiceState Service { get; }
		public string OldState { get; }
		public string Moment { get; }
	}
}
