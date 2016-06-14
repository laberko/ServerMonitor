namespace Finn.MVC.Models
{
    using System;

	public partial class hardwarehistory
    {
        public Guid HwMonGUID { get; set; }
        public int ServerID { get; set; }
        public int RAMFree { get; set; }
        public int CPULoad { get; set; }
        public DateTime TimeStamp { get; set; }
    
        public virtual servers servers { get; set; }
    }
}
