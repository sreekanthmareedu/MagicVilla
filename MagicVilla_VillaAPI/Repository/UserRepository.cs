using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;

namespace MagicVilla_VillaAPI.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDBContext _db;
        private string SecretKey;
        public UserRepository(ApplicationDBContext db,IConfiguration configuration)
        {
            _db = db;
            SecretKey = configuration.GetValue<string>("ApiSettings:Secret");
        }

        public bool IsUniqueUser(string username)
        {
           var user = _db.Users.FirstOrDefault(u => u.UserName.ToLower() == username.ToLower());
            if (user == null)
            {
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = _db.Users.FirstOrDefault(u=>u.Name.ToLower()==loginRequestDTO.UserName.ToLower()&&
            u.Password ==loginRequestDTO.Password
            
            );
            if (user == null)
            {
                 new LoginResponseDTO()
                {
                    Token = " ",
                    User = null
                };

            }
           var tokenhandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(SecretKey);

            var tokendiscriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)

                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenhandler.CreateToken(tokendiscriptor);
            LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
            {
                Token = tokenhandler.WriteToken(token),
                User = user
            };
            return loginResponseDTO;

        }

        public async Task<User> Registration(RegistrationDTO registrationDTO)
        {
            User user = new()
            {
                Name = registrationDTO.Name,
                UserName = registrationDTO.UserName,
                Password = registrationDTO.Password,
                Role = registrationDTO.Role

            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            user.Password = " ";
            return user;
        }

    }
}
