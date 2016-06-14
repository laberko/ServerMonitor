namespace Jake.Service
{
	partial class ProjectInstaller
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
			this.serviceProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();
			this.serviceInstaller1 = new System.ServiceProcess.ServiceInstaller();
			this.serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
			this.serviceProcessInstaller1.Password = null;
			this.serviceProcessInstaller1.Username = null;
			this.serviceInstaller1.Description = "Server Monitor Service";
			this.serviceInstaller1.DisplayName = "Server Monitor";
			this.serviceInstaller1.ServiceName = "SrvMonWatcher";
			this.serviceInstaller1.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
			this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstaller1,
            this.serviceInstaller1});
		}
		private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller1;
		private System.ServiceProcess.ServiceInstaller serviceInstaller1;
	}
}