using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using RepairBox.API.Models;
using RepairBox.BL.DTOs.User;
using RepairBox.BL.Services;
using RepairBox.Common.Commons;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authorization;

namespace RepairBox.API.Controllers
{
    [Route(DeveloperConstants.ENDPOINT_PREFIX)]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        private IUserServiceRepo _userRepo;
        public AuthController(IMemoryCache cache, IConfiguration configuration, IUserServiceRepo userRepo)
        {
            _cache = cache;
            _configuration = configuration;
            _userRepo = userRepo;
        }

        [HttpPost("Login")]
        public IActionResult Login(UserLoginDTO userLogin)
        {
            try
            {
                bool response = _userRepo.VerifyUserLogin(userLogin);
                if (response)
                {
                    var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.NameIdentifier, userLogin.Email),
                        new Claim("OtherProperties", "Example Role")
                    };

                    // Generate JWT Token
                    var tokenKey = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"]);
                    var tokenIssuer = _configuration["Jwt:Issuer"];
                    var tokenAudience = _configuration["Jwt:Audience"];
                    var tokenExpiryMinutes = Convert.ToDouble(_configuration["Jwt:ExpiryMinutes"]);

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(claims),
                        Expires = DateTime.UtcNow.AddMinutes(tokenExpiryMinutes),
                        Issuer = tokenIssuer,
                        Audience = tokenAudience,
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
                    };

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var JWT_Token = tokenHandler.WriteToken(token);

                    return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Message = CustomMessage.LOGIN_SUCCESSFUL });
                }
                else
                {
                    return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, Message = CustomMessage.INCORRECT_CREDENTIALS });
                }
            }
            catch (Exception ex)
            {
                return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, ErrorMessage = ex.Message, ErrorDescription = ex?.InnerException?.ToString() ?? string.Empty });
            }
        }

        [HttpPost("Logout")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Logout(string email)
        {
            try
            {
                if (_cache.TryGetValue(email, out bool isLoggedOut))
                {
                    await HttpContext.SignOutAsync(JwtBearerDefaults.AuthenticationScheme);

                    _cache.Set(email, true, TimeSpan.FromMinutes(5));

                    return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Message = CustomMessage.LOGOUT_SUCCESSFUL });
                }
                else
                {
                    return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, Message = "User not logged in." });
                }
                
            }
            catch (Exception ex)
            {
                return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, ErrorMessage = ex.Message, ErrorDescription = ex?.InnerException?.ToString() ?? string.Empty });
            }
        }

        [HttpPost("CreateUser")]
        public IActionResult CreateUser(CreateUserDTO createUser)
        {
            try
            {
                bool response = _userRepo.CreateUser(createUser);
                if (response)
                {
                    return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Message = string.Format(CustomMessage.ADDED_SUCCESSFULLY, "User") });
                }
                else
                {
                    return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, Message = CustomMessage.EMAIL_ALREADY_EXIST });
                }
            }
            catch (Exception ex)
            {
                return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, ErrorMessage = ex.Message, ErrorDescription = ex?.InnerException?.ToString() ?? string.Empty });
            }
        }

        //[HttpGet("Login")]
        //public IActionResult Login()
        //{
        //    try
        //    {
        //        var message = "";
        //        return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Message = message });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, ErrorMessage = ex.Message, ErrorDescription = ex?.InnerException?.ToString() ?? string.Empty });
        //    }
        //}

        //[HttpGet("/Payment")]
        //public IActionResult Payment()
        //{
        //    try
        //    {
        //        var message = "";
        //        return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Message = message });
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        
        //[HttpGet("/Order")]
        //public IActionResult Order()
        //{
        //    try
        //    {
        //        var message = "";
        //        return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Message = message });
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
    }
}