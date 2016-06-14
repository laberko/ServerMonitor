namespace Finn.MVC.Models
{
	using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class candykingdomdbEntities : DbContext
    {
        public candykingdomdbEntities()
            : base("name=candykingdomdbEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<diskmonitor> diskmonitor { get; set; }
        public virtual DbSet<eventmonitor> eventmonitor { get; set; }
        public virtual DbSet<hardwarehistory> hardwarehistory { get; set; }
        public virtual DbSet<procmonitorcpu> procmonitorcpu { get; set; }
        public virtual DbSet<procmonitorram> procmonitorram { get; set; }
        public virtual DbSet<servers> servers { get; set; }
        public virtual DbSet<servicemonitor> servicemonitor { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
    }
}
