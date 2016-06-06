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
					var old = DateTime.Now.ToUniversalTime() - TimeSpan.Parse(summary.Config.HwMonTimeSpan);
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
						TimeStamp = Convert.ToDateTime(summary.OnlineTime)
					});
				db.SaveChanges();
				}
			}
			catch (OptimisticConcurrencyException ex)
			{
				(ex.Message + ex.InnerException).WriteLog(EventLogEntryType.Warning, 105);
			}
			catch (DbUpdateConcurrencyException ex)
			{
				(ex.Message + ex.InnerException).WriteLog(EventLogEntryType.Warning, 105);
			}
			catch (SocketException ex)
			{
				(ex.Message + ex.InnerException).WriteLog(EventLogEntryType.Error, 105);
			}
			catch (InvalidOperationException ex)
			{
				(ex.Message + ex.InnerException).WriteLog(EventLogEntryType.Error, 105);
			}
		}

	}
}
