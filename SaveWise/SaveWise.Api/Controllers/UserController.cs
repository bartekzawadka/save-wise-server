using System;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SaveWise.Api.Common;
using SaveWise.BusinessLogic.Services;
using SaveWise.DataLayer.Models.Users;

namespace SaveWise.Api.Controllers
{
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IOptions<SecuritySettings> _securitySettings;

        public UserController(IUserService userService, IOptions<SecuritySettings> securitySettings)
        {
            _userService = userService;
            _securitySettings = securitySettings;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] User userData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(GetErrorFromModelState());
            }

            try
            {
                User user = await _userService.AuthenticateAsync(userData.Username, userData.Password);
                return Ok(GetApiToken(user));
            }
            catch (AuthenticationException ae)
            {
                return StatusCode((int) HttpStatusCode.Unauthorized, GetMessageObject(ae.Message));
            }
            catch (Exception ex)
            {
                return StatusCode((int) HttpStatusCode.InternalServerError, GetMessageObject(ex.Message));
            }
        }

        [HttpGet("refreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            User user = await _userService.GetByIdAsync(HttpContext.User.Identity.Name);
            return Ok(GetApiToken(user));
        }

        private ApiToken GetApiToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(_securitySettings.Value.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.Id)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            string tokenString = tokenHandler.WriteToken(token);

            return new ApiToken
            {
                Token = tokenString,
                Expires = tokenDescriptor.Expires,
                Username = user.Username,
                Id = user.Id
            };
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Register user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(GetErrorFromModelState());
            }

            try
            {
                User userCreated = await _userService.CreateAsync(user);
                return Ok(GetApiToken(userCreated));
            }
            catch (ArgumentNullException ane)
            {
                return BadRequest(GetMessageObject(ane.Message));
            }
            catch (DuplicateNameException dne)
            {
                return BadRequest(GetMessageObject(dne.Message));
            }
            catch (Exception e)
            {
                return StatusCode((int) HttpStatusCode.InternalServerError, GetMessageObject(e.Message));
            }
        }

        [HttpPut("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePassword changePassword)
        {
            try
            {
                GetErrorFromModelState();

                await _userService.ChangePassword(changePassword);
                return Ok();
            }
            catch (ArgumentNullException ane)
            {
                return BadRequest(GetMessageObject(ane.Message));
            }
            catch (AuthenticationException ae)
            {
                return BadRequest(GetMessageObject(ae.Message));
            }
            catch (Exception exception)
            {
                return StatusCode((int) HttpStatusCode.InternalServerError, GetMessageObject(exception.Message));
            }
        }
    }
}