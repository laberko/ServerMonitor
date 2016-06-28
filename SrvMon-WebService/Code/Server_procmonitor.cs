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
		private static void UpdateProcmonitor(SrvMonSummary summary)
		{
			try
			{
				using (var db = new candykingdomdbEntities())
				{
					var serverInDb = db.servers.Single(s => s.ServerGUID == summary.Config.ServerId);
					foreach (var proc in db.procmonitorcpu.Where(p => p.ServerID == serverInDb.ID))
					{
						//remove existing
						db.procmonitorcpu.Remove(proc);
					}
					foreach (var proc in summary.TopCpuProcesses.Take(serverInDb.TopCpuProcesses))
					{
						//add new
						db.procmonitorcpu.Add(new procmonitorcpu
						{
							ProcGUID = Guid.NewGuid(),
							ServerID = serverInDb.ID,
							ProcName = proc.ProcName,
							PID = proc.Pid,
							ProcCPU = proc.ProcCpu,
							TimeStamp = Convert.ToDateTime(proc.TimeStamp)
						});
					}
					foreach (var proc in db.procmonitorram.Where(p => p.ServerID == serverInDb.ID))
					{
						//remove existing
						db.procmonitorram.Remove(proc);
					}
					foreach (var proc in summary.TopRamProcesses.Take(serverInDb.TopRamProcesses))
					{
						//add new
						db.procmonitorram.Add(new procmonitorram
						{
							ProcGUID = Guid.NewGuid(),
							ServerID = serverInDb.ID,
							ProcName = proc.ProcName,
							PID = proc.Pid,
							ProcMemory = proc.ProcMemory,
							TimeStamp = Convert.ToDateTime(proc.TimeStamp)
						});
					}
					db.SaveChanges();
				}
			}
			catch (Exception ex) when (ex is OptimisticConcurrencyException || ex is DbUpdateConcurrencyException)
			{
				(ex.Message + ex.InnerException).WriteLog(EventLogEntryType.Warning, 106);
			}
			catch (Exception ex) when (ex is SocketException || ex is InvalidOperationException)
			{
				(ex.Message + ex.InnerException).WriteLog(EventLogEntryType.Error, 106);
			}
		}
	}
}