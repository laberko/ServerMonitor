namespace Finn.MVC.Models
{
    using System;

	public partial class diskmonitor
    {
        public Guid DiskGUID { get; set; }
        public int ServerID { get; set; }
        public string DiskLetter { get; set; }
        public string DiskLabel { get; set; }
        public int DiskSize { get; set; }
        public int DiskFree { get; set; }
        public DateTime TimeStamp { get; set; }
    
        public virtual servers servers { get; set; }
    }
}
