using System.Net;

namespace MagicVilla_VillaAPI.Models
{
    public class APIResponse
    {
        public APIResponse() {
            ErrorMessage = new List<string>();
        
        }
        public HttpStatusCode StatusCodes{set;get;}

        public bool isSuccess { set; get; } = true;

        public List<String> ErrorMessage { get; set; }
        public object Result { set; get; }

    }
}
