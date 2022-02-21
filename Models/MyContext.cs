using Microsoft.EntityFrameworkCore;

namespace login_and_registration.Models
{ 
    public class MyContext : DbContext 
    { 
        public MyContext(DbContextOptions options) : base(options) { }
        public DbSet<User> Users { get; set; }
    }
}