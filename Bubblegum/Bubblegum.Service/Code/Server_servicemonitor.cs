using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.ServiceModel;
using System.Web;
using Bubblegum.Service.EDM;
using Common;

namespace Bubblegum.Service.Code
{
	public partial class Server
	{
		private static void UpdateServicemonitor(SrvMonSummary summary)
		{
			try
			{
				using (var db = new candykingdomdbEntities())
				{
					var serverInDb = db.servers.Single(s => s.ServerGUID == summary.Config.ServerId);
					foreach (var service in summary.ServiceStates)
					{
						switch (db.servicemonitor.Count(m => (m.ServerID == serverInDb.ID) && (m.ServiceName == service.Name)))
						{
							case 0:
								//if no such service in db
								db.servicemonitor.Add(new servicemonitor
								{
									ServiceGUID = Guid.NewGuid(),
									ServerID = serverInDb.ID,
									ServiceName = service.Name,
									ServiceDisplayName = service.DisplayName,
									State = service.State,
									TimeStamp = Convert.ToDateTime(service.TimeStamp)
								});
								goto Unswitch;
							case 1:
								//update state
								var serviceInDb = db.servicemonitor.Single(m => (m.ServerID == serverInDb.ID) && (m.ServiceName == service.Name));
								if (serviceInDb.State != service.State)
								{
									serviceInDb.State = service.State;
								}
								serviceInDb.TimeStamp = Convert.ToDateTime(service.TimeStamp);
								goto Unswitch;
							default:
								//remove possible duplicates
								db.servicemonitor.Remove(
									db.servicemonitor.First(m => (m.ServerID == serverInDb.ID) && (m.ServiceName == service.Name)));
								goto Unswitch;
						}
						Unswitch:
						db.SaveChanges();
					}
					//remove non-monitored services
					foreach (var service in db.servicemonitor
						.Where(m => (m.ServerID == serverInDb.ID)).ToList()
						.Where(s => !summary.ServiceStates.Any(m => (m.Name == s.ServiceName))))
							{
								db.servicemonitor.Remove(service);
							}
					db.SaveChanges();
				}
			}
			catch (OptimisticConcurrencyException ex)
			{
				(ex.Message + ex.InnerException).WriteLog(EventLogEntryType.Warning, 107);
			}
			catch (DbUpdateConcurrencyException ex)
			{
				(ex.Message + ex.InnerException).WriteLog(EventLogEntryType.Warning, 107);
			}
			catch (SocketException ex)
			{
				(ex.Message + ex.InnerException).WriteLog(EventLogEntryType.Error, 107);
			}
			catch (InvalidOperationException ex)
			{
				(ex.Message + ex.InnerException).WriteLog(EventLogEntryType.Error, 107);
			}
		}
	}
}