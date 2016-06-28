using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;
using Common;
//event log error statistics
namespace Jake.Service
{
	public class EventMonitor:AbstractModule
	{
		public EventMonitor(SrvMonSummary summary):base(summary)
		{
		}
		private void OnTimer()
		{
			if (_summary.Config == null)
				return;
			var old = (DateTime.Now - TimeSpan.Parse(_summary.Config.EvMonTimeSpan)).ToString("o");
			var query = "*[System/Level<=2 and System/Level>0 and System/TimeCreated/@SystemTime >= '"
				+ old + "' and System/TimeCreated/@SystemTime <= '" 
				+ DateTime.Now.ToString("o") + "']";
			var eventRecords = new List<EventRecord>();
			foreach (var eventQuery in EventLog.GetEventLogs().Select(eventLog => new EventLogQuery(eventLog.Log, PathType.LogName, query)))
			{
				using (var logReader = new EventLogReader(eventQuery))
				{
					for (var eventDetail = logReader.ReadEvent(); eventDetail != null; eventDetail = logReader.ReadEvent())
					{
						try
						{
							if (eventDetail.FormatDescription()!=null)
							eventRecords.Add(eventDetail);
						}
						catch (EventLogException)
						{}
					}
				}
			}
			try
			{
				_summary.Events = (from eventEntry in eventRecords
								   orderby eventEntry.TimeCreated descending
								   let desc = eventEntry.FormatDescription()
								   group eventEntry by desc.IndexOf('\n') == -1 ? desc : desc.Substring(0, desc.IndexOf('\n'))
					into e
								   where e.Count() > 1
								   select new EventCount
								   {
									   Code = e.First().Id,
									   Source = e.First().ProviderName,
									   Text = e.Key,
									   Count = e.Count(),
									   LastTimeStamp = e.First().TimeCreated.GetValueOrDefault(DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss")
								   }).OrderByDescending(e=>e.Count).Take(20).ToArray();
			}
			catch (NullReferenceException ex)
			{
				(ex.Message + ex.InnerException).WriteLog(EventLogEntryType.Error, 11);
			}
		}
		public override async void OnTimerAsync(object sender, EventArgs args)
		{
			await Task.Run(() => OnTimer());
		}
	}
}