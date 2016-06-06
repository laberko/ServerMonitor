namespace Bubblegum.Service.EDM
{
    using System;
    using System.Collections.Generic;
    
    public partial class servers
    {
        public servers()
        {
            this.servicemonitor = new HashSet<servicemonitor>();
            this.diskmonitor = new HashSet<diskmonitor>();
            this.hardwarehistory = new HashSet<hardwarehistory>();
            this.procmonitorcpu = new HashSet<procmonitorcpu>();
            this.procmonitorram = new HashSet<procmonitorram>();
            this.eventmonitor = new HashSet<eventmonitor>();
        }
    
        public int ID { get; set; }
        public System.Guid ServerGUID { get; set; }
        public string UserID { get; set; }
        public Nullable<System.DateTime> OnlineTime { get; set; }
        public string AgentVersion { get; set; }
        public Nullable<int> RAMTotal { get; set; }
        public Nullable<int> RAMFree { get; set; }
        public Nullable<int> CPULoad { get; set; }
        public string MonitoredServices { get; set; }
        public int TopRamProcesses { get; set; }
        public int TopCpuProcesses { get; set; }
        public string HwMonTimeSpan { get; set; }
        public string EvMonTimeSpan { get; set; }
        public string HostName { get; set; }
        public int ServiceTimer { get; set; }
        public Nullable<System.DateTime> UtcTime { get; set; }
        public string Note { get; set; }
        public bool IsConfigChanged { get; set; }
    
        public virtual ICollection<servicemonitor> servicemonitor { get; set; }
        public virtual ICollection<diskmonitor> diskmonitor { get; set; }
        public virtual ICollection<hardwarehistory> hardwarehistory { get; set; }
        public virtual ICollection<procmonitorcpu> procmonitorcpu { get; set; }
        public virtual ICollection<procmonitorram> procmonitorram { get; set; }
        public virtual ICollection<eventmonitor> eventmonitor { get; set; }
        public virtual AspNetUsers AspNetUsers { get; set; }
    }
}
