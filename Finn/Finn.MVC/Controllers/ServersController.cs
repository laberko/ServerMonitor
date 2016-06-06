using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.Web.Mvc;
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
		public async Task<ActionResult> Index()
        {
	        var userName = User.Identity.GetUserName();
			var userId = _db.AspNetUsers.Single(u => u.UserName == userName).Id;
			var servers = _db.servers.Where(s => s.UserID == userId);
			return View(await servers.ToListAsync());
		}

		// GET: Servers/Details/id
		public async Task<ActionResult> Details(int? id)
        {
			var userName = User.Identity.GetUserName();
			var userId = _db.AspNetUsers.Single(u => u.UserName == userName).Id;
			if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var servers = await _db.servers.FindAsync(id);
            if ((servers == null)||(servers.UserID!=userId))
            {
                return HttpNotFound();
            }
			return View(servers);
        }

		// GET: Servers/Services/id
		public async Task<ActionResult> Services(int? id)
		{
			var userName = User.Identity.GetUserName();
			var userId = _db.AspNetUsers.Single(u => u.UserName == userName).Id;
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			var servers = await _db.servers.FindAsync(id);
			if ((servers == null) || (servers.UserID != userId))
			{
				return HttpNotFound();
			}
			ViewBag.HostName = servers.HostName;
			ViewBag.ID = id;
			var services = _db.servicemonitor.Where(s => s.ServerID == id);
			return View(await services.OrderBy(s=>s.ServiceDisplayName).ToListAsync());
		}

		// GET: Servers/Events/id
		public async Task<ActionResult> Events(int? id)
		{
			var userName = User.Identity.GetUserName();
			var userId = _db.AspNetUsers.Single(u => u.UserName == userName).Id;
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			var servers = await _db.servers.FindAsync(id);
			if ((servers == null) || (servers.UserID != userId))
			{
				return HttpNotFound();
			}
			ViewBag.HostName = servers.HostName;
			ViewBag.EvMonTimeSpanHrs = servers.EvMonTimeSpanHrs;
			ViewBag.ID = id;
			var events = _db.eventmonitor.Where(e => e.ServerID == id);
			return View(await events.OrderByDescending(e => e.Count).ToListAsync());
		}

		// GET: Servers/TopCPU/id
		public async Task<ActionResult> TopCPU(int? id)
		{
			var userName = User.Identity.GetUserName();
			var userId = _db.AspNetUsers.Single(u => u.UserName == userName).Id;
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			var servers = await _db.servers.FindAsync(id);
			if ((servers == null) || (servers.UserID != userId))
			{
				return HttpNotFound();
			}
			ViewBag.HostName = servers.HostName;
			ViewBag.ID = id;
			var processes = _db.procmonitorcpu.Where(s => s.ServerID == id);
			return View(await processes.OrderByDescending(p => p.ProcCPU).ToListAsync());
		}

		// GET: Servers/TopRAM/id
		public async Task<ActionResult> TopRAM(int? id)
		{
			var userName = User.Identity.GetUserName();
			var userId = _db.AspNetUsers.Single(u => u.UserName == userName).Id;
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			var servers = await _db.servers.FindAsync(id);
			if ((servers == null) || (servers.UserID != userId))
			{
				return HttpNotFound();
			}
			ViewBag.HostName = servers.HostName;
			ViewBag.RamTotal = servers.RAMTotal;
			ViewBag.RamFree = servers.RAMFree;
			ViewBag.ID = id;
			var processes = _db.procmonitorram.Where(s => s.ServerID == id);
			return View(await processes.OrderByDescending(p => p.ProcMemory).ToListAsync());
		}

		// GET: Servers/DiskUsage/id
		public async Task<ActionResult> DiskUsage(int? id)
		{
			var userName = User.Identity.GetUserName();
			var userId = _db.AspNetUsers.Single(u => u.UserName == userName).Id;
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			var servers = await _db.servers.FindAsync(id);
			if ((servers == null) || (servers.UserID != userId))
			{
				return HttpNotFound();
			}
			ViewBag.HostName = servers.HostName;
			ViewBag.ID = id;
			var disks = _db.diskmonitor.Where(s => s.ServerID == id);
			return View(await disks.OrderBy(d=>d.DiskLetter).ToListAsync());
		}

		// GET: Servers/HwUsage/id
		public async Task<ActionResult> HwUsage(int? id)
		{
			var userName = User.Identity.GetUserName();
			var userId = _db.AspNetUsers.Single(u => u.UserName == userName).Id;
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}
			var servers = await _db.servers.FindAsync(id);
			if ((servers == null) || (servers.UserID != userId))
			{
				return HttpNotFound();
			}
			ViewBag.HostName = servers.HostName;
			ViewBag.RamTotal = servers.RAMTotal;
			ViewBag.ID = id;
			var hwlogs = _db.hardwarehistory.Where(s => s.ServerID == id);
			return View(await hwlogs.OrderBy(l => l.TimeStamp).ToListAsync());
		}

		// GET: Servers/Edit/id
		public async Task<ActionResult> Edit(int? id)
        {
			var userName = User.Identity.GetUserName();
			var userId = _db.AspNetUsers.Single(u => u.UserName == userName).Id;
			if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var server = await _db.servers.FindAsync(id);
            if ((server == null) || (server.UserID != userId))

			{
                return HttpNotFound();
            }
            return View(server);
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
				(ex.Message+ex.InnerException).WriteLog(System.Diagnostics.EventLogEntryType.Error, 201);
			}
            return View(server);
        }

        // GET: Servers/Delete/id
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var servers = await _db.servers.FindAsync(id);
            if (servers == null)
            {
                return HttpNotFound();
            }
            return View(servers);
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
