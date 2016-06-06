using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Finn.MVC.Models
{
	public partial class servers
	{
		public servers()
		{
			this.diskmonitor = new HashSet<diskmonitor>();
			this.eventmonitor = new HashSet<eventmonitor>();
			this.hardwarehistory = new HashSet<hardwarehistory>();
			this.procmonitorcpu = new HashSet<procmonitorcpu>();
			this.procmonitorram = new HashSet<procmonitorram>();
			this.servicemonitor = new HashSet<servicemonitor>();
		}
		public int ID
		{
			get; set;
		}
		[Required]
		[Display(Name = "ID сервера")]
		public Guid ServerGUID
		{
			get; set;
		}
		[Display(Name = "Владелец")]
		public string UserID
		{
			get; set;
		}
		[Display(Name = "Имя сервера")]
		public string HostName
		{
			get; set;
		}
		[Display(Name = "Последнее обновление")]
		public Nullable<System.DateTime> OnlineTime
		{
			get; set;
		}
		[Display(Name = "Последнее обновление (UTC)")]
		public Nullable<System.DateTime> UtcTime
		{
			get; set;
		}
		[Display(Name = "Версия ПО")]
		public string AgentVersion
		{
			get; set;
		}
		[Display(Name = "Объем RAM (МБ)")]
		public Nullable<int> RAMTotal
		{
			get; set;
		}
		[Display(Name = "Свободно RAM (МБ)")]
		public Nullable<int> RAMFree
		{
			get; set;
		}
		[Display(Name = "Загрузка CPU (%)")]
		public Nullable<int> CPULoad
		{
			get; set;
		}
		public bool IsConfigChanged
		{
			get; set;
		}
		[Display(Name = "Отслеживаемые службы")]
		public string MonitoredServices
		{
			get; set;
		}
		[Display(Name = "Топ процессов (RAM)")]
		public int TopRamProcesses
		{
			get; set;
		}
		[Display(Name = "Топ процессов (CPU)")]
		public int TopCpuProcesses
		{
			get; set;
		}
		[Display(Name = "Диапазон истории загрузки")]
		public string HwMonTimeSpan
		{
			get; set;
		}
		[Display(Name = "Диапазон статистики по ошибкам")]
		public string EvMonTimeSpan
		{
			get; set;
		}
		[Display(Name = "Частота обновления")]
		public int ServiceTimer
		{
			get; set;
		}
		public string Note
		{
			get; set;
		}
		public virtual ICollection<diskmonitor> diskmonitor
		{
			get; set;
		}
		public virtual ICollection<eventmonitor> eventmonitor
		{
			get; set;
		}
		public virtual ICollection<hardwarehistory> hardwarehistory
		{
			get; set;
		}
		public virtual ICollection<procmonitorcpu> procmonitorcpu
		{
			get; set;
		}
		public virtual ICollection<procmonitorram> procmonitorram
		{
			get; set;
		}
		public virtual ICollection<servicemonitor> servicemonitor
		{
			get; set;
		}
		public virtual AspNetUsers AspNetUsers
		{
			get; set;
		}
	}
}
