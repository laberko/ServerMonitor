using System;
using System.Diagnostics;
using System.ServiceModel;
using Common;
using MySql.AspNet.Identity;
using Microsoft.AspNet.Identity;

namespace Bubblegum.Service.Code
{
	[ServiceBehavior(Namespace = "http://bubblegum.laberko.net",
		InstanceContextMode = InstanceContextMode.PerSession)]
	public partial class Server : IServer
	{
		//user authorization using MySQL database
		private static bool Auth(string userName, string password)
		{
			var passed = false;
			try
			{
				using (var userManager = new UserManager<IdentityUser>(new MySqlUserStore<IdentityUser>()))
				{
					var user = userManager.Find(userName, password);
					if (user != null)
					{
						passed = true;
					}
					return passed;
				}
			}
			catch (Exception ex)
			{
				(ex.Message + ex.InnerException).WriteLog(EventLogEntryType.Error, 101);
				return passed;
			}
		}
	}
	
}