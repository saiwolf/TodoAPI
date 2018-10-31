using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TodoAPI.Models;

namespace TodoAPI.DAL
{
    public static class DbInitializer
    {
        public static void Initialize(TodoContext context)
        {
            //Ensure our DB exists
            //context.Database.EnsureCreated();

            // Look for any records
            if (context.Todo.Any() || context.User.Any())
            {
                return; // DB has been seeded already.
            }

            var users = new User[]
            {
                new User { FirstName = "John", LastName = "Doe", Username = "johndoe", Email="john.doe@contoso.com", Password = "$2y$12$zGV/Ufg.IBuSYYyiCo6ibe49zIMsJ8GJynn6CFasK9tjUmE0aPHG2" },
                new User { FirstName = "Jane", LastName = "Doe", Username = "janedoe", Email="jane.doe@contoso.com", Password = "$2y$12$zGV/Ufg.IBuSYYyiCo6ibe49zIMsJ8GJynn6CFasK9tjUmE0aPHG2" },
                new User { FirstName = "James", LastName = "Randi", Username = "jrandi", Email="jrandi@strand.edu", Password = "$2y$12$zGV/Ufg.IBuSYYyiCo6ibe49zIMsJ8GJynn6CFasK9tjUmE0aPHG2" },
            };

            foreach (User u in users)
            {
                context.User.Add(u);
            }
            context.SaveChanges();

            var todos = new Todo[]
            {
                new Todo { Name = "Monday", Content = "You can fall apart.", IsComplete = true, UserId = 1 },
                new Todo { Name = "Tuesday", Content = "Break My Heart", IsComplete = true, UserId = 2 },
                new Todo { Name = "Wednesday", Content = "Break My Heart Pt.2", IsComplete = true, UserId = 3 },
                new Todo { Name = "Thursday", Content = "Doesn't even start.", IsComplete = false, UserId = 2 },
                new Todo { Name = "Friday", Content = "I'm in love.", IsComplete = false, UserId = 1 }
            };

            foreach (Todo t in todos)
            {
                context.Todo.Add(t);
            }
            context.SaveChanges();
        }
    }
}
