using Microsoft.EntityFrameworkCore;

namespace Meegoda.IDP.Entities
{
    public class TodoUserContext : DbContext
    {
        public TodoUserContext(DbContextOptions<TodoUserContext> options)
           : base(options)
        {
           
        }
        public DbSet<User> Users { get; set; }
    }
}
