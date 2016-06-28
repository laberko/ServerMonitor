namespace Finn.MVC.Models
{
	using System.Collections.Generic;
    
    public partial class AspNetRoles
    {
        public AspNetRoles()
        {
            AspNetUsers = new HashSet<AspNetUsers>();
        }
    
        public string Id { get; set; }
        public string Name { get; set; }

	    public virtual ICollection<AspNetUsers> AspNetUsers { get; set; }
    }
}
