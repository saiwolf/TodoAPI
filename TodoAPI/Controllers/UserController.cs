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
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : Controller
    {
        // Delcare our DB context.
        private readonly TodoContext _context;

        // Declare our User Service
        private readonly IUserService _userService;

        /// <summary>
        /// Use Dependency Injection to populate our
        /// DB Context and our user service.
        /// </summary>
        /// <param name="userService">Psuedo-holder for our IUserService</param>
        public UserController(IUserService userService, TodoContext context)
        {
            _context = context;
            _userService = userService;
        }

        /// <summary>
        /// Authenticates a user with our IUserService.
        /// We mark it with [AllowAnonymous] to allow anonymous access.
        /// </summary>
        /// <param name="userParam">POSTed user credentials</param>
        /// <returns>BadRequest (400) if auth fails, Ok (200) with user token on success.</returns>
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody]User userParam)
        {
            var user = _userService.Authenticate(userParam.Username, userParam.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect." });

            return Ok(user);
        }

        //[AllowAnonymous]
        //[HttpPost("HashPass")]
        //public IActionResult HashPassword([FromBody]string pass2hash)
        //{
        //    // hash the password supplied
        //    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(pass2hash);

        //    if(BCrypt.Net.BCrypt.Verify(pass2hash, hashedPassword))
        //    {
        //        return Ok(hashedPassword);
        //    }
        //    else
        //    {
        //        return BadRequest(new { message = "Error hashing password!" });
        //    }
        //}

        /// <summary>
        /// Simple method to get all users, sans passwords.
        /// Users must be authorized first before being allowed
        /// to call this method.
        /// </summary>
        /// <returns>Ok (200) with users object from IUserService</returns>
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.User.AsNoTracking().ToListAsync();
            var users_sanitized = _userService.GetAll(users);
            return Ok(users_sanitized);
        }
    }
}
