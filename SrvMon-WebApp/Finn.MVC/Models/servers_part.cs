using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Finn.MVC.Models
{
	public sealed partial class servers
	{
		[NotMapped]
		[Display(Name = "Диапазон истории загрузки (часы)")]
		public int HwMonTimeSpanHrs
		{
			get
			{
				return HwMonTimeSpan == null ? 0 : TimeSpan.Parse(HwMonTimeSpan).Hours;
			}
			set
			{
				HwMonTimeSpan = value.ToString("00") + ":00:00";
			}
		}
		[NotMapped]
		[Display(Name = "Диапазон статистики по ошибкам (часы)")]
		public int EvMonTimeSpanHrs
		{
			get
			{
				return EvMonTimeSpan == null ? 0 : TimeSpan.Parse(EvMonTimeSpan).Hours;
			}
			set
			{
				EvMonTimeSpan = value.ToString("00") + ":00:00";
			}
		}
		[NotMapped]
		[Display(Name = "Частота обновления (минуты)")]
		public int ServiceTimerMin
		{
			get
			{
				return ServiceTimer / 60;
			}
			set
			{
				ServiceTimer = value * 60;
			}
		}
		[NotMapped]
		[Display(Name = "Состояние сервера")]
		public string ServerState
		{
			get
			{
				if (UtcTime == null)
				{
					return "Неизвестно";
				}
				var span = (DateTime.Now.ToUniversalTime() - UtcTime.GetValueOrDefault()).Minutes;
				return span > ServiceTimerMin * 2 ? "Не в сети" : "В сети";
			}
		}
		[NotMapped]
		[Display(Name = "Свободно RAM (%)")]
		public string RamFreePercent
		{
			get
			{
				if ((RAMFree.HasValue) && (RAMTotal.HasValue))
				{
					return ((double)RAMFree / (double)RAMTotal).ToString("0.0%");
				}
				return "Неизвестно";
			}
		}
		[NotMapped]
		[Display(Name = "Загрузка CPU (%)")]
		public string CpuLoadPercent => CPULoad.HasValue ? CPULoad.Value+"%" : "Неизвестно";
	}
}