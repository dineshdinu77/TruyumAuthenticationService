using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuthenticationService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AuthenticationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(AuthController));
        static List<User> users;
        private IConfiguration _config;
        static AuthController()
        {
            users = new List<User>() 
            {
                new User(){Userid=1,Username="dinu",Password="vasu"},
                new User(){Userid=2,Username="dinesh",Password="vasireddy"},

            };

        }
        public AuthController(IConfiguration configuration)
        {
            _config = configuration;


        }


        [HttpPost("Login")]
        public IActionResult Login([FromBody] User login)
        {
            _log4net.Info("Authentication initiated");

            IActionResult response = Unauthorized();
            var user = users.FirstOrDefault(c => c.Username == login.Username && c.Password == login.Password);
            if (user == null)
            {
                _log4net.Info("User Not Found");
                return NotFound();
            }

            else
            {

                _log4net.Info("Login credential matched");
                return Ok(new
                {
                    token = GenerateJSONWebToken(user)
                });
            }
        }
        private string GenerateJSONWebToken(User userInfo)
        {
            _log4net.Info("Token Generation initiated");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                null,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
