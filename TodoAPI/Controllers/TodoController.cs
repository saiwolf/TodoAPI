using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoAPI.Models;
using TodoAPI.DAL;
using TodoAPI.Utilities;
using Serilog;
using System.Linq;
using TodoAPI.Filters;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoAPI.Controllers
{
    /// <summary>
    /// Our Todo API controller!
    /// </summary>
    [ServiceFilter(typeof(ClientIpCheckFilter))]
    [Route("api/[controller]")]
    [ApiController]
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
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                // Populate `todos` as an IEnumerable<Todo> List from the DB.

                var todos = await _context.Todo.ToListAsync();

                if (!todos.Any())
                {
                    throw new Exception("TodoController.cs [GetAll]: Error retrieving data from DB: No records available.");
                }

                // We don't check if `todos` is null here. The client should have a case
                // to check if the response is blank.
                return Ok(todos);
            }
            catch(Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest(new { Error = "No Records available." });
            }
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
            try
            {
                // Fetch our record from the DB asynchronously
                Todo todo = await _context.Todo.FirstOrDefaultAsync(m => m.Id == id);
                // If `todo` is null, then we very likely don't have any record with that `{id}`.
                // So we return a HTTP 404 (Not Found).
                if (todo == null)
                {
                    throw new Exception("TodoController.cs [GetTodo]: Error retrieving record from DB: Record ID not found.");
                }
                // Else, we return a HTTP 200 (OK) along with our record in JSON format.
                return Ok(todo);
            }
            catch(Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest(new { Error = "Record ID was not found." });
            }
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
            // Try-Catch block for POST.
            try
            {
                // Is the request body actually populated?
                if (todo == null)
                {
                    // Nope, so return an error.
                    Log.Error("TodoController.cs [CreateTodo]: No data received from client.");
                    return BadRequest(new { Message = "No data received from client." });
                }

                // If there's an error with the data provided, then return an error.
                if (!ModelState.IsValid)
                {
                    throw new Exception($"TodoController.cs [CreateTodo]: Data received from client was invalid.\n{ModelState.Values.ToString()}");
                }
                _context.Todo.Add(todo);
                await _context.SaveChangesAsync();
                return CreatedAtRoute("GetTodo", new { id = todo.Id }, todo);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return BadRequest(new { Error = $"Data received from client was invalid.\n{ModelState.Values.ToString()}" });
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
            try
            {
                // Find our record to update in the DB.
                var todo = await _context.Todo.FirstOrDefaultAsync(m => m.Id == id);

                // If `todo` is null, then we didn't find a record.
                if (todo == null)
                {
                    // So we return a 404 (Not Found)
                    Log.Error("TodoController.cs [UpdateTodo]: Error retrieving record: record does not exist.");
                    return NotFound();
                }

                // Assign Values to Model
                todo.Name = item.Name;
                todo.Content = item.Content;

                // If our ModelState is valid, then our data is good against our Model.
                if (!ModelState.IsValid)
                {
                    throw new Exception($"TodoController.cs [UpdateTodo]: Data received from client was invalid.\n{ModelState.Values.ToString()}");
                }
                _context.Todo.Update(todo);
                await _context.SaveChangesAsync();
                return NoContent(); // HTTP Standard recommends returning 204 (No Content) on POST, PUT, and DELETE operations.                
            }
            catch (Exception ex)
            {
                Log.Error($"TodoController.cs [UpdateTodo]: Something went wrong updating the Todo. {ex.Message}");
                return BadRequest(new { Error = $"Error updating the Todo item:\n{ModelState.Values.ToString()}" });
            }
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
            try
            {
                // Find our record in the DB.
                var todo = await _context.Todo.FirstOrDefaultAsync(m => m.Id == id);

                // If `todo` is null, then we didn't find our record.
                if (todo == null)
                {
                    // So we return a 404 (Not Found)
                    Log.Error("TodoController.cs [DeleteTodo]: Error retrieving data from DB: Record does not exist.");
                    return NotFound();
                }

                _context.Todo.Remove(todo);
                await _context.SaveChangesAsync();

                return NoContent(); // HTTP Standard recommends returning 204 (No Content) on POST, PUT, and DELETE operations.
            }
            catch (Exception ex)
            {
                Log.Error($"TodoController.cs [DeleteTodo]: Error deleting record: {ex.Message}");
                return BadRequest(new { Error = "Error deleting Todo." });
            }
        }
    }
}
