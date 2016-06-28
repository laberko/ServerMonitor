using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common;
//monitor local drives capacity
namespace Jake.Service
{
	public class DiskMonitor:AbstractModule
	{
		public DiskMonitor(SrvMonSummary summary):base(summary)
		{
		}
		private void OnTimer()
		{
			_summary.LocalDrives = (from drive in DriveInfo.GetDrives()
				where (drive.DriveType == DriveType.Fixed)
				select new Disk
				{
					DriveLetter = drive.Name.First(),
					DriveLabel = drive.VolumeLabel,
					DriveSize = (int)(drive.TotalSize / (1024 * 1024)),
					DriveFree = (int)(drive.TotalFreeSpace / (1024 * 1024)),
					TimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
				}).ToArray();
		}
		public override async void OnTimerAsync(object sender, EventArgs args)
		{
			await Task.Run(() => OnTimer());
		}
	}
}
