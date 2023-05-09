using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers
{

    [Route("api/UsersAuth")]
    [Controller]
    public class UsersController : Controller
    {
        private IUserRepository _userRepository;

        protected APIResponse response;
        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            this.response = new();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginRequestDTO model)
        {
            var loginresponse = await _userRepository.Login(model);
            if (loginresponse.User == null ) {
                response.isSuccess = false;
                response.StatusCodes = HttpStatusCode.BadRequest;
                response.ErrorMessage.Add("Username or password is Incorrect");

                return BadRequest(response);
            
            }

            response.isSuccess = true;
            response.StatusCodes = HttpStatusCode.OK;
            response.Result = loginresponse;
            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationDTO model)
        {
            bool ifUserNameUnique = _userRepository.IsUniqueUser(model.UserName);
            if(!ifUserNameUnique)
            {
                response.isSuccess = false;
                response.StatusCodes = HttpStatusCode.BadRequest;
                response.ErrorMessage.Add("Username is alreeady exist");

                return BadRequest(response);
            }
            var user = await _userRepository.Registration(model);
            if(user == null)
            {
                response.isSuccess = false;
                response.StatusCodes = HttpStatusCode.BadRequest;
                response.ErrorMessage.Add("Error while registering");

                return BadRequest(response);

            }
            response.isSuccess = true;
            response.StatusCodes = HttpStatusCode.OK;
            
            return Ok(response);
        }

    }
}
