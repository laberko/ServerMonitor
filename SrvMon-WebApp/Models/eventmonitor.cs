using System.ComponentModel.DataAnnotations;

namespace Finn.MVC.Models
{
    public partial class eventmonitor
    {
        public System.Guid EventGUID { get; set; }
        public int ServerID { get; set; }
		[Display(Name = "Код")]
		public int Code { get; set; }
		[Display(Name = "Источник")]
		public string Source { get; set; }
		[Display(Name = "Текст")]
		public string Text { get; set; }
		[Display(Name = "Количество")]
		public int Count { get; set; }
		[Display(Name = "Время последней")]
		public System.DateTime LastTimeStamp { get; set; }
        public virtual servers servers { get; set; }
    }
}
