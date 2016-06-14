using System;
using System.ComponentModel.DataAnnotations;

namespace Finn.MVC.Models
{
    public partial class procmonitorcpu
    {
        public Guid ProcGUID { get; set; }
        public int ServerID { get; set; }
		[Display(Name = "Имя процесса")]
		public string ProcName { get; set; }
		[Display(Name = "ID процесса")]
		public int PID { get; set; }
		[Display(Name = "Загрузка CPU")]
		public int? ProcCPU { get; set; }
        public DateTime TimeStamp { get; set; }
        public virtual servers servers { get; set; }
    }
}
