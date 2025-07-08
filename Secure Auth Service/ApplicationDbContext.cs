using Microsoft.EntityFrameworkCore;
using Secure_Auth_Service.Model;

namespace Secure_Auth_Service
{
    public class ApplicationDbContext :DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> Context) : base(Context) { 
        
        
        }
      public   DbSet<users> Users { get; set; }
    }
}
