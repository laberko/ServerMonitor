using System;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using Bubblegum.Service.EDM;
using Common;

namespace Bubblegum.Service.Code
{
	public partial class Server
	{
		private static void UpdateHardwareHistory(SrvMonSummary summary)
		{
			try
			{
				using (var db = new candykingdomdbEntities())
				{
					var serverInDb = db.servers.Single(s => s.ServerGUID == summary.Config.ServerId);
					var old = DateTime.Now - TimeSpan.Parse(summary.Config.HwMonTimeSpan);
					//remove old records
					foreach (var hw in db.hardwarehistory.Where(h => h.ServerID == serverInDb.ID))
					{
						if (hw.TimeStamp < old)
						{
							db.hardwarehistory.Remove(hw);
						}
					}
					db.hardwarehistory.Add(new hardwarehistory
					{
						HwMonGUID = Guid.NewGuid(),
						ServerID = serverInDb.ID,
						RAMFree = summary.RamFree,
						CPULoad = summary.CpuLoad,
						TimeStamp = DateTime.Now
					});
				db.SaveChanges();
				}
			}
			catch (Exception ex) when (ex is OptimisticConcurrencyException || ex is DbUpdateConcurrencyException)
			{
				(ex.Message + ex.InnerException).WriteLog(EventLogEntryType.Warning, 105);
			}
			catch (Exception ex) when (ex is SocketException || ex is InvalidOperationException)
			{
				(ex.Message + ex.InnerException).WriteLog(EventLogEntryType.Error, 105);
			}
		}
	}
}
