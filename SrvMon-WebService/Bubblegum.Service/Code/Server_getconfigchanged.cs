using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Bubblegum.Service.EDM;
using Common;

namespace Bubblegum.Service.Code
{
	public partial class Server
	{
		public async Task<bool> GetConfigChangedAsync(Guid serverId)
		{
			return await Task.Factory.StartNew(() => GetConfigChanged(serverId));
		}
		private static bool GetConfigChanged(Guid serverId)
		{
			using (var db = new candykingdomdbEntities())
			{
				try
				{
					return db.servers.Single(s => s.ServerGUID == serverId).IsConfigChanged;
				}
				catch (SocketException ex)
				{
					(ex.Message + ex.InnerException).WriteLog(EventLogEntryType.Error, 108);
					return true;
				}
				catch (ArgumentNullException ex)
				{
					(ex.Message + ex.InnerException).WriteLog(EventLogEntryType.Error, 108);
					return true;
				}
				catch (InvalidOperationException ex)
				{
					(ex.Message + ex.InnerException).WriteLog(EventLogEntryType.Error, 108);
					foreach (var server in db.servers.Where(s => s.ServerGUID == serverId))
					{
						db.servers.Remove(server);
					}
					db.SaveChanges();
					return true;
				}
			}
		}
	}
}