using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Common;
//services monitoring class
namespace Jake.Service
{
	public class ServiceMonitor:AbstractModule
	{
		private ServiceState[] _currentState;
		//service state change event
		public event EventHandler<ServiceStateEventArgs> ServiceStateChanged;
		public ServiceMonitor(SrvMonSummary summary):base(summary)
		{
			//subscribe to service state change events
			ServiceStateChanged += ServiceEvent;
			GetServiceState();
		}
		private void GetServiceState()
		{
			var services = new List<ServiceController>();
			try
			{
				if (_summary.Config != null)
				{
					services.AddRange(
						from service in _summary.Config.MonitoredServices.Split(null).Distinct()
						where service.Length > 0
						let srv = new ServiceController(service)
						where srv.ServiceName == service
						select srv);
				}
			}
			catch (Exception ex) when (ex is InvalidOperationException || ex is NullReferenceException)
			{
				(ex.Message + ex.InnerException).WriteLog(EventLogEntryType.Error, 13);
			}
			var newState =
				(from s in services
					select new ServiceState
					{
						Name = s.ServiceName,
						DisplayName = s.DisplayName,
						State = s.Status.ToString(),
						TimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
					}).ToArray();
			if ((_currentState!=null)&&(_currentState.Length==newState.Length))
			{
				for (var i = 0; i < newState.Length; i++)
				{
					if (newState[i].IsChanged(_currentState[i]))
					{
						//raise event if state changed
						OnServiceStateEvent(new ServiceStateEventArgs(newState[i], _currentState[i].State));
					}
				}
			}
			_currentState = newState;
			_summary.ServiceStates = _currentState;
		}
		//event notification
		private void OnServiceStateEvent(ServiceStateEventArgs e)
		{
			//temporary variable for thread safety:
			var handler = ServiceStateChanged;
			//notify subscibers if not null:
			handler?.Invoke(this, e);
		}
		//on service state change
		private static async void ServiceEvent(object sender, ServiceStateEventArgs e)
		{
			await Task.Run(() =>
			{
				var sb = new StringBuilder();
				sb.AppendLine("Service state changed!");
				sb.AppendFormat("{0} ({1})\n", e.Service.DisplayName, e.Service.Name);
				sb.AppendFormat("Old state: {0}\n", e.OldState);
				sb.AppendFormat("New state: {0}\n", e.Service.State);
				sb.AppendFormat("Time: {0}", e.Moment);
				sb.ToString().WriteLog(EventLogEntryType.Warning, 13);
			});
		}
		public override async void OnTimerAsync(object sender, EventArgs args)
		{
			await Task.Run(() => GetServiceState());
		}
	}
}
