using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CityInfo.API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public class AuthenticationRequestBody
        {
            public string UserName { get; set; }
            public string Password { get; set; }    
        }
        private class CityInfoUser
        {
            public int UserId { get; set; }
            public string UserName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string City { get; set; }

            public CityInfoUser(
                int userId,
                string userName,
                string firstName,
                string lastName,
                string city)
            {
                UserId = userId;
                UserName = userName;
                FirstName = firstName;
                LastName = lastName;
                City = city;
            }

        }
        public AuthenticationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [HttpPost("authenticate")]
        public ActionResult<string> Authenticate(AuthenticationRequestBody authentication )
        {
            //Validate User name and password
            var user=ValidateUserCredetials(authentication.UserName,authentication.Password);

            if(user==null)
            {
                return Unauthorized();
            }

            //Create A token
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(_configuration["Authentication:SecretForKey"]));

            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claimsForToken = new List<Claim>();
            claimsForToken.Add(new Claim("sub", user.UserId.ToString()));
            claimsForToken.Add(new Claim("given_name", user.FirstName));
            claimsForToken.Add(new Claim("family_name", user.LastName));
            claimsForToken.Add(new Claim("city", user.City));

            var jwtSecurityToken = new JwtSecurityToken(
               _configuration["Authentication:Issuer"],
               _configuration["Authentication:Audience"],
               claimsForToken,
               DateTime.UtcNow,
               DateTime.UtcNow.AddHours(1),
               signingCredentials);

            var tokenReturn=new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            return Ok(tokenReturn);
        }
        private CityInfoUser ValidateUserCredetials(string? userName,string? password)
        {
            return new CityInfoUser(
                1, null ?? " ",
                "Kevin",
                "Dockx",
                "Paris"
                );
        }
    }
}
