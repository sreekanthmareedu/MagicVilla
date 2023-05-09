using MagicVilla_Utility;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Services.IServices;

namespace MagicVilla_Web.Services
{
    public class AuthServices : BaseService, IAuthService

    {

        private readonly IHttpClientFactory _clientFactory;

        private string villaUrl;

        public AuthServices(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory)
        {
            _clientFactory = clientFactory;
            villaUrl = configuration.GetValue<string>("ServiceUrls:VillaAPI");
        }

        public Task<T> LoginAsync<T>(LoginRequestDTO loginRequestDTO)
        {
            return SendAsync<T>(new Models.ApiRequest()
            {
                ApiType = SD.ApiType.POST,

                Data = loginRequestDTO,
                Url = villaUrl + "api/UsersAuth/login"
            });
        }

        public Task<T> RegisterAsync<T>(RegistrationDTO registrationDTO)
        {
            return SendAsync<T>(new Models.ApiRequest()
            {
                ApiType = SD.ApiType.POST,

                Data = registrationDTO,
                Url = villaUrl + "api/UsersAuth/register"
            });

        }
    }
}
