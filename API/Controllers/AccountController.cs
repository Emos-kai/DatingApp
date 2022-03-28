using API.Data;
using API.DTO;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext Context;
        private readonly ILogger<AccountController> Logger;
        private readonly ITokenService TokenService;

        public AccountController(DataContext context, ITokenService tokenService, ILogger<AccountController> logger)
        {
            Context = context;
            Logger = logger;
            TokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<string>> Register(RegisterDTO registerDto){
            try{
                if(await UserExist(registerDto.Username)) return BadRequest("Username is taken");

                using var hmac = new HMACSHA512();
            
                var user = new AppUser(){
                    UserName = registerDto.Username.ToLower(),
                    PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                    PasswordSalt = hmac.Key
                };

                Context.Users.Add(user);
                await Context.SaveChangesAsync();

                return "User was added";
            }
            catch(Exception ex){
                Logger.LogError(ex.Message);
                return BadRequest("Somethig goes wrong!");
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDto){
            var user = await Context.Users.SingleOrDefaultAsync(u => u.UserName == loginDto.Username.ToLower());

            if(user == null) return Unauthorized("Invalid data");


            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for(int i = 0; i < computedHash.Length; i++){
                if(computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid data");
            }
            return new UserDTO{
                UserName = user.UserName,
                Token = TokenService.CreateToken(user)
            };
        }

        private async Task<bool> UserExist(string username){
            return await Context.Users.AnyAsync(u => u.UserName == username.ToLower());
        }
    }
}