using System;
using System.ComponentModel.DataAnnotations;

namespace Finn.MVC.Models
{
	public partial class servicemonitor
	{
		public Guid ServiceGUID
		{
			get; set;
		}
		public int ServerID
		{
			get; set;
		}
		public string ServiceName
		{
			get; set;
		}
		[Display(Name = "Имя службы")]
		public string ServiceDisplayName
		{
			get; set;
		}
		[Display(Name = "Состояние")]
		public string State
		{
			get; set;
		}
		[Display(Name = "Обновлено")]
		public DateTime TimeStamp
		{
			get; set;
		}
		public virtual servers servers
		{
			get; set;
		}
	}
}
