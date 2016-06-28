using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using System.Threading.Tasks;
using Common;
using Jake.Service.Bubblegum;

//invoke operations on wcf proxy class object
namespace Jake.Service
{
	public class WcfClient : AbstractModule
	{
		//proxy class object
		private ServerClient _client;

		public WcfClient(SrvMonSummary summary):base(summary)
		{
			try
			{
				_client = GetClient();
			}
			catch (Exception ex)
			{
				(ex.Message + ex.InnerException).WriteLog(EventLogEntryType.Error, 6);
			}
		}
		 ~WcfClient()
		 {
			 _client.Close();
		 }
		//create wcf proxy object
		private ServerClient GetClient()
		{
			try
			{
				var client = new ServerClient();
				client.ClientCredentials.UserName.UserName = "ServiceLogin".GetRegString();
				client.ClientCredentials.UserName.Password = "ServicePassword".GetRegString();
				return client;
			}
			catch (Exception ex) when (ex is SocketException || ex is WebException || ex is NullReferenceException)
			{
				(ex.Message + ex.InnerException).WriteLog(EventLogEntryType.Error, 3);
				_client.Abort();
				return null;
			}
			catch (Exception ex)
			{
				(ex.Message + ex.InnerException).WriteLog(EventLogEntryType.Error, 3);
				return null;
			}
		}
		//was configuration changed on server?
		public async Task<bool> ConfigChanged(Guid serverId)
		{
			if ((_client == null) || (_client.State != CommunicationState.Opened))
			{
				_client = GetClient();
			}
			try
			{
				return await _client.GetConfigChangedAsync(serverId);
			}
			catch (FaultException ex)
			{
				(ex.Message).WriteLog(EventLogEntryType.Error, 5);
			}
			catch (Exception ex)
			{
				(ex.Message + ex.InnerException).WriteLog(EventLogEntryType.Error, 5);
				_client.Abort();
			}
			return false;
		}
		//update agent configuration from server
		public async Task<SrvMonParams> GetServerConfig(SrvMonParams config)
		{
			if ((_client == null) || (_client.State != CommunicationState.Opened))
			{
				_client = GetClient();
			}
			try
			{
				return await _client.GetConfigAsync(config, "UserPassword".GetRegString());
			}
			catch (FaultException ex)
			{
				(ex.Message).WriteLog(EventLogEntryType.Error, 4);
			}
			catch (Exception ex)
			{
				(ex.Message + ex.InnerException).WriteLog(EventLogEntryType.Error, 4);
				_client.Abort();
			}
			return config;
		}
		//send info to server
		public override async void OnTimerAsync(object sender, EventArgs args)
		{
			if ((_client == null) || (_client.State != CommunicationState.Opened))
			{
				_client = GetClient();
			}
			try
			{
				if ((_summary.Config != null) && (_summary.ValidationErrors().Count == 0))
				{
					await _client.SendDataAsync(_summary, "UserPassword".GetRegString());
				}
			}
			catch (FaultException ex)
			{
				(ex.Message).WriteLog(EventLogEntryType.Error, 1);
			}
			catch (Exception ex)
			{
				(ex.Message + ex.InnerException).WriteLog(EventLogEntryType.Error, 1);
				_client.Abort();
			}
		}
	}
}