using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TodoAPI.DAL;
using TodoAPI.Models;
using TodoAPI.Helpers;


namespace TodoAPI.Services
{
    public interface IUserService
    {
        User Authenticate(string username, string password);
        IEnumerable<User> GetAll(List<User> users);
    }

    public class UserService : IUserService
    {
        private readonly AppSettings _appSettings;

        private readonly TodoContext _context;

        public UserService(IOptions<AppSettings> appSettings, TodoContext context)
        {
            _appSettings = appSettings.Value;
            _context = context;
        }        

        public User Authenticate(string username, string password)
        {
            var user = _context.User.SingleOrDefault(x => x.Username == username);

            // return null if user not found.
            if (user == null)
            {
                return null;
            }

            // We found the record for that user, now we compare the passwords

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            if(!BCrypt.Net.BCrypt.Verify(password, hashedPassword))
            {
                return null;
            }

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            // remove password before returning
            user.Password = null;

            _context.Dispose();

            return user;
        }

        public IEnumerable<User> GetAll(List<User> users)
        {            
            // return users without passwords
            return users.Select(x =>
            {
                x.Password = null;
                return x;
            });
        }
    }    
}
