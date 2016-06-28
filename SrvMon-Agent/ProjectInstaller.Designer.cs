using System.ServiceProcess;

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
			serviceProcessInstaller = new ServiceProcessInstaller();
			serviceInstaller = new ServiceInstaller();
			serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
			serviceProcessInstaller.Password = null;
			serviceProcessInstaller.Username = null;
			serviceInstaller.Description = "Server Monitor Agent Service";
			serviceInstaller.DisplayName = "Server Monitor";
			serviceInstaller.ServiceName = "SrvMonWatcher";
			serviceInstaller.StartType = ServiceStartMode.Automatic;
			Installers.AddRange(new System.Configuration.Install.Installer[] {
            serviceProcessInstaller,
            serviceInstaller});
		}
		private ServiceProcessInstaller serviceProcessInstaller;
		private ServiceInstaller serviceInstaller;
	}
}