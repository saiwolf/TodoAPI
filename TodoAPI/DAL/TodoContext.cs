using Microsoft.EntityFrameworkCore;
using TodoAPI.Models;

namespace TodoAPI.DAL
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions options)
            : base(options)
        { }

        public virtual DbSet<Todo> Todo { get; set; }
        public virtual DbSet<User> User { get; set; }
    }
}
