using Microsoft.EntityFrameworkCore;

namespace EmpMS.Data
{
    public class AppDbContext : DbContext
    {
        //constructor for DB Context
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
           
        }

        //Dbsets (Tables)

        //Configurations
    }
}
