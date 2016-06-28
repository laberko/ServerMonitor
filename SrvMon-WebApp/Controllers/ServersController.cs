using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;
using Finn.MVC.Models;
using Microsoft.AspNet.Identity;
using Common;

namespace Finn.MVC.Controllers
{
	[Authorize]
	public class ServersController : Controller
    {
        private readonly candykingdomdbEntities _db = new candykingdomdbEntities();

		// GET: Servers
		[OutputCache(NoStore = true, Location = OutputCacheLocation.Client, Duration = 30)]
		public async Task<ActionResult> Index()
        {
			if (!Request.IsAjaxRequest()) return View();
			var userName = User.Identity.GetUserName();
			var userId = _db.AspNetUsers.Single(u => u.UserName == userName).Id;
			var myServers = _db.servers.Where(s => s.UserID == userId).OrderBy(s => s.HostName);
			return PartialView("IndexPartial", await myServers.ToListAsync());
        }

		// GET: Servers/Details/id
		[OutputCache(NoStore = true, Location = OutputCacheLocation.Client, Duration = 30)]
		public async Task<ActionResult> Details(int? id)
        {
			if (!await IsServerOwner(id))
			{
				return HttpNotFound();
			}
			var server = await _db.servers.FindAsync(id);
			if (!Request.IsAjaxRequest()) return View(server);
			return PartialView("DetailsPartial", server);
        }

		// GET: Servers/Services/id
		[OutputCache(NoStore = true, Location = OutputCacheLocation.Client, Duration = 30)]
		public async Task<ActionResult> Services(int? id)
		{
			if (!await IsServerOwner(id))
			{
				return HttpNotFound();
			}
			if (Request.IsAjaxRequest())
				return PartialView("ServicesPartial", await _db.servicemonitor.Where(s => s.ServerID == id).OrderBy(s => s.ServiceDisplayName).ToListAsync());
			ViewBag.HostName = (await _db.servers.FindAsync(id)).HostName;
			ViewBag.ID = id;
			return View();
		}

		// GET: Servers/Events/id
		[OutputCache(NoStore = true, Location = OutputCacheLocation.Client, Duration = 30)]
		public async Task<ActionResult> Events(int? id)
		{
			if (!await IsServerOwner(id))
			{
				return HttpNotFound();
			}
			if (Request.IsAjaxRequest())
				return PartialView("EventsPartial", await _db.eventmonitor.Where(e => e.ServerID == id).OrderByDescending(e => e.Count).ToListAsync());
			var server = await _db.servers.FindAsync(id);
			ViewBag.HostName = server.HostName;
			ViewBag.EvMonTimeSpanHrs = server.EvMonTimeSpanHrs;
			ViewBag.ID = id;
			return View();
		}

		// GET: Servers/TopCpu/id
		[OutputCache(NoStore = true, Location = OutputCacheLocation.Client, Duration = 30)]
		public async Task<ActionResult> TopCpu(int? id)
		{
			if (!await IsServerOwner(id))
			{
				return HttpNotFound();
			}
			if (!Request.IsAjaxRequest())
			{
				ViewBag.ID = id;
				return View();
			}
			ViewBag.HostName = (await _db.servers.FindAsync(id)).HostName;
			return PartialView("TopCpuPartial", await _db.procmonitorcpu.Where(s => (s.ServerID == id) && (s.ProcCPU != 0)).OrderByDescending(p => p.ProcCPU).ToListAsync());
		}

		// GET: Servers/TopRam/id
		[OutputCache(NoStore = true, Location = OutputCacheLocation.Client, Duration = 30)]
		public async Task<ActionResult> TopRam(int? id)
		{
			if (!await IsServerOwner(id))
			{
				return HttpNotFound();
			}
			if (Request.IsAjaxRequest())
			{
				var server = await _db.servers.FindAsync(id);
				ViewBag.RamTotal = server.RAMTotal;
				ViewBag.RamFree = server.RAMFree;
				ViewBag.HostName = server.HostName;
				return PartialView("TopRamPartial", await _db.procmonitorram.Where(s => s.ServerID == id).OrderByDescending(p => p.ProcMemory).ToListAsync());
			}
			ViewBag.ID = id;
			return View();
		}

		// GET: Servers/DiskUsage/id
		public async Task<ActionResult> DiskUsage(int? id)
		{
			if (!await IsServerOwner(id))
			{
				return HttpNotFound();
			}
			ViewBag.HostName = (await _db.servers.FindAsync(id)).HostName;
			ViewBag.ID = id;
			return View(await _db.diskmonitor.Where(s => s.ServerID == id).OrderBy(d=>d.DiskLetter).ToListAsync());
		}

		// GET: Servers/HwUsage/id
		[OutputCache(NoStore = true, Location = OutputCacheLocation.Client, Duration = 30)]
		public async Task<ActionResult> HwUsage(int? id)
		{
			if (!await IsServerOwner(id))
			{
				return HttpNotFound();
			}
			if (Request.IsAjaxRequest())
			{
				ViewBag.RamTotal = (await _db.servers.FindAsync(id)).RAMTotal;
				return PartialView("hwUsagePartial", await _db.hardwarehistory.Where(s => s.ServerID == id).OrderBy(l => l.TimeStamp).ToListAsync());
			}
			ViewBag.HostName = (await _db.servers.FindAsync(id)).HostName;
			ViewBag.ID = id;
			return View();
		}

		// GET: Servers/Edit/id
		public async Task<ActionResult> Edit(int? id)
        {
			if (!await IsServerOwner(id))
			{
				return HttpNotFound();
			}
			return View(await _db.servers.FindAsync(id));
        }

		// POST: Servers/Edit/id
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Edit( servers server)
		{
			server.IsConfigChanged = true;
			if (!ModelState.IsValid) return View(server);
			try
			{
				_db.Entry(server).State = EntityState.Modified;
				await _db.SaveChangesAsync();
				return RedirectToAction("Index");
			}
			catch (DbEntityValidationException ex)
			{
				var sb = new StringBuilder();
				sb.AppendLine(ex.Message);
				foreach (var item in ex.EntityValidationErrors.SelectMany(error => error.ValidationErrors))
				{
					sb.AppendLine(item.ErrorMessage);
				}
				sb.ToString().WriteLog(System.Diagnostics.EventLogEntryType.Error, 201);
			}
			catch (Exception ex)
			{
				(ex.Message + ex.InnerException).WriteLog(System.Diagnostics.EventLogEntryType.Error, 201);
			}
            return View(server);
        }

        // GET: Servers/Delete/id
        public async Task<ActionResult> Delete(int? id)
        {
			if (!await IsServerOwner(id))
			{
				return HttpNotFound();
			}
			return View(await _db.servers.FindAsync(id));
        }

        // POST: Servers/Delete/id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var servers = await _db.servers.FindAsync(id);
            _db.servers.Remove(servers);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

		//check if server with id exists and current user is its owner
		private async Task<bool> IsServerOwner(int? serverId)
		{
			if (serverId == null)
				return false;
			var server = await _db.servers.FindAsync(serverId);
			var userName = User.Identity.GetUserName();
			var userId = _db.AspNetUsers.Single(u => u.UserName == userName).Id;
			return (server != null) && (server.UserID == userId);
		}

		protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
