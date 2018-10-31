using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoAPI.DAL;
using TodoAPI.Models;
using TodoAPI.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoAPI.Controllers
{
    /// <summary>
    /// Our Todo API controller!
    /// [Authorize] tells ASP.NET MVC that users
    /// must authenticate first to be allowed to
    /// use this controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TodoController : ControllerBase
    {
        // Delcare our DB context.
        private readonly TodoContext _context;

        /// <summary>
        /// Use Dependency Injection to populate our
        /// _context DB context.
        /// </summary>
        /// <param name="context">Psuedo-holder for our DB context values.</param>
        public TodoController(TodoContext context)
        {
            _context = context;
        }

        /// <summary>
        /// <para>
        /// GET: api/Todo
        /// </para>
        /// <para>
        /// SQL: SELECT * FROM dbo.Todos;
        /// </para>
        /// <para>
        /// Fetches all records in the DB and returns
        /// them in an <code>IEnumerable</code> list.
        /// </para>
        /// </summary>
        /// <returns>Status Code 200 (OK) along with the list of to-dos in JSON format.</returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Populate `todos` as an IEnumerable<Todo> List from the DB.
            IEnumerable<Todo> todos = await _context.Todo.ToListAsync();
            // We don't check if `todos` is null here. The client should have a case
            // to check if the response is blank.
            return Ok(todos);
        }

        /// <summary>
        /// <para>
        /// GET: api/Todos/{id}
        /// </para>
        /// <para>
        /// SQL: SELECT * FROM dbo.Todos WHERE [id] = '{id}';
        /// </para>
        /// <para>
        /// Fetches a single record from our DB.
        /// </para>
        /// </summary>
        /// <param name="id">Record ID to be returned as JSON.</param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetTodo")] // We name this Action, so CreatedAtRoute later on will work.
        public async Task<IActionResult> GetTodo(int id)
        {
            // Fetch our record from the DB asynchronously
            Todo todo = await _context.Todo.FirstOrDefaultAsync(m => m.Id == id);
            // If `todo` is null, then we very likely don't have any record with that `{id}`.
            // So we return a HTTP 404 (Not Found).
            if (todo == null)
            {
                return NotFound("ID wasn't found.");
            }
            // Else, we return a HTTP 200 (OK) along with our record in JSON format.
            return Ok(todo);
        }

        /// <summary>
        /// <para>
        /// POST: api/Todo
        /// </para>
        /// <para>
        /// SQL: INSERT INTO dbo.Todo ([Name],[Content])
        ///  VALUES({todo.name},{todo.content});
        /// </para>
        /// <para>
        /// Creates a record in our DB from the request body sent.
        /// Supports Model verification, and returns an error if the
        /// ModelState is not valid.
        /// </para>
        /// </summary>
        /// <param name="todo">Values in the POSTed request body</param>
        /// <returns>A location header with the GET URL of the newly created record. Bad Request (400) otherwise.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateTodo([FromBody] Todo todo)
        {
            // Is the request body actually populated?
            if (todo == null)
            {
                // Nope, so return an error.
                return BadRequest("No Data given");
            }

            // If there's an error with the data provided, then return an error.
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            
            // Try-Catch block for DB operations.
            try
            {
                _context.Todo.Add(todo);
                await _context.SaveChangesAsync();
                return CreatedAtRoute("GetTodo", new { id = todo.Id }, todo);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        /// <summary>
        /// <para>
        /// PUT: api/Todo/{id}
        /// </para>
        /// <para>
        /// SQL: UPDATE Todos SET [Name] = {item.name},[Content] = {item.content}
        ///  WHERE [Id] = '{id}';
        /// </para>
        /// </summary>
        /// <param name="id">ID of record we want to update.</param>
        /// <param name="item">POSTed request body containing values to update our record with.</param>
        /// <returns>204 (No Content) on success, 404 (Not Found) if no record exists, and 400 (Bad Request) on error.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodo(int id, [FromBody] Todo item)
        {
            // Find our record to update in the DB.
            var todo = await _context.Todo.FirstOrDefaultAsync(m => m.Id == id);

            // If `todo` is null, then we didn't find a record.
            if (todo == null)
            {
                // So we return a 404 (Not Found)
                return NotFound();
            }

            // Assign Values to Model
            todo.Name = item.Name;
            todo.Content = item.Content;

            // If our ModelState is valid, then our data is good against our Model.
            if (ModelState.IsValid)
            {
                _context.Todo.Update(todo);
                await _context.SaveChangesAsync();
                return NoContent(); // HTTP Standard recommends returning 204 (No Content) on POST, PUT, and DELETE operations.
            }

            return BadRequest("Error updating todo."); // If we reached here, something went drastically wrong.
        }

        /// <summary>
        /// <para>
        /// DELETE: api/Todo/{id}
        /// </para>
        /// <para>
        /// SQ: DELETE FROM Todos WHERE Id = {id};
        /// </para>
        /// <para>
        /// Deletes a record from our DB.
        /// </para>
        /// </summary>
        /// <param name="id">ID of record we want to delete.</param>
        /// <returns>404 (Not Found) if no such <paramref name="id"/> exists, or 204 (No Content) if successful.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            // Find our record in the DB.
            var todo = await _context.Todo.FirstOrDefaultAsync(m => m.Id == id);

            // If `todo` is null, then we didn't find our record.
            if (todo == null)
            {
                // So we return a 404 (Not Found)
                return NotFound();
            }

            _context.Todo.Remove(todo);
            await _context.SaveChangesAsync();

            return NoContent(); // HTTP Standard recommends returning 204 (No Content) on POST, PUT, and DELETE operations.
        }
    }
}
