using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Common;
using Microsoft.AspNet.Identity;
using MySql.AspNet.Identity;

namespace Bubblegum.Service.Code
{
	public partial class Server
	{
		//user authorization using MySQL database
		public async Task<bool> AuthAsync(string userName, string password)
		{
			return await Task.Factory.StartNew(() => Auth(userName, password));
		}
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