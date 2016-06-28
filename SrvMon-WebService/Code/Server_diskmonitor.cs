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
		private static void UpdateDiskMonitor(SrvMonSummary summary)
		{
			try
			{
				using (var db = new candykingdomdbEntities())
				{
					var serverInDb = db.servers.Single(s => s.ServerGUID == summary.Config.ServerId);
					foreach (var drive in db.diskmonitor.Where(d => d.ServerID == serverInDb.ID))
					{
						//remove existing
						db.diskmonitor.Remove(drive);
					}
					foreach (var drive in summary.LocalDrives.Take(20))
					{
						db.diskmonitor.Add(new diskmonitor
						{
							DiskGUID = Guid.NewGuid(),
							ServerID = serverInDb.ID,
							DiskLetter = drive.DriveLetter.ToString(),
							DiskLabel = drive.DriveLabel,
							DiskSize = drive.DriveSize,
							DiskFree = drive.DriveFree,
							TimeStamp = Convert.ToDateTime(drive.TimeStamp)
						});
					}
					db.SaveChanges();
				}
			}
			catch (Exception ex) when (ex is OptimisticConcurrencyException || ex is DbUpdateConcurrencyException)
			{
				(ex.Message + ex.InnerException).WriteLog(EventLogEntryType.Warning, 102);
			}
			catch (Exception ex) when (ex is SocketException || ex is InvalidOperationException)
			{
				(ex.Message + ex.InnerException).WriteLog(EventLogEntryType.Error, 102);
			}
		}
	}
}