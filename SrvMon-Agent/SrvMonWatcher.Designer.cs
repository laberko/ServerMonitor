using System.ComponentModel;
using System.Diagnostics;

namespace Jake.Service
{
	partial class SrvMonWatcher
	{
		private IContainer components = null;
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		private void InitializeComponent()
		{
			jakeEventLog = new EventLog();
			((ISupportInitialize)(jakeEventLog)).BeginInit();
			ServiceName = "SrvMonWatcher";
			((ISupportInitialize)(jakeEventLog)).EndInit();
		}
		private EventLog jakeEventLog;
	}
}
