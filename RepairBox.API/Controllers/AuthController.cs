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
                _configuration["Jwt:SecretKey"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        [HttpPost("Login")]
        public IActionResult Login(UserLoginDTO userLogin)
        {
            try
            {
                bool response = _userRepo.VerifyUserLogin(userLogin);
                if (response)
                {
                    string token = CreateToken(userLogin.Email);

                    var refreshToken = GenerateRefreshToken();

                    SetRefreshToken(refreshToken, userLogin.Email);

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
        [Authorize]
        public IActionResult IsLoggedIn()
        {
            try
            {
                var userEmail = _userRepo.GetMyEmail();
                return Ok(new JSONResponse { Status = ResponseMessage.SUCCESS, Message = "User is logged in.", Data = userEmail });
            }
            catch (Exception ex)
            {
                return Ok(new JSONResponse { Status = ResponseMessage.FAILURE, ErrorMessage = ex.Message, ErrorDescription = ex.InnerException?.ToString() ?? string.Empty });
            }
        }

        [HttpPost("RefreshToken")]
        public IActionResult RefreshToken()
        {
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
        [Authorize]
        public IActionResult Logout()
        {
            try
            {
                var userEmail = _userRepo.GetMyEmail();

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