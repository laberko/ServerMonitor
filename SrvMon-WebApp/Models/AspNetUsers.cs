using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Finn.MVC.Models
{
    public partial class AspNetUsers
    {
        public AspNetUsers()
        {
            AspNetUserClaims = new HashSet<AspNetUserClaims>();
            AspNetUserLogins = new HashSet<AspNetUserLogins>();
            AspNetRoles = new HashSet<AspNetRoles>();
            servers = new HashSet<servers>();
        }
    
        public string Id { get; set; }
		[Display(Name = "Пользователь")]
		public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTime? LockoutEndDateUtc { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public string UserName { get; set; }

	    public ICollection<AspNetUserClaims> AspNetUserClaims { get; set; }
	    public ICollection<AspNetUserLogins> AspNetUserLogins { get; set; }
	    public ICollection<AspNetRoles> AspNetRoles { get; set; }
	    public ICollection<servers> servers { get; set; }
	}
}
