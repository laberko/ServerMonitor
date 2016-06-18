using System;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using Bubblegum.Service.EDM;
//get agent configuration
namespace Bubblegum.Service.Code
{
	public partial class Server
	{
		public async Task<SrvMonParams> GetConfigAsync(SrvMonParams jakeParams, string password)
		{
			return await Task.Factory.StartNew(() => GetConfig(jakeParams, password));
		}
		private static SrvMonParams GetConfig(SrvMonParams jakeParams, string password)
		{
			using (var db = new candykingdomdbEntities())
			{
				try
				{
					if (!Auth(jakeParams.UserId, password))
					{
						var sb = new StringBuilder();
						sb.AppendLine("Authorization error:");
						sb.AppendFormat("Login: {0}\nPassword: {1}", jakeParams.UserId, password);
						sb.ToString().WriteLog(EventLogEntryType.Error, 104);
						return jakeParams;
					}
					if (!db.servers.Any(s => s.ServerGUID == jakeParams.ServerId))
					{
						//add server
						db.servers.Add(new servers
						{
							ServerGUID = jakeParams.ServerId,
							UserID = db.AspNetUsers.Single(u => u.UserName == jakeParams.UserId).Id,
							HostName = jakeParams.HostName,
							OnlineTime = DateTime.Now.ToUniversalTime(),
							MonitoredServices = jakeParams.MonitoredServices,
							TopCpuProcesses = jakeParams.TopCpuProcesses,
							TopRamProcesses = jakeParams.TopRamProcesses,
							HwMonTimeSpan = jakeParams.HwMonTimeSpan,
							EvMonTimeSpan = jakeParams.EvMonTimeSpan,
							ServiceTimer = jakeParams.ServiceTimer,
							IsConfigChanged = true
						});
						db.SaveChanges();
						return jakeParams;
					}
					//get parameters from db
					var serverInDb = db.servers.Single(s => s.ServerGUID == jakeParams.ServerId);
					jakeParams.ServerId = serverInDb.ServerGUID;
					jakeParams.UserId = db.AspNetUsers.Single(u => u.Id == serverInDb.UserID).UserName;
					jakeParams.MonitoredServices = serverInDb.MonitoredServices;
					jakeParams.TopCpuProcesses = serverInDb.TopCpuProcesses;
					jakeParams.TopRamProcesses = serverInDb.TopRamProcesses;
					jakeParams.HwMonTimeSpan = serverInDb.HwMonTimeSpan;
					jakeParams.EvMonTimeSpan = serverInDb.EvMonTimeSpan;
					jakeParams.ServiceTimer = serverInDb.ServiceTimer;
					serverInDb.IsConfigChanged = false;
					db.SaveChanges();
					return jakeParams;
				}
				catch (DbEntityValidationException ex)
				{
					var sb = new StringBuilder();
					sb.AppendLine(jakeParams.UserId);
					sb.AppendLine(jakeParams.ServerId.ToString());
					foreach (var ve in ex.EntityValidationErrors.SelectMany(e => e.ValidationErrors))
					{
						sb.AppendLine(ve.ErrorMessage);
					}
					(ex.Message + sb).WriteLog(EventLogEntryType.Error, 104);
					return jakeParams;
				}
				catch (InvalidOperationException ex)
				{
					(ex.Message + ex.InnerException).WriteLog(EventLogEntryType.Error, 104);
					foreach (var server in db.servers.Where(s => s.ServerGUID == jakeParams.ServerId))
					{
						db.servers.Remove(server);
					}
					db.SaveChanges();
					return jakeParams;
				}
				catch (NullReferenceException ex)
				{
					(ex.Message + ex.InnerException).WriteLog(EventLogEntryType.Error, 104);
					return jakeParams;
				}
			}
		}
	}
}