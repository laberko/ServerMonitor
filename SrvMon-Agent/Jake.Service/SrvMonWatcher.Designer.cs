namespace Jake.Service
{
	partial class SrvMonWatcher
	{
		private System.ComponentModel.IContainer components = null;
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
			this.jakeEventLog = new System.Diagnostics.EventLog();
			((System.ComponentModel.ISupportInitialize)(this.jakeEventLog)).BeginInit();
			this.ServiceName = "SrvMonWatcher";
			((System.ComponentModel.ISupportInitialize)(this.jakeEventLog)).EndInit();
		}
		private System.Diagnostics.EventLog jakeEventLog;
	}
}
