using System;
using Common;

namespace Jake.Service
{
	public abstract class AbstractModule
	{
		protected SrvMonSummary _summary;
		protected AbstractModule(SrvMonSummary summary)
		{
			_summary = summary;
		}
		public abstract void OnTimerAsync(object sender, EventArgs args);
	}
}
