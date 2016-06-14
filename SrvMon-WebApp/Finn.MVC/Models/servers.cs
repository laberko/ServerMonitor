using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Finn.MVC.Models
{
	public partial class servers
	{
		public servers()
		{
			diskmonitor = new HashSet<diskmonitor>();
			eventmonitor = new HashSet<eventmonitor>();
			hardwarehistory = new HashSet<hardwarehistory>();
			procmonitorcpu = new HashSet<procmonitorcpu>();
			procmonitorram = new HashSet<procmonitorram>();
			servicemonitor = new HashSet<servicemonitor>();
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
		public DateTime? OnlineTime
		{
			get; set;
		}
		[Display(Name = "Последнее обновление (UTC)")]
		public DateTime? UtcTime
		{
			get; set;
		}
		[Display(Name = "Версия ПО")]
		public string AgentVersion
		{
			get; set;
		}
		[Display(Name = "Объем RAM (МБ)")]
		public int? RAMTotal
		{
			get; set;
		}
		[Display(Name = "Свободно RAM (МБ)")]
		public int? RAMFree
		{
			get; set;
		}
		[Display(Name = "Загрузка CPU (%)")]
		public int? CPULoad
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

		public ICollection<diskmonitor> diskmonitor
		{
			get; set;
		}

		public ICollection<eventmonitor> eventmonitor
		{
			get; set;
		}

		public ICollection<hardwarehistory> hardwarehistory
		{
			get; set;
		}

		public ICollection<procmonitorcpu> procmonitorcpu
		{
			get; set;
		}

		public ICollection<procmonitorram> procmonitorram
		{
			get; set;
		}

		public ICollection<servicemonitor> servicemonitor
		{
			get; set;
		}
		public AspNetUsers AspNetUsers
		{
			get; set;
		}
	}
}
