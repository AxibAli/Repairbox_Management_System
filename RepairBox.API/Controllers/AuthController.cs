using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using RepairBox.API.Models;
using RepairBox.BL.DTOs.User;
using RepairBox.BL.Services;
using RepairBox.Common.Commons;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;

using RepairBox.DAL.Entities;

namespace RepairBox.API.Controllers
{
    [Route(DeveloperConstants.ENDPOINT_PREFIX)]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private IUserServiceRepo _userRepo;
        public AuthController(IConfiguration configuration, IUserServiceRepo userRepo)
        {
            _configuration = configuration;
            _userRepo = userRepo;
        }

        private RefreshToken GenerateRefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now
            };

            return refreshToken;
        }

        private void SetRefreshToken(RefreshToken newRefreshToken, string userEmail)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires
            };
            Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);

            _userRepo.SetRefreshToken(userEmail, newRefreshToken);
        }

        private string CreateToken(string userEmail)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, userEmail),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:Token"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        [HttpPost("Login")]
        public IActionResult Login(UserLoginDTO userLogin)
        {
            return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, Message = "API not implemented yet." });
            try
            {
                bool response = _userRepo.VerifyUserLogin(userLogin);
                if (response)
                {
                    string token = CreateToken(userLogin.Email);

                    var refreshToken = GenerateRefreshToken();

                    SetRefreshToken(refreshToken, userLogin.Email);

                    /*
                    var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.NameIdentifier, userLogin.Email),
                        new Claim("OtherProperties", "Example Role")
                    };

                    // Generate JWT Token
                    //var tokenKey = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"]);
                    var tokenIssuer = _configuration["Jwt:Issuer"];
                    var tokenAudience = _configuration["Jwt:Audience"];
                    var tokenExpiryMinutes = Convert.ToDouble(_configuration["Jwt:ExpiryMinutes"]);

                    int keySizeInBits = 256;

                    var tokenKey = new byte[keySizeInBits / 8];

                    using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
                    {
                        rng.GetBytes(tokenKey);
                    }

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(claims),
                        NotBefore = DateTime.UtcNow,
                        Expires = DateTime.UtcNow.AddMinutes(tokenExpiryMinutes),
                        Issuer = tokenIssuer,
                        Audience = tokenAudience,
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256)
                    };

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var JWT_Token = tokenHandler.WriteToken(token);
                    */

                    return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Message = CustomMessage.LOGIN_SUCCESSFUL, Data = token });
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

        [HttpGet("IsLoggedIn")]
        //[Authorize]
        public IActionResult IsLoggedIn()
        {
            return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, Message = "API not implemented yet." });
            try
            {
                var data = _userRepo.GetMyEmail();
                return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Message = "User is logged in.", Data = data });
            }
            catch (Exception ex)
            {
                return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, ErrorMessage = ex.Message, ErrorDescription = ex.InnerException?.ToString() ?? string.Empty });
            }
        }

        [HttpPost("RefreshToken")]
        public IActionResult RefreshToken()
        {
            return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, Message = "API not implemented yet." });
            try
            {
                var refreshToken = Request.Cookies["refreshToken"];

                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

                var tokenDTO = _userRepo.GetRefreshToken(userEmail);

                if(tokenDTO == null)
                {
                    return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, Message = string.Format(CustomMessage.NOT_FOUND, "User Token") });
                }
                else if (tokenDTO.Token.Equals(refreshToken))
                {
                    return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, Message = "Invalid Refresh Token." });
                }
                else if (tokenDTO.Expires < DateTime.Now)
                {
                    return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, Message = "Token expired." });
                }

                string token = CreateToken(userEmail);
                var newRefreshToken = GenerateRefreshToken();
                SetRefreshToken(newRefreshToken, userEmail);

                return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Message = "Token Refreshed", Data = token });
            }
            catch (Exception ex)
            {
                return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, ErrorMessage = ex.Message, ErrorDescription = ex.InnerException?.ToString() ?? string.Empty });
            }

        }

        [HttpPost("Logout")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Logout()
        {
            return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, Message = "API not implemented yet." });
            try
            {
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

                _userRepo.ClearRefreshToken(userEmail);

                // Remove the refresh token cookie
                Response.Cookies.Delete("refreshToken");

                return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Message = CustomMessage.LOGOUT_SUCCESSFUL });
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