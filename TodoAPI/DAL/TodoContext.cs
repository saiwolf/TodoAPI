using Microsoft.EntityFrameworkCore;
using TodoAPI.Models;

namespace TodoAPI.DAL
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions options)
            : base(options)
        { }

        public DbSet<Todo> Todo { get; set; }
    }
}
